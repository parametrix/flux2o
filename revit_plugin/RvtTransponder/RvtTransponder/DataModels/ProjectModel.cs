using Autodesk.Revit.DB;
using Autodesk.Revit.DB.IFC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RvtTransponder.DataModels
{
    public class ProjectModel
    {
        public ProjectModel(Document doc)
        {
            ProjectUID = TransponderUtils.GetProjectGUID(doc);
            ProjectTitle = doc.Title;
            IsRegistered = GetRegistrationStatus(ProjectUID);
        }

        public string ProjectUID { get; set; }
        public bool IsRegistered { get; set; }
        public string ProjectTitle { get; set; }

        private bool GetRegistrationStatus(string projectUID)
        {
            throw new NotImplementedException();
        }
    }
}
