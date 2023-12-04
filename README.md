# SelectronicMQTT

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
