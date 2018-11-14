using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Newtonsoft.Json.Linq;
using RvtTransponder.WebUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RvtTransponder
{
    public class RoomUpdater:IUpdater
    {
        private AddInId mAddinId;
        private UpdaterId mUpdaterId;
        private string mUpdaterName;

        public RoomUpdater(AddInId id)
        {
            mAddinId = id;
            mUpdaterId = new UpdaterId(mAddinId, new Guid(AppData.GuidString));
            mUpdaterName = "Room Updater";
        }

        public void Execute(UpdaterData data)
        {
            Document doc = data.GetDocument();
            ICollection<ElementId> changedIds= data.GetModifiedElementIds();
            var changedElements = changedIds.Select(x => doc.GetElement(x));

            // updated rooms = 
            JArray roomArray = null;
            JArray doorArray = null;
            var rooms = changedElements.Where(x => x.Category.Id.IntegerValue == BuiltInCategory.OST_Rooms.GetHashCode()).ToList();
            if (null != rooms && rooms.Count>0)
            {
                roomArray = TransponderUtils.GetRoomModels(rooms);
            }
            
            // updated doors
            var doors = changedElements.Where(x => x.Category.Id.IntegerValue == BuiltInCategory.OST_Doors.GetHashCode()).ToList();
            if (null != doors && doors.Count>0)
            {
                doorArray = TransponderUtils.GetDoorModels(doors);
            }

            // combine values
            if (null != doorArray && null!=roomArray)
            {
                foreach (var item in roomArray)
                {
                    doorArray.Add(item);
                }
                TransponderUtils.TransmitModels(doorArray);
            }
            else if (null != roomArray)
            {
                TransponderUtils.TransmitModels(roomArray);
            }
            else if (null != doorArray)
            {
                TransponderUtils.TransmitModels(doorArray);
            }
        }

        public string GetAdditionalInformation()
        {
            return mUpdaterName;
        }

        public ChangePriority GetChangePriority()
        {
            return ChangePriority.FreeStandingComponents;
        }

        public UpdaterId GetUpdaterId()
        {
            return mUpdaterId;
        }

        public string GetUpdaterName()
        {
            return mUpdaterName;
        }
    }
}
