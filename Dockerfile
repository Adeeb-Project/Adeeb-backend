# **Stage 1: Build**
FROM --platform=linux/arm64 mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project files and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the source code
COPY . ./

# Publish the application
RUN dotnet publish -c Release -o /out

# **Stage 2: Runtime**
FROM --platform=linux/arm64 mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the published output
COPY --from=build /out ./

ENV ASPNETCORE_ENVIRONMENT=Development

# Expose the required ports
EXPOSE 80
EXPOSE 443
EXPOSE 8080

# **Apply migrations and start the app**
ENTRYPOINT ["sh", "-c", "dotnet AdeebBackend.dll --migrate && dotnet AdeebBackend.dll"]
