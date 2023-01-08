using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geolocalizzazione.DataModel
{
    internal class GeoItem
    {
        [PrimaryKey, AutoIncrement]

        public int Id { get; set; }
        public DateTime Data { get; set; }
        //public string Descrizione { get; set; }
        public double Latitudine { get; set; }
        public double Longitudine { get; set; }
        public double Altitudine { get; set; }

    }
}
