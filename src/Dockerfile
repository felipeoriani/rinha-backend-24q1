FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    clang zlib1g-dev
WORKDIR /src
COPY ["RinhaBackEndY24Q1.csproj", "."]
RUN dotnet restore -r linux-x64 "./././RinhaBackEndY24Q1.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./RinhaBackEndY24Q1.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./RinhaBackEndY24Q1.csproj" -c Release -o /app/publish /p:UseAppHost=true

FROM mcr.microsoft.com/dotnet/runtime-deps:8.0 AS final
WORKDIR /app
EXPOSE 80
COPY --from=publish /app/publish .
ENTRYPOINT ["./RinhaBackEndY24Q1"]