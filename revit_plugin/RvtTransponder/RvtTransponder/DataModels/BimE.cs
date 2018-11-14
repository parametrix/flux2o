using Autodesk.Revit.DB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RvtTransponder.DataModels
{
    class BimE
    {
        private const double MIN_CURVE_LENGTH = 0.25;

        protected BimE(string uid)
        {
            Category = "BimE";
            Id = uid;
        }

        [JsonProperty("category")]
        public string Category { get; protected set; }

        [JsonProperty("uid")]
        public string Id { get; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("mark")]
        public string Mark { get; set; }

        [JsonProperty("projectId")]
        public string ProjectId { get; set; }

        [JsonProperty("levelName")]
        public string LevelName { get; set; }

        [JsonProperty("location")]
        public XYZ LocationPt { get; set; }

        [JsonProperty("rotation")]
        public double ZRotation { get; set; }

        [JsonProperty("handflipped")]
        public bool HandFlipped { get; set; }

        [JsonProperty("facingflipped")]
        public bool FacingFlipped { get; set; }

        [JsonProperty("handorientation")]
        public XYZ HandOrientation { get; set; }

        [JsonProperty("facingorientation")]
        public XYZ FacingOrientation { get; set; }

        [JsonProperty("symbolid")]
        public string SymbolId { get; set; }

        [JsonIgnore]
        public IList<List<XYZ>> VLoopList { get; set; }

        [JsonProperty("svgPaths")]
        public IList<string> SvgPaths { get; set; }

        [JsonProperty("properties")]
        public Dictionary<string,string> Properties { get; set; }

        internal List<string> GetInstanceGeometryAsSvgPaths(Element e)
        {
            Autodesk.Revit.Creation.Application creapp = e.Document.Application.Create;
            Options options = new Options();
            options.DetailLevel = ViewDetailLevel.Coarse;
            try
            {
                if(e is FamilyInstance)
                {
                    FamilyInstance fi = e as FamilyInstance;
                    GeometryElement geo = fi.get_Geometry(options);
                    
                    if (null == geo) { return null; }

                    LocationPoint lp = e.Location as LocationPoint;
                    Transform transform = fi.GetTotalTransform();
                    var transformedGeometry = geo.GetTransformed(transform);

                    Solid union = null;
                    List<Solid> solids = new List<Solid>();
                    foreach(GeometryObject obj in transformedGeometry)
                    {
                        Solid solid = obj as Solid;
                        if (null != solid && 0 < solid.Faces.Size)
                        {
                            if (null == union) { union = solid; }
                            else
                            {
                                Solid interSolid = BooleanOperationsUtils.ExecuteBooleanOperation(union, solid, BooleanOperationsType.Intersect);
                                if (Math.Abs(interSolid.Volume) > 0.000001)
                                {
                                    // it intersects
                                    union = BooleanOperationsUtils.ExecuteBooleanOperation(union, solid, BooleanOperationsType.Union);
                                }
                                else
                                {
                                    // add it to a collection of solids to create an array
                                    solids.Add(solid);
                                }
                            }
                        }
                    }
                    // create loops
                    List<string> unionSvgPaths = new List<string>(); List<string> solidSvgPaths = new List<string>();
                    if (null != union) { unionSvgPaths = GetSvgPathsFromSolid(union); }
                    if (solids.Count > 0) // include the non- intersecting solids
                    {
                        foreach(Solid solid in solids)
                        {
                            solidSvgPaths.AddRange(GetSvgPathsFromSolid(solid));
                        }
                        solidSvgPaths.AddRange(unionSvgPaths);
                        return solidSvgPaths;
                    }
                    return unionSvgPaths;
                }
            }
            catch(Exception ex)
            {
                //MessageBox.Show(ex.Message, "Error Creating Geometry Element");
                TransponderUtils.StringBuilder.AppendLine(e.Name+":"+e.Id.ToString()+":"+ex.Message);
            }
            return null;
        }

        private List<string> GetSvgPathsFromSolid(Solid solid)
        {
            Plane plane = Plane.CreateByOriginAndBasis(XYZ.Zero, XYZ.BasisX, XYZ.BasisY);
            ExtrusionAnalyzer extrusionAnalyzer = ExtrusionAnalyzer.Create(solid, plane, XYZ.BasisZ);
            Face face = extrusionAnalyzer.GetExtrusionBase();
            if (null == face) { return null; }
            List<string> svgPathList = new List<string>();
            foreach (EdgeArray a in face.EdgeLoops)
            {
                List<XYZ> vertices = GetVerticesFromEdgeArray(a);
                if (null != vertices)
                {
                    var svgpath = GetAbsSvgPathFromVLoop(vertices);
                    svgPathList.Add(svgpath);
                }
            }
            return svgPathList;
        }

        private List<XYZ> GetVerticesFromEdgeArray(EdgeArray edgeArray)
        {
            List<XYZ> vertices = new List<XYZ>();
            foreach (Edge edge in edgeArray)
            {
                Curve curve = edge.AsCurve();

                if (vertices.Count < 1) { vertices.Add(curve.GetEndPoint(0)); }

                // tesselate
                if (curve.IsBound && !(curve is Line) && curve.ApproximateLength > MIN_CURVE_LENGTH)
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

        internal string GetAbsSvgPathFromVLoop(List<XYZ> vLoop)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("M");
            foreach (XYZ vertex in vLoop)
            {
                sb.Append(vertex.X + "," + vertex.Y + " ");
            }
            string path = sb.ToString().TrimEnd(' ');
            path += "Z";
            return path;
        }
    }
}
