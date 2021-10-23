using DirectCareConnect.Common.Models.Db;
using DirectCareConnect.Common.Models.Db.Clients;
using DirectCareConnect.Common.Models.Db.Handlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.ComponentModel
{
    public class StartServiceModel
    {
        private List<ClientService> availableServices { get; set; }
        private List<SelectableClientServices> availableSelectableClientServices;

        public List<ClientService> AvailableServices { 
            get
            {
                return this.availableServices;
            }
            set
            {
                this.availableServices = value;
                this.availableSelectableClientServices = new List<SelectableClientServices>();
                foreach(var service in this.availableServices)
                {
                    this.availableSelectableClientServices.Add(new SelectableClientServices(service));
                }
            }
        }
        public List<SelectableClientServices> AvailableSelectableClientServices { 
            get
            {
                return this.availableSelectableClientServices;
            }
        }
        public List<ClientWithLocation> AvailalbleClients { get; set; }
        public List<SelectableLocation> AvailableLocations { get; set; }
        public string Message { get; set; }
        public bool ClientFiltered { get; set; }
        public bool ShowAllClients { get; set; }
        public CurrentLocation CurrentLocation { get; set; }

        public WizardScreen CurrentScreen { get; set; }

        public SelectableLocation SelectedLocation { get; set; }
        public ClientService SelectedService { get; set; }


        public enum WizardScreen
        {
            Location, Service, Person
        }



    }
}
