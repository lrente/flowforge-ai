FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY FlowForge.sln ./
COPY FlowForge.Api/FlowForge.Api.csproj ./FlowForge.Api/
COPY FlowForge.Application/FlowForge.Application.csproj ./FlowForge.Application/
COPY FlowForge.Domain/FlowForge.Domain.csproj ./FlowForge.Domain/
COPY FlowForge.Infrastructure/FlowForge.Infrastructure.csproj ./FlowForge.Infrastructure/
COPY FlowForge.Tests/FlowForge.Tests.csproj ./FlowForge.Tests/

RUN dotnet restore FlowForge.sln

COPY . ./
RUN dotnet publish FlowForge.Api/FlowForge.Api.csproj -c Release -o /app/publish
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "FlowForge.Api.dll"]
