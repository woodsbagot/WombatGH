using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WombatGH.Properties;

namespace WombatGH
{
    public class NullsInEmptyBranches : GH_Component
    {

        public NullsInEmptyBranches()
          : base("Nulls In Empty Branches", "Nulls in Empty", "Inserts null objects into empty branches.", "Wombat", "Sets")
        {
        }

        public override Guid ComponentGuid => new Guid("{25e3dc9c-4f9f-4d72-81a3-3bc5a33ba1ef}");
        protected override Bitmap Icon => Resources.WombatGH_NullsEmptyBranch;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data with Empty Branches", "D", "Data whose empty branches will be filled with null objects.",
                GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Data with Nulls", "D", "Data whose empty branches have been filled with null objects.",
                GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var data = new List<object> ();

            if (!DA.GetDataList("Data with Empty Branches", data)) return;

            if (data.Count == 0)
            {
                data = new List<object>() { null };
            }

            DA.SetDataList("Data with Nulls", data);
        }
    }
}