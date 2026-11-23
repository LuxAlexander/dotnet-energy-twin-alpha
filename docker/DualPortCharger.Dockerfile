FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

COPY Directory.Packages.props ./
COPY src/Directory.Build.props src/
COPY src/Devices/DualPortCharger/ src/Devices/DualPortCharger/
COPY src/Devices.Common/ src/Devices.Common/
COPY src/Modbus.Contracts/ src/Modbus.Contracts/
COPY src/Shared/ src/Shared/
COPY src/Shared.Config/ src/Shared.Config/
COPY src/SimCore.Contracts/ src/SimCore.Contracts/
COPY src/SimCore.Time/ src/SimCore.Time/
COPY src/SimCore.TimeSync/ src/SimCore.TimeSync/
RUN dotnet publish src/Devices/DualPortCharger/DualPortCharger.csproj -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /publish/ ./
ENV ENERGYTWIN_DEVICE_CONTROL_DIR=/run/energytwin/control
RUN mkdir -p /data "$ENERGYTWIN_DEVICE_CONTROL_DIR" && chown "$APP_UID" /data "$ENERGYTWIN_DEVICE_CONTROL_DIR"
USER $APP_UID
ENTRYPOINT ["dotnet", "DualPortCharger.dll"]
CMD ["--master", "host.docker.internal:5100", "--weather", "http://host.docker.internal:8080", "--modbus-bind", "0.0.0.0", "--output", "/data"]
