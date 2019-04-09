using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Grasshopper.Kernel;
using Rhino.Geometry;
using WombatGH.Properties;

namespace WombatGH
{
    public class FileNameFromFilePath : GH_Component
    {

        public FileNameFromFilePath()
          : base("File Name From File Path", "Name From Path", "Returns file name with and without file extension from file path.",
              "Wombat", "Files / Interop")
        {
        }

        public override Guid ComponentGuid => new Guid("{452c7adb-18f6-4cae-bc6c-e78fb81c23ff}");
        protected override Bitmap Icon => Resources.WombatGH_FileNameFromFilePath;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("File Path", "F", "File path", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "File name", GH_ParamAccess.item);
            pManager.AddTextParameter("Name With Extension", "E", "File name with file extension", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string file = "";
            DA.GetData("File Path", ref file);

            string name = Path.GetFileNameWithoutExtension(file);
            DA.SetData("Name", name);

            string nameExt = Path.GetFileName(file);
            DA.SetData("Name With Extension", nameExt);
        }  
    }
}