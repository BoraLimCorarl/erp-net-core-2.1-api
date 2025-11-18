#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:2.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:2.1 AS build
WORKDIR /src
COPY ["src/CorarlERP.Web.Host/CorarlERP.Web.Host.csproj", "src/CorarlERP.Web.Host/"]
COPY ["src/CorarlERP.Web.Core/CorarlERP.Web.Core.csproj", "src/CorarlERP.Web.Core/"]
COPY ["src/CorarlERP.Application/CorarlERP.Application.csproj", "src/CorarlERP.Application/"]
COPY ["src/CorarlERP.Application.Shared/CorarlERP.Application.Shared.csproj", "src/CorarlERP.Application.Shared/"]
COPY ["src/CorarlERP.Core.Shared/CorarlERP.Core.Shared.csproj", "src/CorarlERP.Core.Shared/"]
COPY ["src/CorarlERP.Core/CorarlERP.Core.csproj", "src/CorarlERP.Core/"]
COPY ["src/CorarlERP.EntityFrameworkCore/CorarlERP.EntityFrameworkCore.csproj", "src/CorarlERP.EntityFrameworkCore/"]
RUN dotnet restore "src/CorarlERP.Web.Host/CorarlERP.Web.Host.csproj"
COPY . .
WORKDIR "/src/src/CorarlERP.Web.Host"
RUN dotnet build "CorarlERP.Web.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CorarlERP.Web.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CorarlERP.Web.Host.dll"]
