<H1>Deploy and configure</H1>

  1. Setup and configure Digital Twin service in Azure

  2. Grant permissions to your application

  3. Modify a Digital Twins application

  4. Configure UDF

  5. Execute provision your parking places
  

<H2>Setup and configure Digital Twin service in Azure</H2>
1. To setup Digital Twin service needs to go on Azure portal and create new resource group called "Digital-Twin-Solution"

![](https://github.com/sergiibielskyi/parking/blob/master/images/resources.png)

2. Then you need to create Digital twin
 
 ![](https://github.com/sergiibielskyi/parking/blob/master/images/twin.png)
 
 ![](https://github.com/sergiibielskyi/parking/blob/master/images/digitaltwin.png)
 
3. Check your space using swagger - https://{your location}.westeurope.azuresmartspaces.net/management/swagger/ui/index

 ![](https://github.com/sergiibielskyi/parking/blob/master/images/swagger.png)

4. After creating Digital Twin you need to create App registration
Digital Twin uses AAD to control the services (read/write). To register digital twin needs to have AAD ID and AppID. To get AAD ID you need to go to the AAD section and select Properties. Сopy Directory Id in temporary note. 

 ![](https://github.com/sergiibielskyi/parking/blob/master/images/AAD.png)
 
5. Then you need to select App registrations and create new registration.

 ![](https://github.com/sergiibielskyi/parking/blob/master/images/registrations.png)
 
 6. At the end you should grant nessosary permissions.
 
  ![](https://github.com/sergiibielskyi/parking/blob/master/images/grant.png)
  
  ![](https://github.com/sergiibielskyi/parking/blob/master/images/permissions.png)

7. To get the digital twin application you need to execute the command 
```
git clone https://github.com/sergiibielskyi/parking.git
```
Open the application src/digital and run the command dotnet restore. To configure the connection to your digital twin you need to open src/digital/config/appSettings.json and put your ADD ID and App ID and your location to digital twin.
```
{
    "AADInstance": "https://login.microsoftonline.com/",
    "ClientId": "App ID",
    "Tenant": "ADD ID",
    "BaseUrl": "https://your location.westeurope.azuresmartspaces.net/management/api/v1.0/"
}
```
8. Open the src/digital/config/Provision.yaml to see space and resources that we will deploy.
```
- name: Parking places
  type: Venue
  resources:
  - type: IoTHub
  spaces:
  - name: Space 1
    type: Space
    spaces:
    - name: Area A
      type: Area
    - name: Place 1
      type: Place
      subType: Paid
    - name: Place 2
      type: Place
      subType: Free
      devices:
      - name: Raspberry Pi 3 A1
        hardwareId: 1234567890AB
        sensors:
        - dataType: Motion
          hardwareId: SAMPLE_SENSOR_MOTION
      matchers:
      - name: Matcher Motion A1
        dataTypeValue: Motion
      userdefinedfunctions:
      - name: Motion Processor
        matcherNames:
        - Matcher Motion A1
        script: config/udf/availability.js
      roleassignments:
      - roleId: 98e44ad7-28d4-4007-853b-b9968ad132d1 # System Role: SpaceAdministrator
        objectName: Motion Processor
        objectIdType: UserDefinedFunctionId
```
9. To configure UDF you need to open the file src/config/udf/availibility.js and check if everything is ok.

10. Run the command
```
dotnet run ProvisionParking
```
11. After deploying you will see the connection string to the generated device. Copy this string.

![](https://github.com/sergiibielskyi/parking/blob/master/images/connection.png)