# BUILD .NET CORE APP

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY . .
RUN dotnet restore

#build migrator
WORKDIR /app/SpecBox.Migrations
RUN dotnet publish -c Release -o out

# publish
WORKDIR /app/SpecBox.WebApi
RUN dotnet publish -c Release -o out

# install migrator
RUN dotnet tool install -g thinkinghome.migrator.cli

# PREPARE RUNTIME
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime

# install utils
RUN apt-get update && DEBIAN_FRONTEND=noninteractive apt-get install -y locales tzdata wget iputils-ping

# lang
ENV LANG=en_US.UTF-8
RUN locale-gen "en_US.UTF-8" && dpkg-reconfigure --frontend noninteractive locales

# timezone
RUN ln -fs /usr/share/zoneinfo/Europe/Moscow /etc/localtime && dpkg-reconfigure --frontend noninteractive tzdata
# prepare application
WORKDIR /app
# copy migrations
COPY --from=build /app/SpecBox.Migrations/out ./
# copy app
COPY --from=build /app/SpecBox.WebApi/out ./
# copy migration tool
COPY --from=build /root/.dotnet/tools ./
# copy entrypoint script
COPY entrypoint.sh ./

RUN rm -f appsettings.*.json

EXPOSE 80
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["./entrypoint.sh"]
