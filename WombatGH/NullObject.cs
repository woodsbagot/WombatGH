using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WombatGH.Properties;

namespace WombatGH
{
    public class NullObject : GH_Component
    {

        public NullObject()
          : base("Null Object", "Null", "Creates a null object.", "Wombat", "Sets")
        {
        }

        public override Guid ComponentGuid => new Guid("{1bcf1fba-c281-4b85-afe7-e2c7548d0c9e}");
        protected override Bitmap Icon => Resources.WombatGH_NullObj;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Null Object", "N", "A null object.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.SetData("Null Object", null);
        }       
    }
}