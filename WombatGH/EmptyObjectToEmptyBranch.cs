using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WombatGH.Properties;

namespace WombatGH
{
    public class EmptyObjectToEmptyBranch : GH_Component
    {

        public EmptyObjectToEmptyBranch()
          : base("Empty Object To Empty Branch", "Empty", "Returns an empty branch when no data is supplied.", "Wombat", "Sets")
        {
        }

        public override Guid ComponentGuid => new Guid("{7a3939e4-2f31-4567-84f3-ac4bb9496adf}");
        protected override Bitmap Icon => Resources.WombatGH_EmptyObjToEmptyBranch;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data In", "D", "Data in.", GH_ParamAccess.list);
            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Data Out", "D",
                "Returns empty branch if no data is supplied, otherwise returns data in.", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var dataIn = new List<object>();
            List<object> empty = new List<object>();
            if (!DA.GetDataList("Data In", dataIn))
            {
                DA.SetDataList("Data Out", empty);
            }
            else
            {
                DA.SetDataList("Data Out", dataIn);
            }
        }
    }
}