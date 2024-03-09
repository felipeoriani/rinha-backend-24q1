cd src
docker buildx build --platform linux/amd64 -t rinha-backend-y24q1-dotnet-csharp .
cd ..
docker compose up -d