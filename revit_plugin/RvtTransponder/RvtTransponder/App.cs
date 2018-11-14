using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RvtTransponder.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RvtTransponder
{
    public class App : IExternalApplication
    {
        public static UIControlledApplication CachedUiCtrApp;
        public static UIApplication UiApp;
        public static string RvtVersion { get; set; }


        public Result OnStartup(UIControlledApplication uic_app)
        {
            // read configuration for mysql endpoints
            AppData.GetEndPoint(EndPoint.MySqlEndPoint);

            // register updater
            RoomUpdater roomUpdater = new RoomUpdater(uic_app.ActiveAddInId);
            UpdaterRegistry.RegisterUpdater(roomUpdater);

            ElementCategoryFilter roomFilter = new ElementCategoryFilter(BuiltInCategory.OST_Rooms);
            ElementCategoryFilter doorFilter = new ElementCategoryFilter(BuiltInCategory.OST_Doors);
            ElementLogicalFilter elementLogicalFilter = new LogicalOrFilter(roomFilter, doorFilter);
            UpdaterRegistry.AddTrigger(roomUpdater.GetUpdaterId(), elementLogicalFilter, Element.GetChangeTypeGeometry());

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication uic_app)
        {
            UpdaterRegistry.UnregisterUpdater(new RoomUpdater(uic_app.ActiveAddInId).GetUpdaterId());

            return Result.Succeeded;
        }
    }
}
