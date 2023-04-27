FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /source

RUN apt update && apt upgrade -y && apt install npm -y

RUN echo "Cloning code"
RUN git clone https://github.com/PoolPirate/MDAO-Challenge-Bot.git Code \
    && cd Code \
    && dotnet publish -c release -o /app


FROM mcr.microsoft.com/dotnet/runtime:7.0
WORKDIR /app
COPY --from=build /app ./
COPY appsettings.json /app/appsettings.json
ENTRYPOINT ["dotnet", "MDAO-Challenge-Bot.dll"]
