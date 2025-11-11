# ================================
# Étape 1 : Build de l'application
# ================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copier le fichier projet et restaurer les dépendances NuGet
COPY *.csproj ./
RUN dotnet restore

# Copier le reste du code source
COPY . ./

# Compiler et publier l'application en mode Release
RUN dotnet publish -c Release -o /app

# ================================
# Étape 2 : Image Runtime
# ================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copier les fichiers publiés depuis l'étape build
COPY --from=build /app .

# Exposer le port 8080 (Render ou Docker local)
EXPOSE 8080

# Lancer l'application
ENTRYPOINT ["dotnet", "PayrollApp.dll"]
