# Build Frontend
FROM node:20-alpine AS frontend-build
WORKDIR /app/frontend
COPY frontend/package*.json ./
RUN npm install
COPY frontend/ ./
# Force build output to 'dist' folder regardless of vite.config.js settings
RUN npx vite build --outDir dist

# Build Backend
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS backend-build
WORKDIR /app
COPY backend/ ./
# Copy frontend build output to backend wwwroot
# Note: HDEMS.Api/wwwroot should exist or will be created
COPY --from=frontend-build /app/frontend/dist ./HDEMS.Api/wwwroot
RUN dotnet restore HDEMS.sln
RUN dotnet publish HDEMS.Api/HDEMS.Api.csproj -c Release -o /app/publish

# Final Image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=backend-build /app/publish .
# Create a logs directory for Serilog
RUN mkdir -p logs && chmod 777 logs
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "HDEMS.Api.dll"]
