using Microsoft.Azure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PatrolCar
{
    class Program
    {
        private static CancellationTokenSource tokenSource = new CancellationTokenSource();
        static void Main(string[] args)
        {
            string connectionString = CloudConfigurationManager.GetSetting("IotHubConnectionString");
            string iotHubUri = CloudConfigurationManager.GetSetting("IotHubUri");
            string numCars = CloudConfigurationManager.GetSetting("NumCars");
            string latNorth = CloudConfigurationManager.GetSetting("LatitudeNorth");
            string latSouth = CloudConfigurationManager.GetSetting("LatitudeSouth");
            string longEast = CloudConfigurationManager.GetSetting("LongitudeEast");
            string longWest = CloudConfigurationManager.GetSetting("LongitudeWest");
            string milesPerDegreeLat = CloudConfigurationManager.GetSetting("MilesPerDegreeLatitude");
            string milesPerDegreeLong = CloudConfigurationManager.GetSetting("MilesPerDegreeLongitude");
            string locationReportingInterval = CloudConfigurationManager.GetSetting("LocationReportingInterval");
            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(numCars) || 
                string.IsNullOrEmpty(latNorth) || string.IsNullOrEmpty(latSouth) ||
                string.IsNullOrEmpty(longEast) || string.IsNullOrEmpty(longWest) ||
                string.IsNullOrEmpty(locationReportingInterval) || string.IsNullOrEmpty(iotHubUri))
            {
                Trace.WriteLine("One or more parameters missing in appsettings (app.config)");
                return;
            }
            
            // Change Culture settings to en-US
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            var driver = new PatrolCarsDriver(connectionString, iotHubUri, int.Parse(numCars), double.Parse(latNorth), 
                double.Parse(latSouth), double.Parse(longEast), double.Parse(longWest), 
                double.Parse(milesPerDegreeLat), double.Parse(milesPerDegreeLong), int.Parse(locationReportingInterval));
            var token = tokenSource.Token;
            Task.Factory.StartNew(() => driver.Run(token));
            Task.Factory.StartNew(() => driver.WaitForEnter(tokenSource));
            driver.runCompleteEvent.WaitOne();
        }
    }
}
