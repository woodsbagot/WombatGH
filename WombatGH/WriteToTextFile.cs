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
    public class WriteToTextFile : GH_Component
    {

        public WriteToTextFile() : base("Write to Text File", "WriteTXT", "Writes a list of strings to a text file", "Wombat", "Files / Interop")
        {
        }

        public override Guid ComponentGuid => new Guid("{6BA91370-7F61-4B51-A123-A09B181B4D32}");
        protected override Bitmap Icon => Resources.WombatGH_WriteToTxtFile;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Run", "R", "Set to true to stream or write the text. Always on by default!", GH_ParamAccess.item, true);
            pManager.AddTextParameter("Text to write", "T", "The list of strings you wish to write to a text file",
                GH_ParamAccess.list);
            pManager.AddTextParameter("File Path","P","The path to which you want to write",GH_ParamAccess.item);
            pManager.AddBooleanParameter("Append", "A", "Set to true to append text", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("New Only", "N", "Set to true to only add new lines not currently in the document - only applies in \"append\" mode.", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("Done", "D", "True on successful completion", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var run = false;
            var toWrite = new List<string>();
            bool append = false;
            bool newOnly = false;
            var filePath = "";
            if (!DA.GetData("Run", ref run)) return;
            if (!DA.GetDataList("Text to write", toWrite)) return;
            if (!DA.GetData("File Path", ref filePath)) return;
            if (!DA.GetData("Append", ref append)) return;
            if (!DA.GetData("New Only", ref newOnly)) return;

            if (!run) return;

            try
            {
                if (append)
                {
                    IEnumerable<String> textToWrite = toWrite;
                    if (newOnly)
                    {
                        var existing = File.ReadAllLines(filePath);
                        textToWrite = toWrite.Except(existing);
                    }
                    File.AppendAllLines(filePath, textToWrite);
                }
                else
                {
                    File.WriteAllLines(filePath, toWrite);
                }
                DA.SetData("Done", true);
            }
            catch
            {
                DA.SetData("Done", false);
            }
        }
    }
}
