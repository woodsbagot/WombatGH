using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WombatGH.Properties;

namespace WombatGH
{
    public class DirectoryFromFilePath : GH_Component
    {

        public DirectoryFromFilePath()
          : base("Directory From File Path", "Dir From Path", "Returns folder path instead of file path.", "Wombat", "Files / Interop")
        {
        }

        public override Guid ComponentGuid => new Guid("{bf45bde9-4769-4d7b-bd07-4b5397b03bfb}");
        protected override Bitmap Icon => Resources.WombatGH_DirFromFilePath;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("File Path", "F", "File path", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("File Directory", "D", "File directory", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string file = "";
            DA.GetData("File Path", ref file);
            string dir = Path.GetDirectoryName(file);

            DA.SetData("File Directory", dir);
        }
    }
}