# Etapa 1 — build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["TaskManagerAPI/TaskManagerAPI.csproj", "TaskManagerAPI/"]
RUN dotnet restore "TaskManagerAPI/TaskManagerAPI.csproj"

COPY . .
WORKDIR "/src/TaskManagerAPI"
RUN dotnet publish -c Release -o /app/publish

# Etapa 2 — runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TaskManagerAPI.dll"]