using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Geometry;
using Grasshopper.Kernel.Types;
using WombatGH.Properties;

namespace WombatGH
{
    public class ShiftBothWays : GH_Component
    {

        public ShiftBothWays() : base("Shift List Both Ways", "ShiftBoth", "Shifts a list in both directions", "Wombat", "Sets")
        {
        }

        public override Guid ComponentGuid => new Guid("{DEF10DD3-5A6B-4E84-931A-874184851B2F}");
        protected override Bitmap Icon => Resources.WombatGH_ShiftListBoth;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("List", "L", "The list to shift", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Shift Offset", "S", "The offset for the list in each direction",
                GH_ParamAccess.item, 1);
            pManager.AddBooleanParameter("Wrap", "W",
                "Set to true to wrap and offset by 0 and S - otherwise does not wrap and offsets by S and -S",
                GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("List A", "A", "The resulting backward-shifted list (or the original list if wrap is true)", GH_ParamAccess.list);
            pManager.AddGenericParameter("List B", "B", "The resulting forward-shifted list", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var data = new List<IGH_Goo>();
            int shift = 1;
            bool wrap = false;
            if (!DA.GetDataList("List", data)) return;
            if (!DA.GetData("Shift Offset", ref shift)) return;
            if (!DA.GetData("Wrap", ref wrap)) return;

            var baseIndices = Enumerable.Range(0, data.Count);

            var indicesA = baseIndices.Select(a => a + (wrap ? 0 : -shift));
            var indicesB = baseIndices.Select(b => b + shift);
            if (wrap) indicesB = indicesB.Select(b => GH_MathUtil.WrapInteger(b, data.Count));
            indicesA = indicesA.Where(a => a >= 0 && a < data.Count);
            indicesB = indicesB.Where(b => b >= 0 && b < data.Count);

            DA.SetDataList("List A", indicesA.Select(a => data[a]));
            DA.SetDataList("List B", indicesB.Select(b => data[b]));
        }
    }
}
