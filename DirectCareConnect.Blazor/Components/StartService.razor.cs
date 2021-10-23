using DirectCareConnect.Common.ComponentModel;
using DirectCareConnect.Common.Constants;
using DirectCareConnect.Common.Interfaces;
using DirectCareConnect.Common.Models.Db.Clients;
using DirectCareConnect.Common.Models.Rest;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectCareConnect.Blazor.Components
{
    public class StartServiceBase:ComponentBase, IDisposable
    {

        [Inject] IModalService modalService { get; set; }
        [Inject] IXamarinBridge xamarinBridge { get; set; }
        [Inject] IJobService jobService { get; set; }

        [Parameter]
        public StartServiceModel Model{ get; set; }

        
        private List<ClientWithLocation> allClientsNotFiltered { get; set; }

      
        async public void StartServiceSegment()
        {
            try
            {
                var selected = this.Model.AvailalbleClients.Where(p => p.Checked);
                if (selected.Count() == 0)
                {
                    return;
                }
                
                foreach (var item in selected)
                {
                    var service = this.Model.AvailableServices.Where(p => p.ServiceId == this.Model.SelectedService.ServiceId && p.ClientId == item.ClientId).ToList().FirstOrDefault();
                    await jobService.StartServiceSegment(GetStartServiceSegmentModel(item.ClientId, service.ClientServiceId, service.ServiceId, service.IsHCBS, service.IsTherapy, service.IsEvaluation, service.ServiceShortName, this.Model.SelectedLocation));
                }
                
                modalService.Close(Model);
            }

            catch
            {
                
            }
        }

        


        public void SetLocation(SelectableLocation location=null)
        {
            this.Model.SelectedLocation = location;
            this.Model.CurrentScreen = StartServiceModel.WizardScreen.Service;
            this.Model.ShowAllClients = false;
            StateHasChanged();
        }

        public void SetService(ClientService service)
        {
            this.Model.SelectedService = service;
            this.Model.CurrentScreen = StartServiceModel.WizardScreen.Person;
            if (this.Model.SelectedLocation != null)
            {
                this.Model.ShowAllClients = false;
            }
            else
            {
                this.Model.ShowAllClients = true;
            }
            StateHasChanged();
        }

        public void Dispose()
        {
           //
        }

        public void Cancel()
        {
            if (jobService != null)
            {
                jobService.Cancel();
            }  
            modalService.Close(Messaging.Cancel);
        }



        public void ShowAllClients()
        {
            Model.ShowAllClients = true;
            StateHasChanged();
        }

        private StartServiceSegmentModel GetStartServiceSegmentModel(int clientId, int clientServiceId, int serviceId, bool isHCBS, bool isTherapy, bool isEvaluation, string serviceShortName, Location location)
        {
            return new StartServiceSegmentModel
            {
                ClientId = clientId,
                ClientServiceId = clientServiceId,
                ServiceId = serviceId,
                IsHCBS = isHCBS,
                IsTherapy = isTherapy,
                IsEvaluation = isEvaluation,
                ShortServiceName = serviceShortName,
                Location=location       
            };
        }

        

    }
}
