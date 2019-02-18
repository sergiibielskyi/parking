<H1>Configure simulator</H1>

  1. Simulate the data

  2. Find the results

In this section you will learn how to generate the telemetry for our device. First, you need to open the project src/simulate and run the command dotnet restore.
Then you need to put connection string of device in appsettings.json.

![](https://github.com/sergiibielskyi/parking/blob/master/images/device.png)

To run the simulator you need to execute 
```
dotnet run
```

![](https://github.com/sergiibielskyi/parking/blob/master/images/telemetry.png)