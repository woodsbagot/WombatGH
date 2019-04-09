using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WombatGH.Properties;

namespace WombatGH
{
    public class SurfaceDirection : GH_Component
    {

        public SurfaceDirection() : base("Surface Direction", "SrfDirection",
            "Modify surface u, v, and normal directions", "Wombat", "Surface")
        {
        }

        public override Guid ComponentGuid => new Guid("{768CEBBC-4014-42AE-8AC1-3AB03722C7F9}");
        protected override Bitmap Icon => Resources.WombatGH_SrfDir;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface", "S", "Surface", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Flip", "F", "Set True to flip normal", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Swap", "S", "Swap {u} and {v} directions", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Reverse U", "U", "Reverse {u} direction", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Reverse V", "V", "Reverse {v} direction", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Surface", "S", "Surface with new direction", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var brep = default(Brep);
            var flip = false;
            var swap = false;
            var revU = false;
            var revV = false;

            if (!DA.GetData("Surface", ref brep)) return;
            DA.GetData("Flip", ref flip);
            DA.GetData("Swap", ref swap);
            DA.GetData("Reverse U", ref revU);
            DA.GetData("Reverse V", ref revV);

            var isSrf = brep.IsSurface;

            //swap uv
            if (swap)
            {
                brep = brep.Faces[0].Transpose(true).ToBrep();
            }

            //flip u
            if (revU)
            {
                brep = brep.Faces[0].Reverse(0, true).ToBrep();
            }

            //flip v
            if (revV)
            {
                brep = brep.Faces[0].Reverse(1, true).ToBrep();
            }

            //flip brep
            if (flip)
            {
                brep.Flip();
            }

            if (isSrf && (swap || revU || revV))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "This operation will untrim your surface!");
            }

            DA.SetData("Surface", brep);
        }
    }
}
