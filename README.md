# Rinha de Backend 2024 Q1 - felipeoriani

`pt-BR`: Este repositório consiste em apresentar o código da minha participação na [Rinha de Backend](https://twitter.com/rinhadebackend) 2024 Q1 origanizada pelo [@zanfranceschi](https://twitter.com/zanfranceschi). Nesta edição o tema foi o gerenciamento de concorrência e tendo como tópico transações bancárias seguindo alguns critérios (exemplo: `1.5` de CPU e `550Mb` de RAM para toda a infraestrutura). Você consegue saber mais sobre os detalhes do projeto diretamente no [Repositório da Rinha](https://github.com/zanfranceschi/rinha-de-backend-2024-q1), sendo que [neste link](https://github.com/felipeoriani/rinha-de-backend-2024-q1) você encontra o meu fork.

`en-US`: This repository shows the code from my participation in the [Rinha de Backend](https://twitter.com/rinhadebackend) 2024 Q1 organized by [@zanfranceschi](https://twitter.com/zanfranceschi). In this edition the topic was concurrency management on banking transactions following some criterias (e.g.: `1.5` cpu and `550Mb` ram for all the infrastructure). You can find out more about on [Rinha de Backend repository](https://github.com/zanfranceschi/rinha-de-backend-2024-q1), where in [this link](https://github.com/felipeoriani/rinha-de-backend-2024-q1) you will find my fork.

Lessons Learnt:
- Windows dynamic ports can block the Load Testing after some time;
- Handle concurrency in Postgres with `pg_advisory_xact_lock` vs `FOR UPDATE`;
- `ConnectionPool` configurations for `Postgres`, super flexible;
- `NpgsqlDataSource` singleton instance can be helpful in this kind of scenario;
- `Nginx` as Load Balancer is really good and get the job done;
- Optimizations in `csproj` for compilations including `Native AOT` in .NET 8 and speed up the process and the runtime;
- Linux Ubuntu setup for .NET (I used to use just Windows);

## Stack:
<table align=center>
  <tr>
    <td><img src="https://upload.wikimedia.org/wikipedia/commons/thumb/7/7d/Microsoft_.NET_logo.svg/456px-Microsoft_.NET_logo.svg.png" alt="logo .NET" width="100" height="auto"></td>
    <td><img src="https://upload.wikimedia.org/wikipedia/commons/thumb/d/d2/C_Sharp_Logo_2023.svg/1280px-C_Sharp_Logo_2023.svg.png" alt="logo CSharp" width="100" height="auto"></td>
    <td><img src="https://upload.wikimedia.org/wikipedia/commons/2/29/Postgresql_elephant.svg" alt="logo PostgreSql" width="100" height="auto"></td>
    <td><img src="https://upload.wikimedia.org/wikipedia/commons/c/c5/Nginx_logo.svg" alt="logo Nginx" width="100" height="auto"></td>
  </tr>
</table>

## How to Run

Build the docker image for the project and up all the dependencies on the `docker-compose.yml` including `Postgres` database and `Nginx` load balancer including all its configurations.

```
sh up.sh
```

After the load balancer will be listening on `9999` port, and you can request to it as an example:

```
GET http://localhost:9999/clientes/1/extrato
```

or

```
POST http://localhost:9999/clientes/1/transacoes
```

using the request body:

```
{
    "valor": 1000,
    "tipo" : "c",
    "descricao" : "descricao"
}
```

Finally, you can down all the resources and destroy the container image by running:

```
sh down.sh
```

## Load Testing Results

Once I was developing this project a Load Test using Gatling ran in order to evaluate the infrastructure. You can check the results on the image bellow:

![image](https://github.com/felipeoriani/rinha-backend-24q1/assets/865173/f00cbbf6-5693-4eec-a67b-0e6a655a3256)

Thank you.

- https://github.com/felipeoriani
- https://www.linkedin.com/in/felipeoriani/
- https://twitter.com/felipeoriani
