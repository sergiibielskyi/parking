var motionType = "Motion";
var placeAvailable = "Available";
// Add your sensor type here

function process(telemetry, executionContext) {

    try {
        // Log SensorId and Message
        log(`Sensor ID: ${telemetry.SensorId}. `);
        log(`Sensor value: ${JSON.stringify(telemetry.Message)}.`);

        // Get sensor metadata
        var sensor = getSensorMetadata(telemetry.SensorId);

        // Retrieve the sensor reading
        var parseReading = JSON.parse(telemetry.Message);

        // Set the sensor reading as the current value for the sensor.
        setSensorValue(telemetry.SensorId, sensor.DataType, parseReading.SensorValue);

        // Get parent space
        var parentSpace = sensor.Space();

        // Get children sensors from the same space
        var otherSensors = parentSpace.ChildSensors();

        var motionSensor = otherSensors.find(function(element) {
            return element.DataType === motionType;
        });
       
        // get latest values for above sensors
        var motionValue = motionSensor.Value().Value;
        var presence = !!motionValue && motionValue.toLowerCase() === "true";
        
        // Return if no motion found return
        // Modify this line to monitor your sensor value
        if(motionValue === null){
            sendNotification(telemetry.SensorId, "Sensor", "Error: motion is null, returning");
            return;
        }

        // Modify these lines as per your sensor
        var alert = "Place is available.";
        var noAlert = "Place is not available";

        // Modify this code block for your sensor
        // If sensor values are within range and room is available
        if(!presence) {
            log(`${alert}. Presence: ${presence}.`);

            // log, notify and set parent space computed value
            setSpaceValue(parentSpace.Id, placeAvailable, alert);

            // Set up notification for this alert
            parentSpace.Notify(JSON.stringify(alert));
        }
        else {
            log(`${noAlert}. Presence: ${presence}.`);

            // log, notify and set parent space computed value
            setSpaceValue(parentSpace.Id, placeAvailable, noAlert);
        }
    }
    catch (error)
    {
        log(`An error has occurred processing the UDF Error: ${error.name} Message ${error.message}.`);
    }
}

function getFloatValue(str) {
  if(!str) {
      return null;
  }

  return parseFloat(str);
}
