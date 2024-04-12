using Measurer;
using Newtonsoft.Json.Linq;

internal class Program
{
    private static void Main()
    {
        string jsonFilePath = "Datas/SlabMeasure0132981.json";
        double noiseThreshold = 0.5;
        int maxGap = 2; 

        var measurementData = File.ReadAllText(jsonFilePath);
        var measurements = JObject.Parse(measurementData)["Meterings"].ToObject<List<Measurement>>();

        var intervals = new List<MeasurementInterval>();
        MeasurementInterval currentInterval = null;
        int noiseCounter = 0;
        int previousElapsedTime = 0;

        foreach (var measurement in measurements)
        {
            if (measurement.Speed > noiseThreshold)
            {
                if (currentInterval == null)
                {
                    currentInterval = new MeasurementInterval { Start = measurement.ElapsedTime };
                }
                else if (noiseCounter <= maxGap)
                {
                  
                    noiseCounter = 0;
                }
                else
                {                   
                    currentInterval.End = previousElapsedTime;
                    intervals.Add(currentInterval);
                    currentInterval = new MeasurementInterval { Start = measurement.ElapsedTime };
                }

                previousElapsedTime = measurement.ElapsedTime; 
            }
            else if (currentInterval != null)
            {
                noiseCounter++;
            }
        }
              
        if (currentInterval != null && noiseCounter <= maxGap)
        {
            currentInterval.End = previousElapsedTime;
            intervals.Add(currentInterval);
        }
       
        foreach (var interval in intervals)
        {
            Console.WriteLine($"Measurement interval from {interval.Start} to {interval.End}");
        }
    }
}