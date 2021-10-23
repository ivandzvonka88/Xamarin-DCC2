using DirectCareConnect.Common.Handlers;
using DirectCareConnect.Common.Impl;
using DirectCareConnect.Common.Interfaces;
using DirectCareConnect.Common.Interfaces.Storage;
using DirectCareConnect.Common.Models.Db;
using DirectCareConnect.Common.Models.Db.Clients;
using DirectCareConnect.Common.Models.Rest;
using DirectCareConnect.Common.Models.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using DirectCareConnect.Blazor.Services;
using DirectCareConnect.Common.Models.Db.Notes;
using DirectCareConnect.Common.XPlat;

namespace DirectCareConnect.Blazor.Mocks
{
    public class MockDatabaseService : CommonDatabaseService, IDatabaseService
    {
        private CurrentCredentials currentCredentials;
        private InitialData initialData;
        private List<object> _items;
        IServiceProvider provider;

        public MockDatabaseService(IServiceProvider provider)
        {
            this.provider = provider;
        }

        private List<object> Items
        {
            get{
                if (this._items == null)
                    this._items = new List<object>();

                return this._items;
            }
        }

        async public override Task<int> Add(object item)
        {
            var tt = item.GetType();
            var items = this._items.Where(p => p.GetType().Equals(tt));
            object primaryKeyOfItem = null;

            PropertyInfo[] itemProperties = item.GetType().GetProperties();

            foreach (PropertyInfo property in itemProperties)
            {
                var attribute = Attribute.GetCustomAttribute(property, typeof(SQLite.PrimaryKeyAttribute))
                    as SQLite.PrimaryKeyAttribute;

                if (attribute != null) // This property has a KeyAttribute
                {
                    // Do something, to read from the property:
                    primaryKeyOfItem = property.GetValue(item);
                    var index = items.Count();
                    index++;
                    property.SetValue(item, index);
                }
            }

            

            this.Items.Add(item);
            return await Task.FromResult(1);
        }

        public Task<bool> ClosePendingDocumentation(int cid, int sid, int sEid)
        {
            var item = this.initialData.PendingDocumentation.Where(p => p.ClientId == cid && p.ClientServiceId == sid && p.DocumentId == sEid).FirstOrDefault();
            if (item != null)
                this.initialData.PendingDocumentation.Remove(item);

            return Task.FromResult(true);
        }

        public override Task<List<T>> GetAll<T>()
        {
            try
            {
                var tt = typeof(T);
                if (tt.Equals(typeof(Common.Models.Db.Clients.Client)))
                {
                    return Task.FromResult(this.initialData.Clients.Cast<T>().ToList());
                }

                if (tt.Equals(typeof(ClientService)))
                {
                    return Task.FromResult(this.initialData.ClientServices.Cast<T>().ToList());
                }

                if (tt.Equals(typeof(Common.Models.Db.Clients.Location)))
                {
                    return Task.FromResult(this.initialData.Locations.Cast<T>().ToList());
                }

                if (tt.Equals(typeof(Common.Models.Db.Clients.Designee)))
                {
                    return Task.FromResult(this.initialData.Designees.Cast<T>().ToList());
                }

                if (tt.Equals(typeof(ServiceEntry)))
                {
                    return Task.FromResult(this.Items.Where(p => p.GetType() == typeof(ServiceEntry)).Cast<T>().ToList());
                }
               
                if (tt.Equals(typeof(Common.Models.Db.Company)))
                {
                    var lst = new List<Common.Models.Db.Company>();
                    lst.Add(this.initialData.Company);
                    return Task.FromResult(lst.Cast<T>().ToList());
                }

                if (tt.Equals(typeof(CurrentCredentials)))
                {
                    List<CurrentCredentials> st = new List<CurrentCredentials>();
                    st.Add(this.currentCredentials);

                    return Task.FromResult(st.Cast<T>().ToList());
                }

                if (tt.Name == "Object")
                {
                    return Task.FromResult(this.Items.Cast<T>().ToList());
                }

                return Task.FromResult(this.Items.Where(p => p.GetType().Equals(tt)).Cast<T>().ToList());
            }

            catch
            {
                return Task.FromResult(new List<T>());
            }
        }

