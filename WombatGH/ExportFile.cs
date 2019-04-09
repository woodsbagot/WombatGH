using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino;
using Rhino.DocObjects;
using WombatGH.Properties;

namespace WombatGH
{
    public class ExportFile : GH_Component
    {

        public ExportFile() : base("Export File to Path", "Export", "Exports a set of geometry to a specified file format.", "Wombat", "Files / Interop")
        {
        }

        public override Guid ComponentGuid => new Guid("{AF090F39-F4F6-4E77-815B-F7418191D06B}");
        protected override Bitmap Icon => Resources.WombatGH_Export_File;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Run", "R", "Set to true to begin the export", GH_ParamAccess.item, false);
            pManager.AddGeometryParameter("Geometry to Export", "G", "The geometry you want to export.",
                GH_ParamAccess.list);
            pManager.AddTextParameter("Export Path", "P", "The path to export to. File type will be inferred from file extension.", GH_ParamAccess.item);
            var atts = pManager.AddGenericParameter("Attributes", "att",
                 "The object attributes to assign to each piece of geometry. Layer name input on this component will be ignored.",
                 GH_ParamAccess.list);
            pManager[atts].Optional = true;
            var layname = pManager.AddTextParameter("Layer Names", "L", "The layer names to bake to. \n" +
                                                        "These layers will be created for the \n" +
                                                        "export and then deleted afterwards if \n" +
                                                        "they did not already exist. If this input \n" +
                                                        "is supplied, the Attributes input will be \n" +
                                                        "ignored.", GH_ParamAccess.list);
            pManager[layname].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Output Path", "P", "The path to the created file on success.",
                GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var run = false;
            var geometry = new List<IGH_GeometricGoo>();
            var exportPath = "";
            var atts = new List<ObjectAttributes>();
            var layerNames = new List<string>();

            if (!DA.GetData("Run", ref run)) return;
            if (!DA.GetDataList("Geometry to Export", geometry)) return;
            if (!DA.GetData("Export Path", ref exportPath)) return;
            var hasAtts = DA.GetDataList("Attributes", atts);
            var hasLayerName = DA.GetDataList("Layer Names", layerNames);

            if (hasAtts && hasLayerName)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Supply EITHER layer names or attributes - not both. Since both are supplied, layer names will be ignored.");
            }

            if (!run)
            {
                var inputButtons = Params.Input[0].Sources.OfType<Grasshopper.Kernel.Special.GH_ButtonObject>();
                foreach (var btn in inputButtons)
                {
                    Grasshopper.Instances.ActiveCanvas.ActiveObject = null;
                }
                return;
            }

            var addedLayers = new List<Layer>();
            var RhinoDocument = Rhino.RhinoDoc.ActiveDoc;
            var layerTable = RhinoDocument.Layers;
            if (hasLayerName && !hasAtts)
            {

                foreach (var layerName in layerNames)
                {
                    int index;
                    //var index = RhinoDocument.Layers.Find(layerName, true);
                    Rhino.DocObjects.Layer layer = layerTable.FindName(layerName);
                    //if (index < 0)
                    //{
                    //    index = RhinoDocument.Layers.Add(layerName, Color.Black);
                    //    addedLayers.Add(RhinoDocument.Layers[index]);
                    //}
                    if (layer == null)
                    {
                       index = layerTable.Add(layerName, Color.Black);
                       addedLayers.Add(layerTable[index]);
                    }
                    else
                    {
                        index = layer.Index;
                    }

                    atts.Add(new ObjectAttributes
                    {
                        LayerIndex = index
                    });
                }
            }

            if (atts.Count == 0) atts.Add(new ObjectAttributes());

            //bake everything
            var bakedObjects = new List<ObjRef>();
            for (var i = 0; i < geometry.Count; i++)
            {
                var goo = geometry[i];
                var bakeable = goo as IGH_BakeAwareData;
                if (bakeable != null)
                {
                    try
                    {
                        bakeable.BakeGeometry(RhinoDocument, atts[Math.Min(i, atts.Count - 1)], out Guid guid);
                        bakedObjects.Add(new ObjRef(guid));
                    }
                    catch
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Some objects failed to bake.");
                    }
                }
                else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Some objects were not bakeable.");
                }

            }

            //select and export everything
            RhinoDocument.Objects.UnselectAll();
            RhinoDocument.Objects.Select(bakedObjects.Where(o => o.ObjectId != Guid.Empty));
            var extension = Path.GetExtension(exportPath).ToUpper();
            String script;
            switch (extension)
            {
                case ".OBJ":
                    script = $"-_Export \"{exportPath}\" _Enter _Enter _Enter";
                    break;
                case ".AI":
                case ".3DS":
                case ".FBX":
                    script = $"-_Export \"{exportPath}\" _Enter _Enter";
                    break;
                case ".3DM":
                    script = $"-_Export \"{exportPath}\" _Enter";
                    break;
                case ".DWG":
                case ".SAT":
                default:
                    script = $"-_Export \"{exportPath}\" Default _Enter";
                    break;
            }

            RhinoApp.RunScript(script, false);
            bakedObjects.ForEach(o => RhinoDocument.Objects.Delete(o, true));
            foreach (var layer in addedLayers)
            {
                RhinoDocument.Layers.Delete(layer.Index, true);
            }
        }
    }
}
