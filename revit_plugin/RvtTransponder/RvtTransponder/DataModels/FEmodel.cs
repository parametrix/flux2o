using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Newtonsoft.Json;

namespace RvtTransponder.DataModels
{
    class FEmodel:BimE
    {
        public FEmodel(string uid, Element element) : base(uid)
        {
            FamilyInstance fi = element as FamilyInstance;
            Category = element.Category.Name;
            Name = element.Name;
            LevelName = element.Document.GetElement(element.LevelId).Name;
            //ProjectId = element.Document.ProjectInformation.UniqueId.Replace("-", "").Substring(0, 16);
            ProjectId = TransponderUtils.GetProjectGUID(element.Document);
            var location = element.Location;
            if (null != location)
            {
                LocationPt = (location as LocationPoint).Point;
                ZRotation = (location as LocationPoint).Rotation;
            }

            if (null != fi)
            {
                var symbol = fi.Symbol;
                if (null != symbol)
                {
                    SymbolId = symbol.UniqueId.Replace("-", "");
                }
            }
            SvgPaths = base.GetInstanceGeometryAsSvgPaths(element);
            // get room id
            Room room = fi.Room;
            if (null != room) { RoomId = room.UniqueId.Replace("-", ""); }
        }

        [JsonProperty("roomid")]
        public string RoomId { get; set; }

        private void GetSymbolGeometry(Element e)
        {
            Options options = new Options();
            options.DetailLevel = ViewDetailLevel.Coarse;
            FamilyInstance fi = e as FamilyInstance;
            Transform transform = fi.GetTotalTransform();


            GeometryElement geometryElement = e.get_Geometry(options);
            var geometryObject = geometryElement as GeometryObject;

            fi.Symbol.get_Geometry(options);
        }
    }
}
