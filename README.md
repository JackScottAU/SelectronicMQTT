# SelectronicMQTT

SelectronicMQTT is a .NET self-hosted service to link Selectronic solar inverter systems to home automations systems such as Home Assistant. 
It does this by fetching data from the `select.live` web application run by Selectronic, and publishing this to a local MQTT server.

## Installation

1. Ensure the .NET v8.0 SDK is installed on your system.
3. ```
   git clone https://github.com/JackScottAU/SelectronicMQTT.git
   cd SelectronicMQTT/SelectronicMQTT.Service
   dotnet build
   ```
4. Configure application `appsettings.json` using configuration information detailed below.
5. `dotnet run`

Running the application as a background process is left as an exercise to the reader.

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
