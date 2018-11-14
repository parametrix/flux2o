using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RvtTransponder.DataModels
{
    class RoomModel:BimE
    {
        private const double MIN_CURVE_LENGTH = 3;

        public RoomModel(string uid, Room room, SpatialElementBoundaryOptions bOptions):base(uid)
        {
            Category = room.Category.Name;
            Name = room.Name;
            Mark = room.Number;
            LevelName = room.Level.Name;
            //ProjectId = room.Document.ProjectInformation.UniqueId.Replace("-", "").Substring(0,16);
            ProjectId = TransponderUtils.GetProjectGUID(room.Document);
            LocationPt = (room.Location as LocationPoint).Point;

            IList<IList<BoundarySegment>> bSegmentLists = room.GetBoundarySegments(bOptions);
            if (null == bSegmentLists) { return; }

            List<List<XYZ>> vLoopList = new List<List<XYZ>>();
            SvgPaths = new List<string>();
            foreach(var segmentList in bSegmentLists)
            {
                var vLoop = GetVertexLoopsFromBoundarySegments(segmentList);
                if (null != vLoop)
                {
                    vLoopList.Add(vLoop);
                    SvgPaths.Add(GetAbsSvgPathFromVLoop(vLoop));
                }
            }
            VLoopList = vLoopList;
        }

        

        /// <summary>
        /// Get Vertex Loops from SegmentLists
        /// </summary>
        /// <param name="boundarySegments"></param>
        /// <returns></returns>
        private List<XYZ> GetVertexLoopsFromBoundarySegments(IList<BoundarySegment> boundarySegments)
        {
            List<XYZ> vertices = new List<XYZ>();
            foreach (BoundarySegment segment in boundarySegments)
            {
                Curve curve = segment.GetCurve();

                if (vertices.Count < 1) { vertices.Add(curve.GetEndPoint(0)); }

                // tesselate
                if (curve.IsBound && !(curve is Line) && curve.ApproximateLength>MIN_CURVE_LENGTH)
                {
                    var points = curve.Tessellate();
                    points.RemoveAt(0);
                    if (points.Count > 0)
                    {
                        vertices.AddRange(points);
                    }
                }
                else // just create a straight line
                {
                    vertices.Add(curve.GetEndPoint(1));
                }
            }
            return vertices;
        }
    }
}
