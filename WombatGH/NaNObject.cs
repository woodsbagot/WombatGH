using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using WombatGH.Properties;

namespace WombatGH
{
    public class NaNObject : GH_Component
    {

        public NaNObject() : base("NaN Object", "NaN", "Creates a NaN ('not a number') object", "Wombat", "Sets")
        {
        }

        public override Guid ComponentGuid => new Guid("{2B77B060-7695-45CA-A53A-4F21F9B9C36C}");
        protected override Bitmap Icon => Resources.WombatGH_NaNObject;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("NaN Object", "N", "A NaN object", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.SetData("NaN Object", double.NaN);
        }
    }
}