        public Task<CurrentCredentials> GetCurrentCredentialsAsync()
        {
            if (this.currentCredentials == null)
            {
                this.currentCredentials = new CurrentCredentials();
            }
            return Task.FromResult(this.currentCredentials);
        }

        async public Task<DashboardModel> GetDashboardModel()
        {
            DashboardModel model = new DashboardModel();
            model.ClientAlerts = this.initialData.ClientAlerts;
            model.Credentials = this.initialData.Credentials;
            model.PendingDocumentations = this.initialData.PendingDocumentation;
            model.SessionTimeInSeconds = await GetSessionTimeInSeconds();
            return model;
        }

      
        async public Task<bool> IsServiceEntryOpen()
        {
            return await Task.FromResult(false);
        }

        async public Task<bool> SaveCurrentCredentialsAsync(CurrentCredentials currentCredentials)
        {
            this.currentCredentials = currentCredentials;
            return await Task.FromResult(true);
        }

        async public Task<bool> SaveInitialData(InitialData initialData)
        {
            this.initialData = initialData;
            try
            {
                var jobService = this.provider.GetService<IJobService>();
                var loggingService = this.provider.GetService<ILoggingService>();
                LocationChangeHandler.GetLocationChangeHandler(jobService, loggingService).UpdateLocations(initialData.Locations);
            }


            catch
            {

            }

            await AddInternalServicesToPendingDocumentation();
            
            return await Task.FromResult(true);

        }

        async public override Task<int> Update(object item)
        {
            ///not sure any of this is necessary since the object is probably changed in memory anyway, but the code is interesting so keeping until refactored
            var tt = item.GetType();
            var items=this._items.Where(p => p.GetType().Equals(tt));
            object primaryKeyOfItem=null;
            
            PropertyInfo[] itemProperties = item.GetType().GetProperties();

            foreach (PropertyInfo property in itemProperties)
            {
                var attribute = Attribute.GetCustomAttribute(property, typeof(SQLite.PrimaryKeyAttribute))
                    as SQLite.PrimaryKeyAttribute;

                if (attribute != null) // This property has a KeyAttribute
                {
                    // Do something, to read from the property:
                    primaryKeyOfItem= property.GetValue(item);
                }
            }

            if (primaryKeyOfItem != null)
            {
                foreach(var prospectiveItem in items)
                {
                    PropertyInfo[] properties = prospectiveItem.GetType().GetProperties();

                    foreach (PropertyInfo property in properties)
                    {
                        var attribute = Attribute.GetCustomAttribute(property, typeof(SQLite.PrimaryKeyAttribute))
                            as SQLite.PrimaryKeyAttribute;

                        if (attribute != null) // This property has a KeyAttribute
                        {
                            // Do something, to read from the property:
                            var val = property.GetValue(item);
                            if (val.Equals(primaryKeyOfItem))
                            {
                                this.Items.Remove(prospectiveItem);
                                this.Items.Add(item);
                                return await Task.FromResult(1);
                            }
                        }
                    }
                }
            }
            return await Task.FromResult(0);
        }

        public Task<bool> UpdateLocationAlerts(List<Common.Models.Db.Clients.Location> locations)
        {
            return Task.FromResult(true);
        }

        public Task<bool> ResetLocationAlerts(List<Common.Models.Db.Clients.Location> locations)
        {
            return Task.FromResult(true);
        }

        async public Task<Common.Models.Db.Company> GetCompany()
        {
            return (await this.GetAll<Common.Models.Db.Company>()).FirstOrDefault();
        }

        async public Task<List<Common.Models.Db.Clients.Designee>> GetDesignees(int clientId)
        {
            return (await this.GetAll<Common.Models.Db.Clients.Designee>()).Where(p => p.ClientId == clientId).ToList();
        }

        async public Task UpdatePendingDocumentation(List<Common.Impl.Communication.DDezModels.PendingDocumentation> pendingDocumentations)
        {
            await Task.Run(()=>this.initialData.PendingDocumentation = ToPendingDocumentations(pendingDocumentations));

        }

        public override Task<int> Delete(object item)
        {
            var tt = item.GetType();
            if (tt.Equals(typeof(CurrentCredentials)))
            {
                this.currentCredentials = null;
            }
            return Task.FromResult(-7);
        }
    }
}
