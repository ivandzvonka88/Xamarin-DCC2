using DirectCareConnect.Common.Extensions;
using DirectCareConnect.Common.Interfaces.Communication;
using DirectCareConnect.Common.Interfaces.Storage;
using DirectCareConnect.Common.Models.Db;
using DirectCareConnect.Common.Models.Rest;
using DirectCareConnect.Common.Models.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectCareConnect.Common.Impl
{
    internal class JobServiceEndSessionModelRetriever<T> where T: EndSessionModelBase, new()
    {
        private readonly IDatabaseService databaseService;
        private readonly IRestClient restClient;
        public JobServiceEndSessionModelRetriever(IDatabaseService databaseService, IRestClient restClient)
        {
            this.databaseService = databaseService;
            this.restClient = restClient;
        }
        async public Task<T> GetEndSessionModel(string token, int clientId, int clientServiceId, int remoteDocumentId, int internalDocumentId)
        {
            bool RemoteServiceEntryIdWasZero = false;
            int existingServiceEntryId = 0;
            if (remoteDocumentId == 0)
            {
                var serviceEntry = (await this.databaseService.GetAll<ServiceEntry>()).Where(p => p.ClientId == clientId && p.ClientServiceId == clientServiceId && p.EndUTC != null).OrderByDescending(p => p.ServiceEntryId).FirstOrDefault();
                if (serviceEntry != null)
                {
                    if (serviceEntry.RemoteServiceEntryId == 0)
                    {
                        if (internalDocumentId > 0)
                        {
                            var savedModel = await this.databaseService.GetSavedNote(clientId, clientServiceId, internalDocumentId, remoteDocumentId);
                            if (savedModel != null)
                            {
                                var endsessionModel = JsonConvert.DeserializeObject<T>(savedModel);
                                if (endsessionModel.SelectedDesignee != String.Empty)
                                    endsessionModel.IsDesigneeLocked = true;

                                endsessionModel.DocId = internalDocumentId;
                                endsessionModel.RemoteDocId = remoteDocumentId;
                                return endsessionModel;
                            }
                        }
                        RemoteServiceEntryIdWasZero = true;
                        //cannot get from rest client
                        //cannot get from saved either
                        //HAVE to create this database record
                        //model HAS to have DocId of the service entry id HAVE RemoteServiceEntryId 0
                        existingServiceEntryId = serviceEntry.ServiceEntryId;
                    }
                    else
                    {
                        remoteDocumentId = serviceEntry.RemoteServiceEntryId;
                        //model HAS to HAVE RemoteServiceEntryId=serviceEntry.RemoteServiceEntryId
                    }
                }
                else
                {
                    //no idea what to do here?
                }
            }
            else
            {
                //THIS HAS TO BE THE RemoteServiceEntryId (how else would we get here)
                var savedModel = await this.databaseService.GetSavedNote(clientId, clientServiceId, internalDocumentId, remoteDocumentId);
                if (savedModel != null)
                {
                    var endsessionModel = JsonConvert.DeserializeObject<T>(savedModel);
                    if (endsessionModel.SelectedDesignee != String.Empty)
                        endsessionModel.IsDesigneeLocked = true;

                    endsessionModel.DocId = remoteDocumentId;
                    endsessionModel.RemoteDocId = remoteDocumentId;
                    return endsessionModel;
                }
            }

            T model = new T();

            var notes = await this.GetNote(token, clientId, clientServiceId, remoteDocumentId,GetDocType(model));
            if (notes.designeeId > 0)
            {
                notes.designeedLocked = true;
            }

            SetCommonFields(model, notes);

            if (RemoteServiceEntryIdWasZero)
            {
                model.DocId = existingServiceEntryId;
                model.RemoteDocId = 0;
            }
            else
            {
                model.DocId = remoteDocumentId;
                model.RemoteDocId = remoteDocumentId;
            }



            return model;
        }

        private string GetDocType(EndSessionModelBase model)
        {
            if (model is EndSessionHabilitationModel)
            {
                return DirectCareConnect.Common.Constants.Notes.HAHServiceNote;
            }
            else if (model is EndSessionAttendantCareModel)
            {
                return DirectCareConnect.Common.Constants.Notes.ATCServiceNote;
            }

            else if (model is EndSessionRespiteModel)
            {
                return DirectCareConnect.Common.Constants.Notes.RSPServiceNote;
            }

            else if (model is EndSessionTherapyModel)
            {
                return DirectCareConnect.Common.Constants.Notes.TherapyServiceNote;
            }

            return String.Empty;
            
        }

        private void SetCommonFields(EndSessionModelBase endSessionModelBase, ClientNote notes)
        {
            endSessionModelBase.ClientName = notes.clientName;
            endSessionModelBase.Note = notes.note;
            endSessionModelBase.NoShow = notes.noShow;
            endSessionModelBase.AttachmentName = notes.attachmentName;
            endSessionModelBase.ProviderId = notes.providerId;
            endSessionModelBase.CompanyId = int.Parse(notes.coId);
            endSessionModelBase.Designees = notes.designees;
            endSessionModelBase.IsDesigneeLocked = notes.designeedLocked;
            endSessionModelBase.SelectedDesignee = notes.designeeId + "-" + notes.guardianId;

            DateTime dt;
            if (DateTime.TryParse(notes.dt, out dt))
                endSessionModelBase.NoteDate = dt;


            if(endSessionModelBase is EndSessionHabilitationModel)
            {
                ((EndSessionHabilitationModel)endSessionModelBase).LongTermObjectives = notes.longTermObjectives;
                ((EndSessionHabilitationModel)endSessionModelBase).Scoring = notes.scoring;
            }
            else if (endSessionModelBase is EndSessionTherapyModel)
            {
                ((EndSessionTherapyModel)endSessionModelBase).LongTermObjectives = notes.longTermObjectives;
                ((EndSessionTherapyModel)endSessionModelBase).Scoring = notes.scoring;
            }
            else if(endSessionModelBase is EndSessionAttendantCareModel)
            {
                ((EndSessionAttendantCareModel)endSessionModelBase).CareAreas = notes.careAreas;
                ((EndSessionAttendantCareModel)endSessionModelBase).Scoring = notes.scoring;
            }
        }

        async public Task<ClientNote> GetNote(string token, int clientId, int serviceId, int documentId, string docType)
        {
            try
            {
                if (token.IsNullOrEmpty())
                {
                    token = (await this.databaseService.GetCurrentCredentialsAsync()).Token;
                }

                ClientNote note = null;
                if (documentId > 0)
                {
                    var companyId = (await this.databaseService.GetCompany()).CompanyId;
                    try
                    {
                        note = await this.restClient.GetNote(token, companyId.ToString(), documentId.ToString(), docType);
                    }

                    catch { }

                    if (note == null || note.clientId == 0)
                    {
                        note = await this.databaseService.GetNote(clientId, serviceId);
                        if (note != null)
                        {
                            note.docId = documentId;
                            note.dt = DateTime.UtcNow.ToString();
                        }
                    }
                    else
                    {
                        note.remoteDocId = note.docId;
                    }

                }
                else
                {
                    note = await this.databaseService.GetNote(clientId, serviceId);
                }
                note.designees = await this.databaseService.GetDesignees(note.clientId);
                return note;
            }

            catch
            {
                return await Task.FromResult(new ClientNote());
            }
        }
    }
}
