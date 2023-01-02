using static System.Net.Mime.MediaTypeNames;

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
			await geolocalizza();
		}else{
			await App.Current.MainPage.DisplayAlert("Diritti non presenti", "Premere su OK per fornire i permessi di localizzazione", "OK");

			var richiesti = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

			if (richiesti == PermissionStatus.Granted){
				geolocalizza();

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


	private async Task geolocalizza(){
		var richiesta = new GeolocationRequest(GeolocationAccuracy.Best);
		var datiGeolocalizzazione = await Geolocation.GetLocationAsync(richiesta);

		// Notifica su schermo
		//await App.Current.MainPage.DisplayAlert("GEOLOCALIZZATO",
		//	$"Latitudine: {datiGeolocalizzazione.Latitude} - Longitudine: {datiGeolocalizzazione.Longitude} - Altitudine: {datiGeolocalizzazione.Altitude}" , "OK");
		
		// Visualizzazione sulla label
		Visualizza.Text = datiGeolocalizzazione.ToString();

		// slavataggio dei dati in un file .csv
		
		//SalvaDati(
		//	$"Latitudine: {datiGeolocalizzazione.Latitude} - " +
		//	$"Longitudine: {datiGeolocalizzazione.Longitude} - " +
		//	$"Altitudine: {datiGeolocalizzazione.Altitude} ");
		
		SalvaDati(datiGeolocalizzazione.ToString());
	}

	private void SalvaDati(string text){
        SavePosition salvaPosizioni = new SavePosition();

		var generalPath = FileSystem.Current.AppDataDirectory;
		var path = Path.Combine(generalPath, "position.txt");

		if (salvaPosizioni.verificaEsistenza(path)){
            salvaPosizioni.addLine(path, text);
		}else{
			salvaPosizioni.creaFile(path);
		}
    }
}
