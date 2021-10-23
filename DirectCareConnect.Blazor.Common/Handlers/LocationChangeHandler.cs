using DirectCareConnect.Common.GeoFence;
using DirectCareConnect.Common.Interfaces;
using DirectCareConnect.Common.Interfaces.Storage;
using DirectCareConnect.Common.Models.Db.Clients;
using DirectCareConnect.Common.Models.Db.Handlers;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Linq;
using DirectCareConnect.Common.XPlat;

namespace DirectCareConnect.Common.Handlers
{
    public class LocationChangeHandler
    {
        //to handle location changes from the device, and compare against locations
        //if geofence location matches, check status of current job, if already running ignore
        //if not running, send a notification with information

        private static LocationChangeHandler handler;
        private static Dictionary<int, DateTime> alertsSent;

        private List<Location> locations;
        private GeolocationUtilties util;
        private CurrentLocation currentLocation;

        private IJobService jobService;
        ILoggingService loggingService;

        public static LocationChangeHandler GetLocationChangeHandler(IJobService jobService, ILoggingService loggingService)
        {
            if (handler == null)
                handler = new LocationChangeHandler(jobService, loggingService);

            return handler;
        }
        private LocationChangeHandler(IJobService jobService, ILoggingService loggingService)
        {
            this.util = new GeolocationUtilties();
            this.jobService = jobService;
            this.loggingService = loggingService;

        }

        public void UpdateLocations(List<Location> locations)
        {
            foreach(var location in locations)
            {
                if(location.LastAlert!=null)
                    this.loggingService.LogInfo("Victor", $"updating location in handle- {location.ClientLocationId} - last alert");
            }
            
            this.locations = locations;
        }

        /// <summary>
        /// This gets called from the underlying OS
        /// </summary>
        /// <param name="currentLocation"></param>
        /// <returns></returns>
        async public Task UpdateLocation(CurrentLocation currentLocation)
        {
            //if the locations haven't been set yet, nothing to do.
            if (this.locations == null || currentLocation == null)
                return;

            //this was the same location as last time, no need to repeat anythi again
            if (this.currentLocation != null && this.currentLocation.Latitude == currentLocation.Latitude && this.currentLocation.Longitude == currentLocation.Longitude)
                return;

            this.currentLocation = currentLocation;

            //the issue with the popup seems to be WHY 
            List<Location> closeLocations = new List<Location>();

            foreach (var location in locations)
            {

                var miles = GetMiles(location);
                if (location.RadiusInMiles >= miles)
                {
                    closeLocations.Add(location);
                    //  App.NotifierService?.OnModalNotification(miles.ToString());
                    //return;
                }

            }

            if (closeLocations.Count > 0)
            {

                await this.jobService.SetClosestLocations(this.currentLocation, closeLocations);
                //should send location service, the list of close locations
                if (!await CanStartJob())
                    return;

                if (closeLocations.Any())
                {
                    await TriggerStartJob(closeLocations);
                }


            }
            else
            {
                await this.jobService.SetClosestLocations(this.currentLocation, null);
            }
        }

        async private Task TriggerStartJob(List<Location> closeLocations)
        {
            if (alertsSent == null)
                alertsSent = new Dictionary<int, DateTime>();

            bool newAlert = false;
            foreach(var location in closeLocations)
            {
                if (!alertsSent.ContainsKey(location.ClientLocationId))
                {
                    alertsSent.Add(location.ClientLocationId, DateTime.UtcNow);
                    newAlert = true;
                    await this.loggingService.LogInfo("Victor", $"will pop job condition 1");
                }
                else if(alertsSent[location.ClientLocationId]<DateTime.UtcNow.AddMinutes(-30))
                {
                    newAlert = true;
                    alertsSent[location.ClientLocationId] = DateTime.UtcNow;
                    await this.loggingService.LogInfo("Victor", $"will pop job condition 2");
                }
                else
                {
                    alertsSent[location.ClientLocationId] = DateTime.UtcNow;
                    await this.loggingService.LogInfo("Victor", $"will NOT pop job condition 3");
                }

            }

            if (newAlert)
            {
                var model = await this.jobService.GetStartServiceModel(closeLocations);
                this.jobService.TriggerModal(model);
            }

        }

        async private Task<bool> CanStartJob()
        {
            return await this.jobService.CanStartJob();
        }

        private double GetMiles(Location location)
        {
            return util.Distance(new Position
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude
            },

                new Position
                {
                    Latitude = currentLocation.Latitude,
                    Longitude = currentLocation.Longitude
                }, DistanceType.Miles
            );
        }
    }
}
