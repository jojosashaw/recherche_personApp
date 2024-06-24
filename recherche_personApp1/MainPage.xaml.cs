using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Newtonsoft.Json.Linq;

namespace recherche_personApp1
{
    public partial class MainPage : ContentPage
    {
        private readonly HttpClient _httpClient;

        public MainPage()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
        }

        private async void OnRechercherClicked(object sender, EventArgs e)
        {
            string prenom = PrenomEntry.Text;
            if (int.TryParse(AnneeEntry.Text, out int annee) && annee >= 2003 && annee <= 2023)
            {
                string dataset = "244400404_prenoms-enfants-nes-nantes";
                string query = $"&q=enfant_prenom:{prenom}%20AND%20annee:{annee}&rows=0&facet=enfant_prenom&facet=annee";

                string url = $"https://data.nantesmetropole.fr/api/records/1.0/search/?dataset={dataset}{query}";

                try
                {
                    var response = await _httpClient.GetStringAsync(url);
                    var json = JObject.Parse(response);
                    var facets = json["facet_groups"];
                    int totalCount = 0;

                    foreach (var facet in facets)
                    {
                        if (facet["name"].ToString() == "enfant_prenom")
                        {
                            var facetsPrenom = facet["facets"];
                            foreach (var item in facetsPrenom)
                            {
                                if (item["name"].ToString() == prenom)
                                {
                                    totalCount = (int)item["count"];
                                }
                            }
                        }
                    }

                    ResultatLabel.Text = $"Nombre d'enfants nés avec le prénom {prenom} en {annee} : {totalCount}";
                }
                catch (Exception ex)
                {
                    ResultatLabel.Text = $"Erreur : {ex.Message}";
                }
            }
            else
            {
                ResultatLabel.Text = "Veuillez entrer une année valide entre 2003 et 2023.";
            }
        }
    }
}
