using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Newtonsoft.Json.Linq;
using RvtTransponder.WebUtils;
using System;
using System.Collections.Generic;

namespace RvtTransponder
{
    [Transaction(TransactionMode.ReadOnly)]
    class Cmd_UploadProject:IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            TransponderUtils.StringBuilder = new System.Text.StringBuilder(); // for error recording

            // upload all rooms
            ElementCategoryFilter roomFilter = new ElementCategoryFilter(BuiltInCategory.OST_Rooms);
            ElementCategoryFilter doorFilter = new ElementCategoryFilter(BuiltInCategory.OST_Doors);

            var categories = new List<BuiltInCategory>()
            {
                BuiltInCategory.OST_Furniture,
                BuiltInCategory.OST_FurnitureSystems,
                BuiltInCategory.OST_Casework,
                BuiltInCategory.OST_PlumbingFixtures
            };

            ElementMulticategoryFilter multicategoryFilter = new ElementMulticategoryFilter(categories);

            var rooms = new FilteredElementCollector(doc)
                .WherePasses(roomFilter)
                .ToElements();

            var doors = new FilteredElementCollector(doc)
                .WherePasses(doorFilter)
                .WhereElementIsNotElementType()
                .ToElements();

            var fe = new FilteredElementCollector(doc)
                .WherePasses(multicategoryFilter)
                .WhereElementIsNotElementType()
                .ToElements();


            // combined array
            JArray rArray = TransponderUtils.GetRoomModels(rooms);
            JArray dArray = TransponderUtils.GetDoorModels(doors);
            
            // concat rooms arrays
            foreach(var item in rArray)
            {
                dArray.Add(item);
            }
            // concat fe array
            if (null != fe && fe.Count>0)
            {
                JArray feArray = TransponderUtils.GetFeModels(fe);
                foreach(var item in feArray)
                {
                    dArray.Add(item);
                }
            }
            TransponderUtils.TransmitModels(dArray);
            

            return Result.Succeeded;
        }
    }
}
