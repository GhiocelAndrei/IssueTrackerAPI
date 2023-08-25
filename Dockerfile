# Build Stage

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "./src/IssueTrackerAPI/IssueTrackerAPI.csproj"
RUN dotnet publish "./src/IssueTrackerAPI/IssueTrackerAPI.csproj" -c release -o /app --no-restore

# Serve Stage

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app ./

EXPOSE 80
ENTRYPOINT ["dotnet", "IssueTrackerAPI.dll"]
