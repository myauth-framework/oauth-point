FROM mcr.microsoft.com/dotnet/core/aspnet

ADD ./out/app /app/

ENTRYPOINT cd /app && dotnet MyAuth.OAuthPoint.dll
