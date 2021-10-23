using DirectCareConnect.Common.Models.Db;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using DirectCareConnect.Common.Models.Rest;
using DirectCareConnect.Common.Models.Db.Clients;
using Newtonsoft.Json;
using DirectCareConnect.Common.Models.Db.Notes;
using System.Runtime.CompilerServices;

namespace DirectCareConnect.Common.Impl
{
    public class CommonDatabaseService : BaseService
    {
        public virtual Task<List<T>> GetAll<T>() where T : new()
        {
            throw new NotImplementedException("Method must be overriden");
        }


        async protected Task<double?> GetSessionTimeInSeconds()
        {
            var exist = (await (this.GetAll<ServiceEntry>())).Where(p => p.EndUTC == null).OrderBy(p => p.StartUTC).FirstOrDefault();

            if (exist != null)
            {
                var diff = DateTime.UtcNow - exist.StartUTC;
                return diff.TotalSeconds;
            }

            return null;
        }

        async public Task<List<ServiceEntry>> GetOpenServiceEntries()
        {
            return (await this.GetAll<ServiceEntry>()).Where(p => p.EndUTC == null).ToList();
        }

        async public Task<ClientNote> GetNote(int clientId, int serviceId)
        {
            //TODO is service id being sent here from new session? don't remove breakpoint
            var service = (await this.GetAll<ClientService>()).Where(p => p.ClientId == clientId && p.ClientServiceId == serviceId).FirstOrDefault();
            if (service != null)
            {
                var note = JsonConvert.DeserializeObject<ClientNote>(service.CleanNote);
                if (note.docId == 0)
                {
                    //TODO is service id being sent here from new session? don't remove breakpoint
                    var serviceEntry = (await this.GetAll<ServiceEntry>()).Where(p => p.ClientId == clientId && p.ClientServiceId == serviceId && p.EndUTC != null).FirstOrDefault();
                    if(serviceEntry!=null)
                        note.docId = serviceEntry.RemoteServiceEntryId;
                }
                return note;
            }
            return null;
            //return 
        }
        async public Task<string> GetSavedNote(int clientId, int serviceId, int internalDocumentId, int externalServiceEntryId)
        {
            //check for existing notes
            var savedNote = await _GetSavedNote(clientId, serviceId, internalDocumentId, externalServiceEntryId);
            if (savedNote != null)
            {
                return savedNote.JsonNote;
                //
            }

            return null;
            //return 
        }

        async public Task<bool> UpdateSavedServiceEntryNotes(int serviceEntryId, int externalServiceEntryId)
        {
            var note = (await this.GetAll<SavedNote>()).Where(p => p.InternalServiceEntryId == serviceEntryId).FirstOrDefault();
            if (note != null && note.ExternalServiceEntryId == 0)
            {
                note.ExternalServiceEntryId = externalServiceEntryId;
                await this.Update(note);
            }
            return true;
        }

        async public Task<bool> SaveDirtyNote(int clientId, int serviceId, int serviceEntryId, int externalServiceEntryId, string noteJson)
        {
            var note = await _GetSavedNote(clientId, serviceId, serviceEntryId, externalServiceEntryId);

            if (note == null)
            {
                note = new SavedNote
                {
                    ClientId = clientId,
                    ServiceId = serviceId,
                    ExternalServiceEntryId = externalServiceEntryId,
                    InternalServiceEntryId = serviceEntryId,
                    JsonNote = noteJson
                };

                await this.Add(note);

            }

            else
            {
                note.JsonNote = noteJson;
                await this.Update(note);
            }
            return true;
        }

        async protected Task<SavedNote> _GetSavedNote(int clientId, int serviceId, int internalDocumentId, int externalServiceEntryId)
        {
            return (await this.GetAll<SavedNote>()).Where(p => p.ClientId == clientId && p.ServiceId == serviceId && (
                (p.InternalServiceEntryId == internalDocumentId && internalDocumentId > 0) || (p.ExternalServiceEntryId == externalServiceEntryId && externalServiceEntryId > 0)
            )).FirstOrDefault();
        }

        public virtual Task<int> Update(object item)
        {
            throw new Exception("This must be overridden");

        }

        public virtual Task<int> Add(object item)
        {
            throw new Exception("This must be overridden");
        }

        public virtual Task<int> Delete(object item)
        {
            throw new Exception("This must be overridden");
        }

        protected List<PendingDocumentation> ToPendingDocumentations(List<Common.Impl.Communication.DDezModels.PendingDocumentation> pendingDocumentations)
        {
            return pendingDocumentations.Select(p => new PendingDocumentation
            {
                ClientId = p.ClientId,
                ClientServiceId = p.ClientServiceId,
                DocumentId = int.Parse(p.DocId),
                Alert = p.ClientName + " - " + p.Svc,
                Svc = p.Svc.ToString()
            }
            ).ToList();
        }

        async protected Task AddInternalServicesToPendingDocumentation()
        {


            var openServicesOnDevice = (await this.GetAll<ServiceEntry>()).Where(p => p.RemoteServiceEntryId == 0);
            foreach (var openServiceOnDevice in openServicesOnDevice)
            {
                var exist = (await this.GetAll<PendingDocumentation>()).Where(p => p.InternalDocumentId == openServiceOnDevice.ServiceEntryId && p.DocumentId == 0).FirstOrDefault();
                if (exist == null)
                {
                    var client = (await this.GetAll<Client>()).Where(p => p.ClientId == openServiceOnDevice.ClientId).First();
                    var service = (await this.GetAll<ClientService>()).Where(p => p.ClientServiceId == openServiceOnDevice.ClientServiceId).First();
                    await this.Add(new PendingDocumentation
                    {
                        Alert = client.FirstName + " " + client.LastName + " - " + service.ServiceName +  " Internal",
                        ClientId = openServiceOnDevice.ClientId,
                        ClientServiceId = openServiceOnDevice.ClientServiceId,
                        DocumentId = 0,
                        InternalDocumentId = openServiceOnDevice.ServiceEntryId,
                        Svc = service.ServiceName
                    });
                }
            }
        }


        async public Task ClearCredentials()
        {
            var exist = await this.GetAll<CurrentCredentials>();
            foreach(var cred in exist)
            {
                await this.Delete(cred);   
            }

        }

        async public void CopyLastAlerts(List<Location> locations)
        {
            try
            {
                var existings = await this.GetAll<Location>();
                foreach (var location in locations)
                {
                    var existing = existings.Where(p => p.ClientId == location.ClientId && p.ClientLocationId == location.ClientLocationId).FirstOrDefault();
                    if (existing != null)
                    {
                        location.LastAlert = existing.LastAlert;
                    }
                }
            }

            catch { }
        }
    }
}
