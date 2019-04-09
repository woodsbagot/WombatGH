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
    public class ShrinkSurface : GH_Component
    {

        public ShrinkSurface() : base("Shrink Surface", "Shrink", 
            "Contract underlying untrimmed surface close to trimming boundaries", "Wombat", "Surface")
        {
        }

        public override Guid ComponentGuid => new Guid("{41D2B0D2-5D6D-4054-917C-96058934B8FB}");
        protected override Bitmap Icon => Resources.WombatGH_ShrinkSrf;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Surface", "S", "Surface to shrink", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Surface", "S", "Contracted surface", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var srf = default(Brep);
            if (!DA.GetData("Surface", ref srf)) return;
            if (srf.IsSurface) AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "There's no need to shrink untrimmed surfaces!");

            srf.Faces.ShrinkFaces();

            DA.SetData("Surface", srf);
        }
    }           
}
