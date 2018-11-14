using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace RvtTransponder.WebUtils
{
    class WebHandler
    {
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

        // get async callbacks from here http://stackoverflow.com/questions/29228072/c-sharp-callback-after-http-get-post-completes
        // USE THIS ONLY FOR PAYLOAD LESS THAN 2038 CHARACTERS
        internal async Task PostData(PostData data)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(data.URL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/text"));
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/plain"));

                HttpResponseMessage response;


                FormUrlEncodedContent content = new FormUrlEncodedContent(data.DataPairs);
                // to avoid uri errror (uri has limit of 2038 characters)
                //// from:https://stackoverflow.com/questions/38440631/httpclient-the-uri-string-is-too-long
                //var stringPayload = JsonConvert.SerializeObject(data.DataPairs.Values);
                //var content = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                // http post
                response = await client.PostAsync(data.URL, content);
                if (response.IsSuccessStatusCode && NewDataReceived != null)
                {
                    var responseBodyAsText = response.Content.ReadAsStringAsync().Result;

                    OnNewDataReceived(responseBodyAsText.ToString());
                }
            }
        }
    }
}
