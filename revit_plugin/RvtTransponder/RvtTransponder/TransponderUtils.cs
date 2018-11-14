using Autodesk.Revit.DB;
using Autodesk.Revit.DB.IFC;
using Autodesk.Revit.DB.Architecture;
using Newtonsoft.Json.Linq;
using RvtTransponder.DataModels;
using RvtTransponder.WebUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RvtTransponder
{
    class TransponderUtils
    {
        public static StringBuilder StringBuilder;

        internal static JArray GetRoomModels(IList<Element> roomElements)
        {
            Document doc = roomElements.FirstOrDefault().Document;
            SpatialElementBoundaryOptions bOptions = new SpatialElementBoundaryOptions();
            bOptions.SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Finish;
            JArray array = new JArray();
            foreach(Element e in roomElements)
            {
                Room room = e as Room;
                RoomModel roomModel = new RoomModel(e.UniqueId.Replace("-", ""), room, bOptions);
                var o = JToken.FromObject(roomModel);
                array.Add(o);
            }
            return array;
        }

        internal static JArray GetDoorModels(IList<Element> doorElements)
        {
            Document doc = doorElements.FirstOrDefault().Document;
            JArray array = new JArray();
            foreach(Element e in doorElements)
            {
                DoorModel doorModel = new DoorModel(e.UniqueId.Replace("-", ""), e);
                var o = JToken.FromObject(doorModel);
                array.Add(o);
            }
            return array;
        }

        internal static JArray GetFeModels(IList<Element> feElements)
        {
            Document doc = feElements.FirstOrDefault().Document;
            JArray array = new JArray();
            foreach (Element e in feElements)
            {
                FEmodel feModel = new FEmodel(e.UniqueId.Replace("-", ""), e);
                var o = JToken.FromObject(feModel);
                array.Add(o);
            }
            if (TransponderUtils.StringBuilder.Length > 0)
            {
                MessageBox.Show(TransponderUtils.StringBuilder.ToString());
                TransponderUtils.StringBuilder.Clear();
            }
            
            return array;
        }

        /// <summary>
        /// Method to upload files
        /// </summary>
        /// <param name="jArray"></param>
        internal static void TransmitModels(JArray jArray)
        {
            JsonWebController webController = new JsonWebController();
            webController.SendRequest(jArray.ToString(Newtonsoft.Json.Formatting.None));
        }

        internal static void RegisterProject(Document doc)
        {
            PostData data = new PostData();
            data.URL = AppData.GetEndPoint(EndPoint.ProjectRegistrationEndPoint);
            data.DataPairs = new Dictionary<string, string>();
            //data.DataPairs.Add("uid", doc.ProjectInformation.UniqueId.Replace("-", "").Substring(0,16));
            data.DataPairs.Add("uid", GetProjectGUID(doc));
            data.DataPairs.Add("fileName", doc.Title);

            WebController webController = new WebController();
            webController.SendRequest(data);
        }

        internal static void GetRhinoUpdates(Document doc)
        {
            PostData data = new PostData();
            data.URL = AppData.GetEndPoint(EndPoint.IndexEndPoint);
            data.DataPairs = new Dictionary<string, string>();
            data.DataPairs.Add("query", TransmissionType.GET_RHINO_UPDATES.ToString());
            //data.DataPairs.Add("uid", doc.ProjectInformation.UniqueId.Replace("-", "").Substring(0, 16));
            data.DataPairs.Add("uid", GetProjectGUID(doc));
            data.DataPairs.Add("fileName", doc.Title);

            WebController webController = new WebController();
            webController.SendRequest(data);
        }

        // get ifc guid for project
        internal static string GetProjectGUID(Document doc)
        {
            string projectid = Autodesk.Revit.DB.IFC.ExporterIFCUtils.CreateProjectLevelGUID(doc, IFCProjectLevelGUIDType.Project);
            return projectid;
        }
    }
}
