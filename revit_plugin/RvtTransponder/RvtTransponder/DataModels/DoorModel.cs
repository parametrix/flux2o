using Autodesk.Revit.DB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RvtTransponder.DataModels
{
    class DoorModel:BimE
    {
        public DoorModel(string uid, Element door) : base(uid)
        {
            FamilyInstance fi = door as FamilyInstance;
            Category = door.Category.Name;
            Name = door.Name;
            Mark = door.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString();
            LevelName = door.Document.GetElement(door.LevelId).Name;
            //ProjectId = door.Document.ProjectInformation.UniqueId.Replace("-", "").Substring(0,16);
            ProjectId = TransponderUtils.GetProjectGUID(door.Document);
            var location = door.Location;
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

                var toroom = fi.ToRoom;
                if (null != toroom) { ToRoomId = toroom.UniqueId.Replace("-", ""); }
                var fromroom = fi.FromRoom;
                if (null != fromroom) { FromRoomId = fromroom.UniqueId.Replace("-", ""); }

                HandFlipped = fi.HandFlipped;
                HandOrientation = fi.HandOrientation;
                FacingFlipped = fi.FacingFlipped;
                FacingOrientation = fi.FacingOrientation;
            }

            var widthParam = fi.Symbol.get_Parameter(BuiltInParameter.DOOR_WIDTH);
            if (null != widthParam)
            {
                Width = widthParam.AsDouble();
            }

            var htParam = fi.Symbol.get_Parameter(BuiltInParameter.DOOR_HEIGHT);
            if (null != htParam)
            {
                Height = htParam.AsDouble();
            }
        }

        [JsonProperty("toroom")]
        public string ToRoomId { get; set; }

        [JsonProperty("fromroom")]
        public string FromRoomId { get; set; }

        [JsonProperty("width")]
        public double Width { get; set; }

        [JsonProperty("height")]
        public double Height { get; set; }

    }
}
