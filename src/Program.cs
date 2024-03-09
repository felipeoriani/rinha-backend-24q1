using Microsoft.AspNetCore.Mvc;
using Npgsql;
using NpgsqlTypes;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ConnectionString");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new Exception("The connectionString is not defined.");

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, RinhaJsonSerializerContext.Default);
});

var dataSource = NpgsqlDataSource.Create(connectionString);
builder.Services.AddScoped(_ => dataSource.CreateConnection());

int[] limites =
[
    100_000,
    80_000,
    1_000_000,
    10_000_000,
    500_000
];

var app = builder.Build();

app.MapPost("/clientes/{id}/transacoes", async (int id, Transacao transacao, [FromServices] NpgsqlConnection connection) =>
{
    if (id > 5)
        return Results.NotFound();

    if (transacao.Tipo is not ("c" or "d") ||
        transacao.Descricao is null or "" or { Length: > 10 })
        return Results.UnprocessableEntity();

    var limite = limites[id - 1];

    using (connection)
    {
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT crebitar_{transacao.Tipo}($1, $2, $3, $4)";

        command.Parameters.AddWithValue(NpgsqlDbType.Integer, id);
        command.Parameters.AddWithValue(NpgsqlDbType.Integer, transacao.Valor);
        command.Parameters.AddWithValue(NpgsqlDbType.Varchar, transacao.Descricao);
        command.Parameters.AddWithValue(NpgsqlDbType.Integer, -limite);

        object? saldo = await command.ExecuteScalarAsync();

        if (saldo is DBNull or null or not int)
            return Results.UnprocessableEntity();

        return Results.Ok(new TransacaoResposta((int) saldo, limite));
    }
});

app.MapGet("/clientes/{id}/extrato", async (int id, [FromServices] NpgsqlConnection connection) =>
{
    if (id > 5)
        return Results.NotFound();

    using (connection)
    {
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT valor_out, tipo_out, descricao_out FROM extrato($1)";
        command.Parameters.AddWithValue(NpgsqlDbType.Integer, id);

        await using var reader = await command.ExecuteReaderAsync();

        await reader.ReadAsync();

        var saldo = new Saldo(reader.GetInt32(0), DateTime.Now, limites[id - 1]);

        var transacoes = new List<TransacaoExtrato>(10);

        while (await reader.ReadAsync())
            transacoes.Add(new(reader.GetInt32(0), reader.GetString(1), reader.GetString(2)));

        return Results.Ok(new Extrato(saldo, transacoes));
    }
});

app.Run();

public record Transacao(int Valor, string Tipo, string Descricao);
public record TransacaoResposta(int Saldo, int Limite);
public record Saldo(int Total, DateTime Data_extrato, int Limite);
public record TransacaoExtrato(int Valor, string Tipo, string Descricao);
public record Extrato(Saldo saldo, List<TransacaoExtrato> Ultimas_transacoes);

[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(DateTime))]
[JsonSerializable(typeof(Saldo))]
[JsonSerializable(typeof(Transacao))]
[JsonSerializable(typeof(List<TransacaoExtrato>))]
[JsonSerializable(typeof(Extrato))]
[JsonSerializable(typeof(TransacaoResposta))]
internal partial class RinhaJsonSerializerContext : JsonSerializerContext { }
