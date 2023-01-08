// code base : https://www.youtube.com/watch?v=xJQXqxkpWTg

using Geolocalizzazione.DataModel;
using Geolocalizzazione.SaveData;

namespace Geolocalizzazione;

public partial class MainPage : ContentPage{

	private PermissionStatus permessi = PermissionStatus.Unknown;

    private readonly SaveDataOnDatabase database;

    public MainPage(){
		InitializeComponent();
		VerificaPermessi();

        database = new SaveDataOnDatabase();
	}

	protected async void VerificaPermessi()
	{
        // Controllo dei permessi
        var location = await Permissions.CheckStatusAsync < Permissions.LocationWhenInUse>();
        var storage = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();

        if(DeviceInfo.Platform == DevicePlatform.Android || DeviceInfo.Platform == DevicePlatform.iOS)
        {
            
            if (location == PermissionStatus.Granted && storage == PermissionStatus.Granted)
            {
                permessi = PermissionStatus.Granted;
            }
            else if (permessi != PermissionStatus.Granted)
            {
                await App.Current.MainPage.DisplayAlert("Richiesti i permessi di scrittura su disco", "Permessi di scrittura sul disco", "OK");
                storage = await Permissions.RequestAsync<Permissions.StorageWrite>();

                await App.Current.MainPage.DisplayAlert("Richiesti i permessi di geolocalizzazione", "Permessi di geolocalizzazione necessari", "OK");
                location = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

                if (storage == PermissionStatus.Granted && location == PermissionStatus.Granted)
                    permessi = PermissionStatus.Granted;
                else
                    VerificaPermessi();
            }
            else
            {
                if (location == PermissionStatus.Granted && (storage == PermissionStatus.Unknown || storage == PermissionStatus.Denied || storage == PermissionStatus.Limited || storage == PermissionStatus.Restricted))
                {
                    await App.Current.MainPage.DisplayAlert("Richiesti i permessi di scrittura su disco", "Permessi di scrittura sul disco", "OK");
                    storage = await Permissions.RequestAsync<Permissions.StorageWrite>();
                    if (storage == PermissionStatus.Granted) permessi = PermissionStatus.Granted;
                }
                else if (storage == PermissionStatus.Granted && (location == PermissionStatus.Unknown || location == PermissionStatus.Denied || location == PermissionStatus.Limited || location == PermissionStatus.Restricted))
                {
                    await App.Current.MainPage.DisplayAlert("Richiesti i permessi di geolocalizzazione", "Permessi di geolocalizzazione necessari", "OK");
                    location = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    if (location == PermissionStatus.Granted) permessi = PermissionStatus.Granted;
                }
            }
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("Dispositivo non supportato", "Dispositivo non supportato", "OK");
        }

    }

    private async void LocalizzazioneBtn_Clicked(object sender, EventArgs e){

		// se i permessi sono garantiti, viene chiamato il metodo per la geolocalizzazione
		if(permessi == PermissionStatus.Granted)
        {
            var esitoGeolocalizzazione = await Geolocalizza();

            // salvataggio sul database

            GeoItem nuovaPosizione = new()
            {
                Data = DateTime.Now,
                Latitudine = esitoGeolocalizzazione.Latitude,
                Longitudine = esitoGeolocalizzazione.Longitude,
                Altitudine = (double) esitoGeolocalizzazione.Altitude
            };

            int esitoSalvataggio = await database.AggiungiGeoItem(nuovaPosizione);
            if(esitoSalvataggio != 0)
            {
                await App.Current.MainPage.DisplayAlert("Inserimento Eseguito", $"Id Generato da SQLite: {nuovaPosizione.Id}", "OK");
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Inserimento FALLITO", "", "OK");
            }

            // salvataggio delle posizioni in un file di testo
            SalvaDati(esitoSalvataggio.ToString());
		}
        else
        {
            await App.Current.MainPage.DisplayAlert("Richiesti i permessi", "Permessi di geolocalizzazione e di scrittura necessari", "OK");
            VerificaPermessi();
        }

    }

	private static async Task<Location> Geolocalizza(){
		var richiesta = new GeolocationRequest(GeolocationAccuracy.Best);
        return await Geolocation.GetLocationAsync(richiesta);
	}

    // salvataggio dati in un file di testo
	private void SalvaDati(string text)
    {
        SavePosition salvaPosizioni = new();

		string path = salvaPosizioni.PathDispositivo();

        // Visualizzazione sulla label
        PathLabel.Text = path;

		// slavataggio della stringa in un file
		if (salvaPosizioni.VerificaEsistenza(path))
			salvaPosizioni.AddLine(path, text);
		else if (!salvaPosizioni.VerificaEsistenza(path))
			salvaPosizioni.CreaFile(path, text);
    }

}
