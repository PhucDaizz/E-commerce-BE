FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY "E-commerce.sln" .
COPY ECommerce.API/ECommerce.API.csproj ECommerce.API/
COPY Ecommerce.Application/Ecommerce.Application.csproj Ecommerce.Application/ 
COPY Ecommerce.Domain/Ecommerce.Domain.csproj Ecommerce.Domain/
COPY Ecommerce.Infrastructure/Ecommerce.Infrastructure.csproj Ecommerce.Infrastructure/

RUN dotnet restore "E-commerce.sln"

COPY . .

WORKDIR "/src/ECommerce.API"
RUN dotnet publish "ECommerce.API.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 7295
ENV ASPNETCORE_URLS=http://+:7295

USER root
COPY --from=build /app/publish .
USER app

ENTRYPOINT ["dotnet", "ECommerce.API.dll"]

