using DirectCareConnect.Common.Interfaces.Authentication;
using DirectCareConnect.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Microsoft.Extensions.DependencyInjection;
using DirectCareConnect.Common.Interfaces;
using DirectCareConnect.Common.ComponentModel;
using DirectCareConnect.Common.Models.Rest;
using DirectCareConnect.Common.Models.UI;
using DirectCareConnect.Common.Models.Db.Handlers;
using DirectCareConnect.Common.Impl.Communication.DDezModels;

[assembly: Dependency(typeof(ProxyJobService))]
namespace DirectCareConnect.Services
{
    public class ProxyJobService: IJobService
    {
        private IJobService jobService;

        public ProxyJobService()
        {
            this.jobService = App.Service.GetService<IJobService>();
        }

        public Task<bool> Cancel()
        {
            return this.jobService.Cancel(); 
        }

        public Task<bool> CanStartJob()
        {
            return this.jobService.CanStartJob();
        }

        public Task<bool> ClosePendingDocumentation(int clientId, int clientServiceId, int serviceId)
        {
            return this.jobService.ClosePendingDocumentation(clientId, clientServiceId, serviceId);
        }

        public Task<StartServiceModel> GetEndServiceModel()
        {
            return this.jobService.GetEndServiceModel();
        }

      
        public Task<StartServiceModel> GetStartServiceModel(List<DirectCareConnect.Common.Models.Db.Clients.Location> locations)
        {
            return this.jobService.GetStartServiceModel(locations);
        }

        public Task<StartServiceModel> GetStartServiceModel()
        {
            return this.jobService.GetStartServiceModel();
        }

        public Task<Common.Impl.Communication.DDezModels.SetNoteResponse> SetNote(string restToken, EndSessionRespiteModel model)
        {
            return this.jobService.SetNote(restToken, model);
        }

        public Task<Common.Impl.Communication.DDezModels.SetNoteResponse> SetNote(string restToken, EndSessionHabilitationModel model)
        {
            return this.jobService.SetNote(restToken, model);
        }

        public Task<Common.Impl.Communication.DDezModels.SetNoteResponse> SetNote(string restToken, EndSessionAttendantCareModel model)
        {
            return this.jobService.SetNote(restToken, model);
        }

        public Task<bool> StartServiceSegment(StartServiceSegmentModel model)
        {
            return this.jobService.StartServiceSegment(model);
        }

        public void TriggerModal(StartServiceModel model)
        {
            this.jobService.TriggerModal(model);
        }

        Task<int> IJobService.EndServiceSegment(int clientId, int serviceId)
        {
            return this.jobService.EndServiceSegment(clientId, serviceId);
        }

        public Task<CurrentLocation> GetCurrentLocation()
        {
            return this.jobService.GetCurrentLocation();
        }

        public Task<bool> SetClosestLocations(CurrentLocation currentLocation, List<DirectCareConnect.Common.Models.Db.Clients.Location> closeLocations)
        {
            return this.jobService.SetClosestLocations(currentLocation,closeLocations);
        }

        public Task<List<DirectCareConnect.Common.Models.Db.Clients.Location>> GetClosestLocations()
        {
            return this.jobService.GetClosestLocations();
        }

        public Task<EndSessionRespiteModel> GetEndSessionRespiteModel(string token, int clientId, int clientServiceId, int remoteDocumentId, int internalDocumentId)
        {
            return this.jobService.GetEndSessionRespiteModel(token, clientId, clientServiceId, remoteDocumentId, internalDocumentId);
        }

        public Task<EndSessionHabilitationModel> GetEndSessionHabilitationModel(string token, int clientId, int clientServiceId, int remoteDocumentId, int internalDocumentId)
        {
            return this.jobService.GetEndSessionHabilitationModel(token, clientId, clientServiceId, remoteDocumentId, internalDocumentId);
        }

        public Task<EndSessionAttendantCareModel> GetSessionAttendantCareModel(string token, int clientId, int clientServiceId, int remoteDocumentId, int internalDocumentId)
        {
            return this.jobService.GetSessionAttendantCareModel(token, clientId, clientServiceId, remoteDocumentId, internalDocumentId);
        }

        public Task<EndSessionOccupationalTherapyModel> GetEndSessionOccuptionalTherapyModel(string token, int clientId, int clientServiceId, int remoteDocumentId, int internalDocumentId)
        {
            return this.jobService.GetEndSessionOccuptionalTherapyModel(token, clientId, clientServiceId, remoteDocumentId, internalDocumentId);
        }

        public Task<SetNoteResponse> SetNote(string restToken, EndSessionOccupationalTherapyModel model)
        {
            return this.jobService.SetNote(restToken, model);
        }

        public Task<EndSessionPhysicalTherapyModel> GetEndSessionPhysicalTherapyModel(string token, int clientId, int clientServiceId, int remoteDocumentId, int internalDocumentId)
        {
            return this.jobService.GetEndSessionPhysicalTherapyModel(token, clientId, clientServiceId, remoteDocumentId, internalDocumentId);
        }

        public Task<SetNoteResponse> SetNote(string restToken, EndSessionPhysicalTherapyModel model)
        {
            return this.jobService.SetNote(restToken, model);
        }
    }
}
