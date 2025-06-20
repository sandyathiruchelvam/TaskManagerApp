FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TaskManagerApp/TaskManagerApp.csproj", "TaskManagerApp/"]
RUN dotnet restore "TaskManagerApp/TaskManagerApp.csproj"
COPY . .
WORKDIR "/src/TaskManagerApp"
RUN dotnet build "TaskManagerApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaskManagerApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskManagerApp.dll"]
