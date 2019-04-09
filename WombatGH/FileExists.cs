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
    public class FileExists : GH_Component
    {

        public FileExists() : base("File or Directory Exists", "Exists", "Returns True if supplied file path or directory exists.", "Wombat", "Files / Interop")
        {
        }

        public override Guid ComponentGuid => new Guid("{ECCFE9FE-64CF-46DA-9F22-BBC3372360EA}");
        protected override Bitmap Icon => Resources.WombatGH_FileExists;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Path", "P", "File path or directory as string.", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("Exists", "E", "True if file or directory exists, False if it doesn't.",
                GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string path = "";
            DA.GetData("Path", ref path);

            bool fExists = File.Exists(path);
            bool dExists = Directory.Exists(path);
            
            DA.SetData("Exists", fExists || dExists);
        }
    }
}
