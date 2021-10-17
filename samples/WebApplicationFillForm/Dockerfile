#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["samples/WebApplicationFillForm/WebApplicationFillForm.csproj", "samples/WebApplicationFillForm/"]
COPY ["src/Kevsoft.PDFtk/Kevsoft.PDFtk.csproj", "src/Kevsoft.PDFtk/"]
RUN dotnet restore "samples/WebApplicationFillForm/WebApplicationFillForm.csproj"
COPY . .
WORKDIR "/src/samples/WebApplicationFillForm"
RUN dotnet build "WebApplicationFillForm.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApplicationFillForm.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApplicationFillForm.dll"]