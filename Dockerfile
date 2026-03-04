# بناء ونشر في مرحلة واحدة
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# نسخ كل شيء
COPY . .

# النشر مباشرة - استخدام المسار الصحيح
RUN dotnet publish ArabicChatBot.csproj -c Release -o out

# مرحلة التشغيل
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 8080
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "ArabicChatBot.dll"]