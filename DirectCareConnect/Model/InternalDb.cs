using DirectCareConnect.Common.Models.Db;
using DirectCareConnect.Common.Models.Db.Notes;
using SQLite;
namespace DirectCareConnect.Model
{
    public class InternalDb
    {
        public readonly SQLiteAsyncConnection database;
        public InternalDb(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<CurrentCredentials>().Wait();
            database.CreateTableAsync<ServiceEntry>().Wait();
            database.CreateTableAsync<SavedNote>().Wait();
        }

    }
}
