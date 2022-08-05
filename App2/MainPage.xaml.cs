using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net.Http;
using MonkeyCache.FileStore;
namespace App2
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            Barrel.ApplicationId = "myAPP";

        }

        private async void button1_Clicked(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();
            string result;
            string query = "https://venus.sod.asu.edu/WSRepository/Services/NumberGuessRest/Service.svc/GetSecretNumber?lower=" + lower.Text + "&upper=" + upper.Text;
            if (!Barrel.Current.IsExpired(query))
            {
                string s = Barrel.Current.Get<string>(query);
                Number.Text =  s;
                numberInfo.Text = "number retrieved from cach ";
            }
            else {
                try
                {
                    var response = await client.GetAsync(query);
                    response.EnsureSuccessStatusCode();
                    result = (await response.Content.ReadAsStringAsync()).Replace(@"""", "");
                }
                catch (HttpRequestException ex)
                {
                    result = ex.ToString();
                }
                Number.Text =result;
                numberInfo.Text = "number retrieved after new query was made  ";

                Barrel.Current.Add(query, result, expireIn: TimeSpan.FromSeconds(10));
            }
            
        }

        private async void button2_Clicked(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();
            string nValue = Number.Text;
            string query = "https://venus.sod.asu.edu/WSRepository/Services/EncryptionRest/Service.svc/Encrypt?text=" + nValue;
            string result;
            if (!Barrel.Current.IsExpired(query))
            {
                string s = Barrel.Current.Get<string>(query);
                Encrypted.Text = s;
                encryptedInfo.Text = "number encrypted retrieved from cach ";
            }
            else
            {
                try
                {
                    var response = await client.GetAsync(query);
                    response.EnsureSuccessStatusCode();
                    result = (await response.Content.ReadAsStringAsync()).Replace(@"""", "");
                }
                catch (HttpRequestException ex)
                {
                    result = ex.ToString();
                }
                encryptedInfo.Text = "number encrypted retrieved after new query was made ";
                Encrypted.Text = result;
                Barrel.Current.Add(query, result, expireIn: TimeSpan.FromSeconds(10));
            }
        }
    }
}
