using DirectCareConnect.Common.ComponentModel;
using DirectCareConnect.Common.Extensions;
using DirectCareConnect.Common.Handlers;
using DirectCareConnect.Common.Interfaces;
using DirectCareConnect.Common.Interfaces.Communication;
using DirectCareConnect.Common.Interfaces.Storage;
using DirectCareConnect.Common.Models.Db;
using DirectCareConnect.Common.Models.Db.Clients;
using DirectCareConnect.Common.Models.Db.Handlers;
using DirectCareConnect.Common.Models.Rest;
using DirectCareConnect.Common.Models.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectCareConnect.Common.Impl
{
    public class JobService : IJobService
    {
        private bool requestPending;
        private CurrentLocation currentLocation;
        protected List<Location> closeLocations;
        async public Task<bool> CanStartJob()
        {
            return !await this.databaseService.IsServiceEntryOpen() && !requestPending;
        }

        IDatabaseService databaseService;
        INotifierService notifierService;
        IRestClient restClient;
        public JobService(IDatabaseService databaseService, INotifierService notifierService, IRestClient restClient)
        {
            this.databaseService = databaseService;
            this.notifierService = notifierService;
            this.restClient = restClient;
            
        }
        
        async public Task<StartServiceModel> GetStartServiceModel(List<Location> locations)
        {

            await this.databaseService.UpdateLocationAlerts(locations);

            StartServiceModel model = new StartServiceModel();

            //for now ignore geofenced clients and send all to the front-end, front end will filter
            //List<int> clientIds = locations.Select(p => p.ClientId).GroupBy(p => p).Select(p=>p.Key).ToList();
            return await GetStartServiceModel();
           
            
        }


        async public Task<StartServiceModel> GetStartServiceModel()
        {
            //List<Location> locations - get locations
            //this.databaseService.UpdateLocationAlerts(locations);
            StartServiceModel model = new StartServiceModel();

            //List<int> clientIds = locations.Select(p => p.ClientId).GroupBy(p => p).Select(p => p.Key).ToList();

            model.AvailalbleClients = (await this.databaseService.GetAll<Client>()).Select(p=>new ClientWithLocation(p)).ToList();
            model.AvailableServices = (await this.databaseService.GetAll<ClientService>()).ToList();
            model.AvailableLocations = (await this.databaseService.GetAll<Location>()).Select(p => new SelectableLocation(p)).ToList();
            var openServices = (await this.databaseService.GetAll<ServiceEntry>()).Where(p => p.EndUTC == null);

            foreach (var service in openServices)
            {
                model.AvailableServices.RemoveAll(p => p.ClientId == service.ClientId && p.ServiceId == service.ServiceId);
            }

            UpdateAvailableLocationsWithGeoFenceInformation(model);
            
            model.AvailableLocations = model.AvailableLocations.Where(p => p.IsInGeofence == true).ToList();
            var test = JsonConvert.SerializeObject(model);
            model.Message = test;
            //model.AvailalbleClients = this.databaseService.AvailalbleClients();
            return model;
        }

        protected virtual void UpdateAvailableLocationsWithGeoFenceInformation(StartServiceModel model)
        {
            if (this.closeLocations != null && model!=null && model.AvailableLocations!=null)
            {
                foreach (var location in model.AvailableLocations)
                {
                    if (this.closeLocations.Where(p => p.ClientLocationId == location.ClientLocationId && p.LocationTypeId==location.LocationTypeId ).Any())
                    {
                        location.IsInGeofence = true;
                        try
                        {
                            model.AvailalbleClients.Where(p => p.ClientId == location.ClientId).FirstOrDefault().IsInGeofence = true;
                        }

                        catch { }
                    }
                }
            }

            model.AvailableLocations = model.AvailableLocations.Where(p => p.IsInGeofence == true).ToList();
        }

        public void TriggerModal(StartServiceModel model)
        {
          requestPending = true;
            var json = JsonConvert.SerializeObject(model);
            
            this.notifierService.OnModalNotification(new ModalEventHandlerEventArgs
            {
                ModalModel =model,
                ModalName = "DirectCareConnect.Blazor.Components.StartService",
                ModalTitle = String.Empty

            });

        }

        async public Task<bool> StartServiceSegment(StartServiceSegmentModel model)
        {

            /* XXXXXXXXXX 
             *  Set Service Entry StartLat and StartLon
             *  Set StartClientLocationId and StartLocationTypeId if within fence.
             * 
             * try send the server if acked set ack = true
             * will need background task  maybe let background task handler all sends then no need to send here  
            */

            Company company = await this.databaseService.GetCompany();
            ServiceEntry serviceEntry = new ServiceEntry {
                ClientId = model.ClientId,
                ClientServiceId = model.ClientServiceId,
                ServiceId = model.ServiceId,
                IsHCBS = model.IsHCBS,
                IsTherapy = model.IsTherapy,
                IsEvaluation = model.IsEvaluation,
                ShortServiceName = model.ShortServiceName,
                StartUTC = DateTime.UtcNow,
                StartAcknowledged = false,
                EndAcknowledged = false,

                // XXXXX hardcoded
                CoId = company.CompanyId, //development company

                ProviderId = company.ProviderId, // assttherapist@thpl.com

                

                // end test 3


                //XXXXXX end hardcoded


            };

            if (model.Location != null)
            {
                serviceEntry.StartLat = Convert.ToDecimal(model.Location.Latitude); // lat for 
                serviceEntry.StartLon = Convert.ToDecimal(model.Location.Longitude); // lon for
                serviceEntry.StartClientLocationId = model.Location.ClientLocationId; // LocationId for 42910 N Outer Bank Ct
                serviceEntry.StartLocationTypeId = model.Location.LocationTypeId; // client home
            }
            else
            {
                if (this.currentLocation != null)
                {
                    serviceEntry.StartLat = Convert.ToDecimal(this.currentLocation.Latitude);
                    serviceEntry.StartLon = Convert.ToDecimal(this.currentLocation.Longitude);
                }

            }

          
            await this.databaseService.Add(serviceEntry);
            
            // end option 2

            this.requestPending = false;

            return await Task.FromResult(true);
        }

        async public Task<bool> Cancel()
        {
            this.requestPending = false;
            //reset the handler , actually this info should be in this service not handler
            return await Task.FromResult(true);
        }

        async public Task<StartServiceModel> GetEndServiceModel()
        {
            
            StartServiceModel model = new StartServiceModel();

            var openServiceEntries = (await this.databaseService.GetAll<ServiceEntry>()).Where(p=>p.EndUTC==null).ToList();

            model.AvailalbleClients = (await this.databaseService.GetAll<Client>()).Where(p => openServiceEntries.Select(x=>x.ClientId).Contains(p.ClientId)).Select(p=>new ClientWithLocation(p)).ToList();
            var availableServices = (await this.databaseService.GetAll<ClientService>()).Where(p => openServiceEntries.Select(x => x.ClientId).Contains(p.ClientId)).ToList();

            model.AvailableServices= (from a in availableServices join b in openServiceEntries on new { a.ClientId, a.ServiceId } equals new { b.ClientId, b.ServiceId} select a).ToList();

            model.AvailableLocations = (await this.databaseService.GetAll<Location>()).Select(p=>new SelectableLocation(p)).ToList();
            //model.AvailalbleClients = this.databaseService.AvailalbleClients();
            return model;
            //return await this.databaseService
        }

        async public Task<int> EndServiceSegment(int clientId, int serviceId)
        {
            var service = (await this.databaseService.GetAll<ServiceEntry>()).Where(p => p.EndUTC == null && p.ClientId == clientId && p.ServiceId == serviceId).FirstOrDefault();
            if (service == null)
                return -1;

            if (this.currentLocation != null)
            {
                service.EndLat = (decimal)this.currentLocation.Latitude;
                service.EndLon = (decimal)this.currentLocation.Longitude;
            }
            else
            {
                service.EndLat = 0;
                service.EndLon = 0;
            }
            if (closeLocations!=null && closeLocations.Count == 0)
            {
                service.EndClientLocationId=0;
            }
            else if (service.StartClientLocationId.HasValue && closeLocations!=null && closeLocations.Select(p => p.ClientLocationId).Contains(service.StartClientLocationId.Value))
            {
                service.EndClientLocationId = service.StartClientLocationId;
                service.EndLocationTypeId = service.StartLocationTypeId; // client home
            }
            else
            {
                //what's the alternative, to prompt again?
                service.EndClientLocationId = closeLocations?.First().ClientLocationId;
                service.EndLocationTypeId = closeLocations?.First().LocationTypeId;
            }
            
            service.EndUTC = DateTime.UtcNow;
            int x = await this.databaseService.Update(service);

            // end option 2

            return x;

        }


        public Task<Communication.DDezModels.SetNoteResponse> SetNote(string restToken, EndSessionRespiteModel model)
        {
            SetNote(model);
            if (model.DocId == 0)
            {
                return Task.FromResult(new Communication.DDezModels.SetNoteResponse
                {
                    Er = new Communication.DDezModels.Er
                    {
                         Code=-1,
                         Msg="Document Id Cannot Be Zero"
                    }
                });
                
            }
            return this.restClient.SetNote(restToken, model);
        }

        public Task<bool> ClosePendingDocumentation(int clientId, int clientServiceId, int serviceId)
        {
            return this.databaseService.ClosePendingDocumentation(clientId, clientServiceId, serviceId);
        }

        public Task<Communication.DDezModels.SetNoteResponse> SetNote(string restToken, EndSessionHabilitationModel model)
        {
            SetNote(model);
            return this.restClient.SetNote(restToken, model);
        }

        private void SetNote(EndSessionModelBase model)
        {
            if (model.FileId != Guid.Empty)
            {
                string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), model.FileId + "-" + model.FileName);
                model.FileBuffer = new byte[model.FileSize];
                using (var str = File.OpenRead(fileName))
                {
                    str.Read(model.FileBuffer, 0, model.FileSize);

                }
            }

        }

        public Task<Communication.DDezModels.SetNoteResponse> SetNote(string restToken, EndSessionAttendantCareModel model)
        {
            SetNote(model);
            return this.restClient.SetNote(restToken, model);
        }

        public Task<Communication.DDezModels.SetNoteResponse> SetNote(string restToken, EndSessionOccupationalTherapyModel model)
        {
            SetNote(model);
            return this.restClient.SetNote(restToken, model);
        }
        public Task<Communication.DDezModels.SetNoteResponse> SetNote(string restToken, EndSessionPhysicalTherapyModel model)
        {
            SetNote(model);
            return this.restClient.SetNote(restToken, model);
        }

        public Task<bool> SendServiceIpdate (string restToken, ServiceEntry model)
        {
           // SetNote(model);
            return this.restClient.SendServiceUpdate(restToken, model);
        }

        public Task<CurrentLocation> GetCurrentLocation()
        {
            return Task.FromResult(this.currentLocation);
        }

        async public Task<bool> SetClosestLocations(CurrentLocation currentLocation, List<Location> closeLocations)
        {
            if(closeLocations==null)
            {
                this.closeLocations = new List<Location>();
            }
            else
            {
                this.closeLocations = closeLocations;
            }

            this.currentLocation = currentLocation;
            var openEntries = await this.databaseService.GetOpenServiceEntries();

            foreach (var entry in openEntries)
            {

            }

            return true;
        }

        public Task<List<Location>> GetClosestLocations()
        {
            return Task.FromResult(this.closeLocations);
        }

        async public Task<EndSessionRespiteModel> GetEndSessionRespiteModel(string token, int clientId, int clientServiceId, int remoteDocumentId, int internalDocumentId)
        {
            JobServiceEndSessionModelRetriever<EndSessionRespiteModel> retrieve = new Impl.JobServiceEndSessionModelRetriever<EndSessionRespiteModel>(this.databaseService, this.restClient);
            return await retrieve.GetEndSessionModel(token, clientId, clientServiceId, remoteDocumentId, internalDocumentId);
            
        }
        async public Task<EndSessionHabilitationModel> GetEndSessionHabilitationModel(string token, int clientId, int clientServiceId, int remoteDocumentId, int internalDocumentId)
        {
            JobServiceEndSessionModelRetriever<EndSessionHabilitationModel> retrieve = new Impl.JobServiceEndSessionModelRetriever<EndSessionHabilitationModel>(this.databaseService, this.restClient);
            var model= await retrieve.GetEndSessionModel(token, clientId, clientServiceId, remoteDocumentId, internalDocumentId);
            return model;
        }
        async public Task<EndSessionAttendantCareModel> GetSessionAttendantCareModel(string token, int clientId, int clientServiceId, int remoteDocumentId, int internalDocumentId)
        {
            JobServiceEndSessionModelRetriever<EndSessionAttendantCareModel> retrieve = new Impl.JobServiceEndSessionModelRetriever<EndSessionAttendantCareModel>(this.databaseService, this.restClient);
            var model = await retrieve.GetEndSessionModel(token, clientId, clientServiceId, remoteDocumentId, internalDocumentId);
            return model;
        }

        async public Task<EndSessionOccupationalTherapyModel> GetEndSessionOccuptionalTherapyModel(string token, int clientId, int clientServiceId, int remoteDocumentId, int internalDocumentId)
        {
            JobServiceEndSessionModelRetriever<EndSessionOccupationalTherapyModel> retrieve = new Impl.JobServiceEndSessionModelRetriever<EndSessionOccupationalTherapyModel>(this.databaseService, this.restClient);
            var model = await retrieve.GetEndSessionModel(token, clientId, clientServiceId, remoteDocumentId, internalDocumentId);
            return model;
        }
        async public Task<EndSessionPhysicalTherapyModel> GetEndSessionPhysicalTherapyModel(string token, int clientId, int clientServiceId, int remoteDocumentId, int internalDocumentId)
        {
            JobServiceEndSessionModelRetriever<EndSessionPhysicalTherapyModel> retrieve = new Impl.JobServiceEndSessionModelRetriever<EndSessionPhysicalTherapyModel>(this.databaseService, this.restClient);
            var model = await retrieve.GetEndSessionModel(token, clientId, clientServiceId, remoteDocumentId, internalDocumentId);
            return model;
        }


    }
}
