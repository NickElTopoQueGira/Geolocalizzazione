using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geolocalizzazione
{
    internal class SavePosition
    {
        public bool verificaEsistenza(string path)
        {
            if (File.Exists(path))
                return true;
            else
                return false;
        }

        public void creaFile(string path)
        {
            File.Create(path).Close();
        }

        public void addLine(string path, string text)
        {
            StreamWriter sw = new StreamWriter(path, true);
            sw.WriteLine(text);
            sw.Close();
        }
    }
}
