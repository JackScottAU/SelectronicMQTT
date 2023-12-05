# SelectronicMQTT

SelectronicMQTT is a .NET self-hosted service to link Selectronic solar inverter systems to home automations systems such as Home Assistant. 
It does this by fetching data from the `select.live` web application run by Selectronic, and publishing this to a local MQTT server.

## Installation

1. Ensure the .NET v8.0 SDK is installed on your system.
3. Run the following commands in Bash/PowerShell:
   ```
   git clone https://github.com/JackScottAU/SelectronicMQTT.git
   cd SelectronicMQTT/SelectronicMQTT.Service
   dotnet build
   ```
4. Configure application `appsettings.json` using configuration information detailed below.
5. Run the application:
   ```
   dotnet run
   ```

### Running as a Background Process (Linux-based Systems)

I run the application on a Ubuntu system as a daemon using the following systemd service file (`/etc/systemd/system/selectronicmqtt.service`):

```
[Unit]
Description=SelectronicMQTT Service

[Install]
WantedBy=multi-user.target

[Service]
# will set the Current Working Directory (CWD)
WorkingDirectory=/home/jscott/SelectronicMQTT/SelectronicMQTT.Service/bin/Debug/net8.0/
# systemd will run this executable to start the service
ExecStart=/usr/bin/dotnet "/home/jscott/SelectronicMQTT/SelectronicMQTT.Service/bin/Debug/net8.0/SelectronicMQTT.Service.dll"
# to query logs using journalctl, set a logical name here
SyslogIdentifier=SelectronicMQTT

# Use your username to keep things simple, for production scenario's I recommend a dedicated user/group.
# If you pick a different user, make sure dotnet and all permissions are set correctly to run the app.
# To update permissions, use 'chown yourusername -R /srv/AspNetSite' to take ownership of the folder and files,
#       Use 'chmod +x /srv/AspNetSite/AspNetSite' to allow execution of the executable file.
User=jscott

# ensure the service restarts after crashing
Restart=always
# amount of time to wait before restarting the service
RestartSec=30

# copied from dotnet documentation at
# https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-3.1#code-try-7
KillSignal=SIGINT
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

# Application Configuration
Environment=Selectronic__Email=""
Environment=Selectronic__Password=""
Environment=Selectronic__SystemNumber=""
Environment=Mqtt__Hostname=""
Environment=Mqtt__Username=""
Environment=Mqtt__Password=""
Environment=Mqtt__UniqueID=""
Environment=Mqtt__SendHomeAssistantDiscoveryMessages=true
```

Change `WorkingDirectory` and `ExecStart` as appropriate for your system, as well as the `Environment=` variables.

## Configuration

Required appsettings.json configuration:

```
  "Selectronic": {
    "Email": "",
    "Password": "",
    "SystemNumber": ""
  },
  "Mqtt": {
    "Hostname": "",
    "Username": "",
    "Password": "",
    "UniqueID": "",
    "SendHomeAssistantDiscoveryMessages": true
  }
```

The system number is the end portion of the select.live URL (likely to be a number with a 1-3 digit length). I use the same thing for the uniqueID.
