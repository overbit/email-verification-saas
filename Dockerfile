FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["EmailVerificationService.csproj", ""]
RUN dotnet restore "EmailVerificationService.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "EmailVerificationService.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "EmailVerificationService.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "EmailVerificationService.dll"]