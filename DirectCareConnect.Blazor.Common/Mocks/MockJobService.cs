using DirectCareConnect.Common.ComponentModel;
using DirectCareConnect.Common.Impl;
using DirectCareConnect.Common.Interfaces;
using DirectCareConnect.Common.Interfaces.Communication;
using DirectCareConnect.Common.Interfaces.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DirectCareConnect.Common.Mocks
{
    public class MockJobService: JobService, IJobService
    {
        private List<int> locationsInGeoFence;

        public MockJobService(IDatabaseService databaseService, INotifierService notifierService, IRestClient restClient): base(databaseService, notifierService, restClient)
        {
            var a = this.GetType().Assembly;
            using (Stream stream = a.GetManifestResourceStream("DirectCareConnect.Common.Resources.MockedData.json"))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();
                    MockedJson mocked = JsonConvert.DeserializeObject<MockedJson>(json);
                    this.locationsInGeoFence = mocked.LocationsInGeoFence;
                }

            }
        }

        protected override void UpdateAvailableLocationsWithGeoFenceInformation(StartServiceModel model)
        {
            if (this.closeLocations != null)
            {
                foreach (var location in model.AvailableLocations)
                {
                    if (this.closeLocations.Where(p => p.ClientLocationId == location.ClientLocationId && p.LocationTypeId == location.LocationTypeId).Any())
                    {
                        location.IsInGeofence = true;
                        var client = model.AvailalbleClients.Where(p => p.ClientId == location.ClientId).FirstOrDefault();
                        if (client != null)
                            client.IsInGeofence = true;
                    }
                }
            }
            model.AvailableLocations = model.AvailableLocations.Where(p => p.IsInGeofence == true).ToList();
        }


        private class MockedJson
        {
            public List<int> LocationsInGeoFence { get; set; }
        }
    }
}
