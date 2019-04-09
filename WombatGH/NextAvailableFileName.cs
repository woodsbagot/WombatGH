using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using WombatGH.Properties;

namespace WombatGH
{
    public class NextAvailableFileName : GH_Component
    {

        public NextAvailableFileName() : base("Next Available Filename", "NextFile", "Chooses the next unused filename in a directory following a pattern", "Wombat", "Files / Interop")
        {
        }

        public override Guid ComponentGuid => new Guid("{67BA052D-1616-42C7-AAA8-1945CE8D869F}");
        protected override Bitmap Icon => Resources.WombatGH_NextAvailableFileName;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Format", "F", "Filename text format, like \"File_{0:000}.png\". Must include an index placeholder {0}.",
                GH_ParamAccess.item, "{0:000}.png");
            pManager.AddTextParameter("Folder Path", "P", "The path to the folder you want to generate filepaths for",
                GH_ParamAccess.item);
            var trigger = pManager.AddGenericParameter("Trigger", "T", "An optional input to force a refresh of the file path",
                GH_ParamAccess.tree);
            pManager[trigger].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("File Path", "FP", "The unused File Path", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string format = "";
            string folderPath = "";
            if (!DA.GetData("Format", ref format)) return;
            if (!DA.GetData("Folder Path", ref folderPath)) return;      

            DA.SetData("File Path", GetNextFilename(Path.Combine(folderPath + "/", format)));
        }

        private static string GetNextFilename(string pattern)
        {
            string tmp = string.Format(pattern, 1);
            if (tmp == pattern)
                throw new ArgumentException("The pattern must include an index place-holder", nameof(pattern));

            if (!File.Exists(tmp))
                return tmp; // short-circuit if no matches

            int min = 1, max = 2; // min is inclusive, max is exclusive/untested

            while (File.Exists(string.Format(pattern, max)))
            {
                min = max;
                max *= 2;
            }

            while (max != min + 1)
            {
                int pivot = (max + min) / 2;
                if (File.Exists(string.Format(pattern, pivot)))
                    min = pivot;
                else
                    max = pivot;
            }

            return string.Format(pattern, max);
        }
    }
}
