using System.Diagnostics;
using System.Reflection.Metadata;
using Microsoft.Maui.Controls.PlatformConfiguration;

namespace Geolocalizzazione;

public partial class MainPage : ContentPage{

	public MainPage(){
		InitializeComponent();
	}
	
    private async void LocalizzazioneBtn_Clicked(object sender, EventArgs e){
		// Controllo dei permessi
		var permessi = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

		// se i permessi sono garantiti, viene chiamato il metodo per la geolocalizzazione
		if(permessi == PermissionStatus.Granted){
			await Geolocalizza();
		}else{
			await App.Current.MainPage.DisplayAlert("Diritti non presenti", "Premere su OK per fornire i permessi di localizzazione", "OK");

			var richiesti = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

			if (richiesti == PermissionStatus.Granted){
				await Geolocalizza();

			}else{
				if (DeviceInfo.Platform == DevicePlatform.Android){
					await App.Current.MainPage.DisplayAlert("Richiesti i permessi di geolocalizzazione", "Permessi di geolocalizzazione necessari", "OK");
				}else if (DeviceInfo.Platform == DevicePlatform.iOS){
					await App.Current.MainPage.DisplayAlert("Richiesti i permessi di geolocalizzazione", "Permessi di geolocalizzazione necessari", "OK");
				}else{
					await App.Current.MainPage.DisplayAlert("Dispositivo non supportato", "Dispositivo non supportato", "OK");
				}
			}
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

	private void SalvaDati(string text){
        SavePosition salvaPosizioni = new();

		var path = "";

		if(DeviceInfo.Platform == DevicePlatform.Android){
            // per l'accesso da parte dell'utente alla cartella appdata dell'applicazione che si trova sotto
            // /data/user/0/com.nick.geolocalizzazione/files/
            // sono richiesti i privilegi di root

            // andorid document folder
            path = @"/sdcard/Documents/position.txt";
		}else if(DeviceInfo.Platform == DevicePlatform.iOS){
			path = Path.Combine(
				FileSystem.Current.AppDataDirectory, 
				"position.txt");
		}

		// Visualizzazione sulla label
		PathLabel.Text = path;

		// slavataggio della stringa in un file
		if(salvaPosizioni.verificaEsistenza(path)){
            salvaPosizioni.addLine(path, text);
		}else{
			salvaPosizioni.creaFile(path);
		}
    }
}
