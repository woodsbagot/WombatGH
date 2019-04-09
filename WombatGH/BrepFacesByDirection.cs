using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Geometry;
using System.Drawing;
using WombatGH.Properties;

namespace WombatGH
{
    public class BrepFacesByDirection : GH_Component
    {
        public BrepFacesByDirection()
            : base(
                "Brep Faces By Direction", "FBD", "Gets brep faces by their normal direction compared to the Z axis",
                "Wombat", "Brep")
        {           
        }

        public override Guid ComponentGuid => new Guid("{D4168E0F-D5F5-4EFC-8259-9A87F4BFFF4C}");
        protected override Bitmap Icon => Resources.WombatGH_BRepFacesByDir;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Brep", "B", "The brep from which to get faces", GH_ParamAccess.item);
            pManager.AddNumberParameter("Tolerance", "T",
                "The allowed tolerance from vertical for something to be counted as a top or bottom surface. Value from 0 to 1",
                GH_ParamAccess.item, 0.9);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Top", "T", "Upward-Facing surfaces", GH_ParamAccess.list);
            pManager.AddBrepParameter("Bottom", "B", "Downward-Facing surfaces", GH_ParamAccess.list);
            pManager.AddBrepParameter("Sides", "S", "All the remaining surfaces", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var b = default(Brep);
            var tol = default(double);

            if (!DA.GetData("Brep", ref b)) return;
            if (!DA.GetData("Tolerance", ref tol)) return;

            var topFaces = new List<Brep>();
            var btmFaces = new List<Brep>();
            var sideFaces = new List<Brep>();

            foreach (var face in b.Faces)
            {
                
               var faceNormal = face.NormalAt(face.Domain(0).Mid, face.Domain(1).Mid);
                faceNormal.Unitize();
                var dotProduct = faceNormal*Vector3d.ZAxis;
                if (dotProduct > tol) //upward facing
                {
                    topFaces.Add(face.DuplicateFace(true));
                    
                } else if (-dotProduct > tol) //downward facing
                {
                    btmFaces.Add(face.DuplicateFace(true));
                }
                else //neither
                {
                    sideFaces.Add(face.DuplicateFace(true));
                }
            }

            DA.SetDataList("Top", topFaces);
            DA.SetDataList("Bottom", btmFaces);
            DA.SetDataList("Sides", sideFaces);
        }
    }
}
