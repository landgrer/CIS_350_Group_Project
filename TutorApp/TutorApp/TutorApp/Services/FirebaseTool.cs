using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TutorApp.Services
{
    public class FirebaseTool
    {
        IFirebaseConfig firebaseConfig = new FirebaseConfig() { BasePath = "https://loginwith-dce6a-default-rtdb.firebaseio.com/" };
        IFirebaseClient client;

        public FirebaseTool()
        {
            try
            {
                client = new FirebaseClient(firebaseConfig);
            }
            catch (Exception ex)
            {
                Task.Run(async () =>
                {
                    string title = "Error";
                    string message = ex.ToString();
                    await Application.Current.MainPage.DisplayAlert(title, message, "OK");
                });
            }
        }

        public async void Call()
        {
            FirebaseResponse response = await client.GetAsync(@"Meeting");
            Dictionary<string, Dictionary<string, string>> data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(response.Body.ToString());
        }
    }
}
