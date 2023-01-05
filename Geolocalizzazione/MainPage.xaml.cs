using System.Diagnostics;
using System.Reflection.Metadata;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.VisualBasic;

namespace Geolocalizzazione;

public partial class MainPage : ContentPage{

	private PermissionStatus permessi = PermissionStatus.Unknown; 

    public MainPage(){
		InitializeComponent();
		VerificaPermessi();
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
			await Geolocalizza();
		}
        else
        {
            await App.Current.MainPage.DisplayAlert("Richiesti i permessi", "Permessi di geolocalizzazione e di scrittura necessari", "OK");
            VerificaPermessi();
        }

    }

	private async Task Geolocalizza(){
		var richiesta = new GeolocationRequest(GeolocationAccuracy.Best);
		var datiGeolocalizzazione = await Geolocation.GetLocationAsync(richiesta);

        string text = $"Latitudine: {datiGeolocalizzazione.Latitude} - Longitudine: {datiGeolocalizzazione.Longitude} - Altitudine: {datiGeolocalizzazione.Altitude}";

        // Notifica su schermo
        //await App.Current.MainPage.DisplayAlert("GEOLOCALIZZATO",
        //	text , "OK");

        // Visualizzazione sulla label
        Visualizza.Text = text;

		// slavataggio dei dati in un file .csv
		SalvaDati(text);
	}

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
