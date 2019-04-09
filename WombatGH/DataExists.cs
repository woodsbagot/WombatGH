using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using WombatGH.Properties;

namespace WombatGH
{
    public class DataExists : GH_Component
    {

        public DataExists() : base("Data Exists", "HasData", "Tests for existence of data", "Wombat", "Sets")
        {
        }

        public override Guid ComponentGuid => new Guid("{DA5E9B5D-BF45-4595-9709-AEF8CA96BE8D}");
        protected override Bitmap Icon => Resources.WombatGH_DataExists;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "The data to test for existence", GH_ParamAccess.tree);
            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("Existence Flag", "E", "True if data exists", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Structure<IGH_Goo> data;
            bool hasData = DA.GetDataTree(0, out data);

            DA.SetData(0, data.Branches.Count!=0);
        }

    }
}
