FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Lastlink.Api/Lastlink.Api.csproj", "Lastlink.Api/"]
COPY ["Lastlink.Application/Lastlink.Application.csproj", "Lastlink.Application/"]
COPY ["Lastlink.Domain/Lastlink.Domain.csproj", "Lastlink.Domain/"]
COPY ["Lastlink.Infrastructure/Lastlink.Infrastructure.csproj", "Lastlink.Infrastructure/"]
COPY ["Lastlink.Produtos.sln", "./"]

RUN dotnet restore "Lastlink.Api/Lastlink.Api.csproj"

COPY . .
WORKDIR /src/Lastlink.Api
RUN dotnet publish "Lastlink.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Lastlink.Api.dll"]
