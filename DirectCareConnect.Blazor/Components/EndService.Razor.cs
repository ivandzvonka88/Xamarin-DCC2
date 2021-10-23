using DirectCareConnect.Blazor.Pages.Bases;
using DirectCareConnect.Common.ComponentModel;
using DirectCareConnect.Common.Constants;
using DirectCareConnect.Common.Interfaces;
using DirectCareConnect.Common.Models.ComponentCommunication;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectCareConnect.Blazor.Components
{
    public class EndServiceBase: ModelBase, IDisposable
    {
        [Inject] IModalService modalService { get; set; }
        
        [Inject] IJobService jobService { get; set; }

        [Parameter]
        public StartServiceModel Model { get; set; }


        public void Cancel()
        {
            modalService.Close(Messaging.Cancel);
        }

        async public void EndServiceSegment(int clientId, int serviceId, int clientServiceId)
        {
            var sessionId=await jobService.EndServiceSegment(clientId, serviceId);
            modalService.Close(new EndServiceModel { ClientId = clientId, ServiceId = serviceId, SessionId= sessionId, ClientServiceId=clientServiceId });
        }

        async public void EndServiceSegment()
        {
            var selected = this.Model.AvailableSelectableClientServices.Where(p => p.Checked);
            if (selected.Count() == 0)
            {
                return;
            }
            foreach (var item in selected)
            {
                await jobService.EndServiceSegment(item.ClientId, item.ServiceId);
            }
            modalService.Close(Messaging.EndService);
        }

    }
}
