using Geolocalizzazione.DataModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geolocalizzazione.SaveData
{

    internal class SaveDataOnDatabase
    {
        private readonly SQLiteAsyncConnection connessione;

        public SaveDataOnDatabase()
        {
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "posizioni.db");

            var dbStringaConnessione = new SQLiteConnectionString(databasePath);
            connessione = new SQLiteAsyncConnection(dbStringaConnessione);

            var risposta = InizializzaDB();
        }

        private async Task InizializzaDB()
        {
            await connessione.CreateTableAsync<GeoItem>();
        }

        public async Task<int> AggiungiGeoItem(GeoItem item)
        {
            return await connessione.InsertAsync(item);
        }

        public async Task<List<GeoItem>> LeggiGeoItem()
        {
            return await connessione.Table<GeoItem>().ToListAsync();
        }
    }
}
