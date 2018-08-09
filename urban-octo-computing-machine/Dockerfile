FROM microsoft/dotnet:2.1-runtime-nanoserver-1709 AS base
MAINTAINER Keith McCartney "keithtmccartney@hotmail.com"

# Make sure we have S3 & additional libraries
RUN apt-get update -qq \
	&& apt-get install -y git \
	&& mkdir /tmp/emitter \
	&& cd /tmp/emitter \
	&& git clone "https://github.com/keithtmccartney/urban-octo-computing-machine.git" "/tmp/emitter" \
	&& cd /tmp/emitter \
	&& dotnet restore -v Minimal \
	&& cd /tmp/emitter/src/Server \
	&& dotnet publish -c Release --output /app \
	&& rm -rf /tmp/emitter

# Application will be in app folder
WORKDIR /app
ADD deploy.sh /
CMD ["/bin/bash", "/deploy.sh"]

FROM microsoft/dotnet:2.1-sdk-nanoserver-1709 AS build
WORKDIR /src
COPY urban-octo-computing-machine/urban-octo-computing-machine.csproj urban-octo-computing-machine/
RUN dotnet restore urban-octo-computing-machine/urban-octo-computing-machine.csproj
COPY . .
WORKDIR /src/urban-octo-computing-machine
RUN dotnet build urban-octo-computing-machine.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish urban-octo-computing-machine.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "urban-octo-computing-machine.dll"]
