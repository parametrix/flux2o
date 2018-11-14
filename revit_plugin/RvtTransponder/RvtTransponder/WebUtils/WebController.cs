using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

namespace RvtTransponder.WebUtils
{
    class WebController
    {
        internal string ServerResponse { get; set; }
        public string p_Message { get; set; }

        internal async void SendRequest(PostData data)
        {
            if (data.DataPairs.Values.FirstOrDefault().Length > 65536)
            {
                System.Windows.MessageBox.Show("The amount of data exceeds the value that can be tranmitted with this application.");
                return;
            }
            // from http://stackoverflow.com/questions/29228072/c-sharp-callback-after-http-get-post-completes
            WebHandler client = new WebHandler();
            client.NewDataReceived += Client_NewDataRecievedHandler;
            await client.PostData(data);
            
        }

        private void Client_NewDataRecievedHandler(object sender, NewDataReceivedEventArgs e)
        {
            this.ServerResponse = e.Data;
            this.p_Message = "Response Recieved";
            System.Windows.MessageBox.Show(e.Data);
        }
    }
}
