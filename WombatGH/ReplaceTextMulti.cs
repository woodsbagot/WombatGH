using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using WombatGH.Properties;

namespace WombatGH
{
    public class ReplaceTextMultiple : GH_Component
    {

        public ReplaceTextMultiple() : base("Replace Text (Multiple)", "RepM", "Replace all occurences of any pattern in a list with the corresponding replacement string", "Wombat", "Text")
        {
        }

        public override Guid ComponentGuid => new Guid("{A9A95A92-20F9-4B3C-AD26-B3A772665C57}");
        protected override Bitmap Icon => Resources.WombatGH_ReplaceTextMulti;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Text", "T", "The text to operate on", GH_ParamAccess.item);
            pManager.AddTextParameter("Fragment", "F", "The list of fragments to replace", GH_ParamAccess.list);
            pManager.AddTextParameter("Replacement", "R", "The replacement fragments. If blank, all instances of F will be removed", GH_ParamAccess.list);
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Text", "T", "Result of text replacement", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string T = "";
            var F = new List<string>();
            var R = new List<string>();
            if (!DA.GetData("Text", ref T)) return;
            if (!DA.GetDataList("Fragment", F)) return;
            DA.GetDataList("Replacement", R);

            if (!R.Any()) R.Add("");
            for (var i = 0; i < F.Count; i++)
            {
                T = T.Replace(F[i], R[i % R.Count]);
            }

            DA.SetData("Text", T);
        }
    }
}
