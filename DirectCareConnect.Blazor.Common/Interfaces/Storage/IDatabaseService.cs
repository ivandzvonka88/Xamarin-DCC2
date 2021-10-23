using BlazorMobile.Common.Attributes;
using DirectCareConnect.Common.Logging;
using DirectCareConnect.Common.Models.Db;
using DirectCareConnect.Common.Models.Db.Clients;
using DirectCareConnect.Common.Models.Rest;
using DirectCareConnect.Common.Models.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DirectCareConnect.Common.Interfaces.Storage
{
    [ProxyInterface]
    public interface IDatabaseService
    {
        Task<CurrentCredentials> GetCurrentCredentialsAsync();
        Task<bool> SaveCurrentCredentialsAsync(CurrentCredentials currentCredentials);
        Task<bool> SaveInitialData(InitialData initialData);
        Task<bool> Message(string message);
        Task<string> Message();
        Task<bool> IsServiceEntryOpen();
        Task<List<T>> GetAll<T>() where T : new();
        Task<bool> UpdateLocationAlerts(List<Models.Db.Clients.Location> locations);
        Task<bool> ResetLocationAlerts(List<Models.Db.Clients.Location> locations);
        Task<DashboardModel> GetDashboardModel();
       // EndSessionRespiteModel GetRespiteNotes(string cid, string sid, string sEid);
        Task<int> Add(object item);
        Task<int> Update(object item);
        Task<bool> ClosePendingDocumentation(int cid, int sid, int sEid);
        Task<List<ServiceEntry>> GetOpenServiceEntries();
        Task<Models.Db.Company> GetCompany();
        Task<List<Models.Db.Clients.Designee>> GetDesignees(int clientId);
        Task<ClientNote> GetNote(int clientId, int serviceId);
        Task<string> GetSavedNote(int clientId, int serviceId, int internalDocumentId, int externalServiceEntryId);
        Task<bool> SaveDirtyNote(int clientId, int serviceId, int serviceEntryId, int externalServiceEntryId, string noteJson);
        Task<bool> UpdateSavedServiceEntryNotes(int serviceEntryId, int externalServiceEntryId);
        Task UpdatePendingDocumentation(List<Impl.Communication.DDezModels.PendingDocumentation> pendingDocumentations);
        Task ClearCredentials();
    }
}
