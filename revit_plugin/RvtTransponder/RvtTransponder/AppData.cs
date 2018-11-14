using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RvtTransponder
{
    class AppData
    {
        internal static string GuidString = "2BB18D10-1550-4FD9-B8FC-516B8B828395";
        //internal static string MySqlEndPoint = @"http://flux2o.ml/rvttransponder_02/html/postData.php";
        //internal static string ProjectRegistrationEndPoint = @"http://flux2o.ml/rvttransponder_02/html/registerProject.php";
        //internal static string IndexEndPoint = @"http://flux2o.ml/rvttransponder_02/html/index.php";

        
        
        /// <summary>
        /// Read data from config file
        /// </summary>
        /// <param name="endpt"></param>
        /// <returns></returns>
        internal static string GetEndPoint(EndPoint endpt)
        {
            string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string yamlPath = Path.Combine(assemblyPath, @".config", @"endpoints");
            if(!File.Exists(yamlPath))
            {
                MessageBox.Show("Configuration File does not exist", "Error");
                return null;
            }
            string contents = File.ReadAllText(yamlPath);
            // split by lines
            var lines = contents.Split(new[] { Environment.NewLine },StringSplitOptions.RemoveEmptyEntries);
            if(null==lines || lines.Length < 1)
            {
                MessageBox.Show("Empty Configuration File", "Error");
                return null;
            }

            // interate to find the endpoint
            foreach (string line in lines)
            {
                var pair = line.Split(';');
                if (pair[0].Equals(endpt.ToString()))
                {
                    return pair[1];
                }
            }
            MessageBox.Show("Endpoint not found: "+endpt.ToString(), "Error");
            return null;
        }
    }

    internal enum EndPoint
    {
        MySqlEndPoint,
        ProjectRegistrationEndPoint,
        IndexEndPoint
    }
}
