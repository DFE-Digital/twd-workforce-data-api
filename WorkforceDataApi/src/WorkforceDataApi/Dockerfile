# syntax=docker/dockerfile:1
  FROM mcr.microsoft.com/dotnet/aspnet:7.0
  ARG GIT_SHA
  ENV GitSha ${GIT_SHA}
  COPY bin/Release/net7.0/publish/ App/
  WORKDIR /App
  ENTRYPOINT ["dotnet", "WorkforceDataApi.dll"]
  EXPOSE 80
