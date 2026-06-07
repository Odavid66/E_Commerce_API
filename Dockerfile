# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the csproj and restore dependencies
COPY ["E-commerce_API.csproj", "./"]
RUN dotnet restore "E-commerce_API.csproj"

# Copy the remaining files and build the app
COPY . .
RUN dotnet build "E-commerce_API.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "E-commerce_API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Run
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# .NET defaults to port 8080. Render will detect EXPOSE and route traffic here.
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# The compiled application DLL to execute
ENTRYPOINT ["dotnet", "E-commerce_API.dll"]