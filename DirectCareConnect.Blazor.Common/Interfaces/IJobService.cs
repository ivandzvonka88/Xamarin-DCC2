using BlazorMobile.Common.Attributes;
using DirectCareConnect.Common.ComponentModel;
using DirectCareConnect.Common.Handlers;
using DirectCareConnect.Common.Impl.Communication.DDezModels;
using DirectCareConnect.Common.Models.Db.Handlers;
using DirectCareConnect.Common.Models.Rest;
using DirectCareConnect.Common.Models.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DirectCareConnect.Common.Interfaces
{
    [ProxyInterface]
    public interface IJobService
    {
        Task<bool> CanStartJob();   
        Task<StartServiceModel> GetStartServiceModel(List<DirectCareConnect.Common.Models.Db.Clients.Location> locations);
        Task<StartServiceModel> GetStartServiceModel();
        void TriggerModal(StartServiceModel model);

        Task<bool> StartServiceSegment(StartServiceSegmentModel model);
        Task<bool> Cancel();
        Task<StartServiceModel> GetEndServiceModel();
        Task<EndSessionPhysicalTherapyModel> GetEndSessionPhysicalTherapyModel(string restToken, int clientId, int clientServiceId, int remoteDocumentId, int internalDocId);
        Task<int> EndServiceSegment(int clientId, int serviceId);
        Task<EndSessionOccupationalTherapyModel> GetEndSessionOccuptionalTherapyModel(string restToken, int clientId, int clientServiceId, int remoteDocumentId, int internalDocId);
        Task<EndSessionHabilitationModel> GetEndSessionHabilitationModel(string token, int clientId, int clientServiceId, int remoteDocumentId, int internalDocumentId);
        Task<EndSessionRespiteModel> GetEndSessionRespiteModel(string token, int clientId, int clientServiceId, int remoteDocumentId, int internalDocumentId);

        Task<DirectCareConnect.Common.Impl.Communication.DDezModels.SetNoteResponse> SetNote(string restToken, EndSessionRespiteModel model);
        Task<DirectCareConnect.Common.Impl.Communication.DDezModels.SetNoteResponse> SetNote(string restToken, EndSessionHabilitationModel model);
        Task<EndSessionAttendantCareModel> GetSessionAttendantCareModel(string token, int clientId, int clientServiceId, int remoteDocumentId, int internalDocumentId);
        Task<DirectCareConnect.Common.Impl.Communication.DDezModels.SetNoteResponse> SetNote(string restToken, EndSessionAttendantCareModel model);

        Task<bool> ClosePendingDocumentation(int clientId, int clientServiceId, int serviceId);
        Task<SetNoteResponse> SetNote(string restToken, EndSessionOccupationalTherapyModel model);
        Task<SetNoteResponse> SetNote(string restToken, EndSessionPhysicalTherapyModel model);
        Task<CurrentLocation> GetCurrentLocation();
        Task<List<DirectCareConnect.Common.Models.Db.Clients.Location>> GetClosestLocations();
        Task<bool> SetClosestLocations(CurrentLocation currentLocation,List<DirectCareConnect.Common.Models.Db.Clients.Location> closeLocations);
    }
}
