using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace RvtTransponder.WebUtils
{
    class JsonWebController
    {
        internal string ServerResponse { get; set; }
        public string p_Message { get; set; }

        internal async void SendRequest(string jsonString)
        {
            JsonWebHandler client = new JsonWebHandler();
            client.NewDataReceived += Client_NewDataRecievedHandler;
            await client.PostData(jsonString);
        }

        private void Client_NewDataRecievedHandler(object sender, NewDataReceivedEventArgs e)
        {
            this.ServerResponse = e.Data;
            this.p_Message = "Response Recieved";
            System.Windows.MessageBox.Show(e.Data);
        }
    }

    class JsonWebHandler
    {
        internal string ServerResponse { get; set; }
        public string p_Message { get; set; }

        // Create custom Event Args: http://stackoverflow.com/questions/12055431/c-string-as-parameter-to-event
        internal delegate void NewDataReceivedEvent(object sender, NewDataReceivedEventArgs args);
        internal event NewDataReceivedEvent NewDataReceived;
        protected virtual void OnNewDataReceived(string response)
        {
            if (NewDataReceived != null)
            {
                NewDataReceived(this, new NewDataReceivedEventArgs(response));
            }
        }

        internal async Task PostData(string json)
        {
            using (var client = new HttpClient())
            {
                var url = new Uri(AppData.GetEndPoint(EndPoint.MySqlEndPoint));
                client.BaseAddress = url;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                
                var httpcontent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, httpcontent);
                if (response.IsSuccessStatusCode && NewDataReceived != null)
                {
                    var responseBodyAsText = response.Content.ReadAsStringAsync().Result;
                    OnNewDataReceived(responseBodyAsText.ToString());
                }
            }
        }
    }
}
