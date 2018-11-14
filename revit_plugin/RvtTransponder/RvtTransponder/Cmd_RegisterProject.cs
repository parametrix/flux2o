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
    class Cmd_RegisterProject:IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            TransponderUtils.RegisterProject(doc);
            

            return Result.Succeeded;
        }
    }
}
