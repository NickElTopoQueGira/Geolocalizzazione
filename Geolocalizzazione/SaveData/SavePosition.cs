using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * !INFO!
 * 
 * Questa classe, si occupa del salvataggio offline dei dati
 * i dati vengono salvati in un file sotta la cartella documenti, se si tratta di un dispositivo Andoid
 * se si usa un dipositivo iOS, i dati vengono salvati nella cartella appdata dell'app
 *
 */

namespace Geolocalizzazione
{
    public class SavePosition
    {
        public string PathDispositivo()
        {
            string path;
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                // per l'accesso da parte dell'utente alla cartella appdata dell'applicazione che si trova sotto
                // /data/user/0/com.nick.geolocalizzazione/files/
                // sono richiesti i privilegi di root

                // andorid document folder
                path = @"/sdcard/Documents/position.txt";
            }
            else if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                path = Path.Combine(
                    FileSystem.Current.AppDataDirectory,
                    "position.txt");
            }
            else
            {
                path = null;
            }

            return path;
        }

        public bool VerificaEsistenza(string path)
        {
            try
            {
                if (File.Exists(path))
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                App.Current.MainPage.DisplayAlert("Errore", e.ToString(), "OK");
                return false;
            }
        }

        public void CreaFile(string path,string text)
        {
            try
            {
                File.Create(path).Close();
                AddLine(path, text);
            }
            catch(Exception e)
            {
                App.Current.MainPage.DisplayAlert("Errore", e.ToString() , "OK");
            }
            
        }

        public void AddLine(string path, string text)
        {            
            try
            {
                StreamWriter sw = new (path, true);
                sw.WriteLine(text);
                sw.Close();

            }
            catch (Exception e)
            {
                App.Current.MainPage.DisplayAlert("Errore", e.ToString(), "OK");
            }
        }
    }
}
