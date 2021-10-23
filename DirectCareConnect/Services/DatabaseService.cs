using BlazorMobile.Common.Helpers;
using DirectCareConnect.Common.Handlers;
using DirectCareConnect.Common.Impl;
using DirectCareConnect.Common.Interfaces;
using DirectCareConnect.Common.Interfaces.Storage;
using DirectCareConnect.Common.Logging;
using DirectCareConnect.Common.Models.Db;
using DirectCareConnect.Common.Models.Db.Clients;
using DirectCareConnect.Common.Models.Rest;
using DirectCareConnect.Common.Models.UI;
using DirectCareConnect.Common.XPlat;
using DirectCareConnect.Model;
using DirectCareConnect.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(DatabaseService))]
namespace DirectCareConnect.Services
{
    public class DatabaseService: CommonDatabaseService, IDatabaseService
    {
        InternalDb internalDb;
        ILoggingService logging;
        public DatabaseService(): this(DependencyService.Get<ILoggingService>())
        {
            
        }

        public DatabaseService(ILoggingService logging)
        {
            this.internalDb = App.Database;
            this.logging = logging;
            this.logging.LogError("DCC", "DB Service called");
        }

        async override public Task<List<T>> GetAll<T>() 
        {
            try
            {
                var items = await this.internalDb.database.Table<T>().ToListAsync();
                return items;
            }

            catch
            {
                return null;
            }
        }

        async public Task<CurrentCredentials> GetCurrentCredentialsAsync()
        {
            try
             {
                await this.logging.LogError("DCC", "DB Service called get creds");
                var exist = await this.internalDb.database.Table<CurrentCredentials>().FirstOrDefaultAsync();
                await this.logging.LogError("DCC", "DB Service called got creds");
                return exist;
            }

            catch
            {
                return null;
            }
        }

        async public Task<bool> IsServiceEntryOpen()
        {
            try
            {
                var exist = await this.internalDb.database.Table<ServiceEntry>().Where(p => p.EndUTC == null).FirstOrDefaultAsync();
                if (exist != null)
                    return true;

                return false;
            }

            catch 
            { 
               // ConsoleHelper.WriteException(ee);
                return false;
            }
            
        }

        async public Task<bool> SaveCurrentCredentialsAsync(CurrentCredentials currentCredentials)
        {
            var exist = await this.internalDb.database.Table<CurrentCredentials>().FirstOrDefaultAsync();
            int res = 0;
            if (exist == null)
            {
                res = await internalDb.database.InsertAsync(currentCredentials);
            }
            else
            {
                exist.Credentials = currentCredentials.Credentials;
                exist.LastRefreshed = currentCredentials.LastRefreshed;
                exist.Token = currentCredentials.Token;
                exist.TokenIssued = currentCredentials.TokenIssued;
                exist.MessagingToken = currentCredentials.MessagingToken;
                exist.Email = currentCredentials.Email;
                res = await internalDb.database.UpdateAsync(exist);
            }
            return true;
        }

        async public Task<bool> SaveInitialData(InitialData initialData)
        {
            try
            {
                if (initialData != null)
                {
                    
                    try
                    {
                        CopyLastAlerts(initialData.Locations);
                        await this.internalDb.database.DropTableAsync<Common.Models.Db.Clients.Location>();
                    }
                    catch { }

                    await this.internalDb.database.CreateTableAsync<Common.Models.Db.Clients.Location>();
                    await this.internalDb.database.InsertAllAsync(initialData.Locations);

                    try
                    {
                        await this.internalDb.database.DropTableAsync<Common.Models.Db.Clients.Client>();
                    }
                    catch { }

                    await this.internalDb.database.CreateTableAsync<Common.Models.Db.Clients.Client>();
                    await this.internalDb.database.InsertAllAsync(initialData.Clients);

                    try
                    {
                        await this.internalDb.database.DropTableAsync<ClientService>();
                    }

                    catch { }
                    await this.internalDb.database.CreateTableAsync<ClientService>();
                    await this.internalDb.database.InsertAllAsync(initialData.ClientServices);

                    try
                    {
                        await this.internalDb.database.DropTableAsync<Common.Models.Db.Clients.ClientAlert>();
                    }

                    catch { }
                    await this.internalDb.database.CreateTableAsync<Common.Models.Db.Clients.ClientAlert>();
                    await this.internalDb.database.InsertAllAsync(initialData.ClientAlerts);

                    try
                    {
                        await this.internalDb.database.DropTableAsync<Common.Models.Db.Clients.Credential>();
                    }

                    catch { }
                    await this.internalDb.database.CreateTableAsync<Common.Models.Db.Clients.Credential>();
                    await this.internalDb.database.InsertAllAsync(initialData.Credentials);

                    try
                    {
                        await this.internalDb.database.DropTableAsync<Common.Models.Db.Clients.PendingDocumentation>();
                    }

                    catch { }


                    await this.internalDb.database.CreateTableAsync<Common.Models.Db.Clients.PendingDocumentation>();
                    await this.internalDb.database.InsertAllAsync(initialData.PendingDocumentation);

                    try
                    {
                        await this.internalDb.database.DropTableAsync<Common.Models.Db.Company>();
                    }

                    catch { }

                    await this.internalDb.database.CreateTableAsync<Common.Models.Db.Company>();
                    await this.internalDb.database.InsertAsync(initialData.Company);


                    try
                    {
                        await this.internalDb.database.DropTableAsync<Common.Models.Db.Clients.Designee>();
                    }

                    catch { }

                    await this.internalDb.database.CreateTableAsync<Common.Models.Db.Clients.Designee>();
                    await this.internalDb.database.InsertAllAsync(initialData.Designees);

                    LocationChangeHandler.GetLocationChangeHandler(Xamarin.Forms.DependencyService.Get<IJobService>(), Xamarin.Forms.DependencyService.Get<ILoggingService>()).UpdateLocations(initialData.Locations);
                }
                try
                {
                    await AddInternalServicesToPendingDocumentation();
                }

                catch
                {

                }
                return true;
                
                
                
            }
            catch
            {
                return false;
            }
        }

        

