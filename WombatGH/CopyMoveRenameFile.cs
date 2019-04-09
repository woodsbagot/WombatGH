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
    public class CopyMoveRenameFile : GH_Component
    {

        public CopyMoveRenameFile() : base("Copy/Move/Rename Files", "CopyFile", "Allows you to copy or rename/move a file", "Wombat", "Files / Interop")
        {
        }

        public override Guid ComponentGuid => new Guid("{FE1A5116-43A2-47E8-81C6-6A8501F6A861}");
        protected override Bitmap Icon => Resources.WombatGH_CopyMoveRenameFile;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Run", "R", "Set to true to begin the operation", GH_ParamAccess.item, false);
            pManager.AddTextParameter("From Path", "F", "The file path you want to move or copy from",
                GH_ParamAccess.item);
            pManager.AddTextParameter("To Path", "T", "The path you want to move/copy/rename to", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Move", "M",
                "Set to true to move/rename the file, set to false to just make a copy", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Output path", "O", "The path to the moved/renamed/copied file",
                GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var run = false;
            var fromPath = "";
            var toPath = "";
            var deleteOld = false;

            if (!DA.GetData("Run", ref run)) return;
            if (!DA.GetData("From Path", ref fromPath)) return;
            if (!DA.GetData("To Path", ref toPath)) return;
            if (!DA.GetData("Move", ref deleteOld)) return;

            if (!run) return;

            File.Copy(fromPath,toPath);
            if(deleteOld) File.Delete(fromPath);

            DA.SetData("Output path",toPath);
        }
    }
}
