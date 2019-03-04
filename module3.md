<H1>Configure Logic App for routing</H1>

1. Configure Event Hub

2. Configure enpoints

3. Configure Cosmos DB for keeping telemetry

4. Configure Teams channel for notification

5. Build Logic App with routing

To configure Event Hub you need to create new namespace Event Hub
![](https://github.com/sergiibielskyi/parking/blob/master/images/eventhub.png)

Then you need to create one instance of Event Hub

![](https://github.com/sergiibielskyi/parking/blob/master/images/createhub.png)

Next step is configure endpoints for connecting to the Event Hub. For this action you need to open src/config/Endpoints.yaml and put Primary and Secondary connection keys.

![](https://github.com/sergiibielskyi/parking/blob/master/images/connectionhub.png)

Don't forget to put the name of Event Hub instance as well.

Next step you need to create Cosmos DB with SQL API and one container.

![](https://github.com/sergiibielskyi/parking/blob/master/images/cosmosdb.png)

For creation teams channel is easy to open the existing teams and create new channel.

![](https://github.com/sergiibielskyi/parking/blob/master/images/teams.png)

Next step is execute the scipt to create endpoint.

```
dotnet run CreateEndpoints
```
Last thing to do is creating Logic App for routing our messages.

![](https://github.com/sergiibielskyi/parking/blob/master/images/logicapp.png)

![](https://github.com/sergiibielskyi/parking/blob/master/images/step1.png)

In step2 upload existing schema src/digital/config/motion.json

![](https://github.com/sergiibielskyi/parking/blob/master/images/step2.png)

![](https://github.com/sergiibielskyi/parking/blob/master/images/step3.png)

![](https://github.com/sergiibielskyi/parking/blob/master/images/step4.png)

![](https://github.com/sergiibielskyi/parking/blob/master/images/step5.png)

![](https://github.com/sergiibielskyi/parking/blob/master/images/step6.png)

![](https://github.com/sergiibielskyi/parking/blob/master/images/step7.png)

To test how it works you need to start simulate application and wait the results