        async public Task<bool> UpdateLocationAlerts(List<Common.Models.Db.Clients.Location> locations)
         {
           foreach(var location in locations)
            {
                await this.logging.LogInfo("Victor", $"updating location- {location.ClientLocationId}");
                location.LastAlert = DateTime.UtcNow;
                await this.Update(location);
            }

            return true;
        }
        async public Task<bool> ResetLocationAlerts(List<Common.Models.Db.Clients.Location> locations)
        {
            foreach (var location in locations)
            {
                location.LastAlert =null;
                await this.Update(location);
            }

            return true;
        }

        async public Task<DashboardModel> GetDashboardModel()
        {
            DashboardModel model = new DashboardModel();
            model.ClientAlerts= await this.internalDb.database.Table<Common.Models.Db.Clients.ClientAlert>().ToListAsync();
            model.Credentials= await this.internalDb.database.Table<Common.Models.Db.Clients.Credential>().ToListAsync();
            model.PendingDocumentations = await this.internalDb.database.Table<Common.Models.Db.Clients.PendingDocumentation>().ToListAsync();
            model.SessionTimeInSeconds = await GetSessionTimeInSeconds();
            model.WeeklyAvgAllowedInMinutes = await GetWeeklyAvgAllowed();
            return model;
        }

        async private Task<double?> GetWeeklyAvgAllowed()
        {
            var exist = (await(this.GetAll<ServiceEntry>())).Where(p => p.EndUTC == null).OrderBy(p => p.StartUTC).FirstOrDefault();

            if (exist != null)
            {
                var service= (await (this.GetAll<ClientService>())).Where(p => p.ClientServiceId==exist.ClientServiceId).FirstOrDefault();
                if (service != null)
                    return service.WeeklyHours;
            }

            return null;
        }

        async public override Task<int> Add(object item)
        {
            return await this.internalDb.database.InsertAsync(item);
        }

        async public override Task<int> Delete(object item)
        {
            return await this.internalDb.database.DeleteAsync(item);
        }


        async public override Task<int> Update(object item)
        {
            return await this.internalDb.database.UpdateAsync(item);
        }

        async public Task<bool> ClosePendingDocumentation(int cid, int sid, int sEid)
        {
            var item=await this.internalDb.database.Table<Common.Models.Db.Clients.PendingDocumentation>().Where(p => p.ClientId == cid && p.ClientServiceId == sid && p.DocumentId == sEid).FirstOrDefaultAsync();
            if (item != null)
            {
                await this.internalDb.database.DeleteAsync(item);
            }

            var note = await this._GetSavedNote(cid, sid, sEid, sEid);
            if (note != null)
            {
                await this.internalDb.database.DeleteAsync(note);
            }

            return true;

        }

        async public Task<Common.Models.Db.Company> GetCompany()
        {
            try
            {
                var exist = await this.internalDb.database.Table<Common.Models.Db.Company>().FirstOrDefaultAsync();
                return exist;
            }

            catch
            {
                return null;
            }
        }

        async public Task<List<Common.Models.Db.Clients.Designee>> GetDesignees(int clientId)
        {
            return (await this.GetAll<Common.Models.Db.Clients.Designee>()).Where(p => p.ClientId == clientId).ToList();
        }

        async public Task UpdatePendingDocumentation(List<Common.Impl.Communication.DDezModels.PendingDocumentation> pendingDocumentations)
        {
            try
            {
                await this.internalDb.database.DropTableAsync<PendingDocumentation>();
            }

            catch { }


            await this.internalDb.database.CreateTableAsync<PendingDocumentation>();
            await this.internalDb.database.InsertAllAsync(ToPendingDocumentations(pendingDocumentations));
        }
    }
}
