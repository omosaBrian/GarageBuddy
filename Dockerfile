# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy solution and project files
COPY *.sln ./
COPY GarageBuddy.Web/*.csproj ./GarageBuddy.Web/
RUN dotnet restore

# Copy all files and build
COPY . ./
WORKDIR /app/GarageBuddy.Web
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/GarageBuddy.Web/out .
ENTRYPOINT ["dotnet", "GarageBuddy.Web.dll"]
