using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using WombatGH.Properties;

namespace WombatGH
{
    public class FBXElementID : GH_Component
    {

        public FBXElementID() : base("Element ID from FBX Import", "FBX ID", "Gets the element id from a referenced element brought in from a Revit FBX Export", "Wombat", "Files / Interop")
        {
        }

        public override Guid ComponentGuid => new Guid("{A375EED6-96DD-4C10-AF59-7BD5D2010A40}");
        protected override Bitmap Icon => Resources.WombatGH_ElementIDFromFBX;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGeometryParameter("Geometry", "G", "The referenced geometry from the FBX Import",
                GH_ParamAccess.item);   
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Element ID", "I", "The element ID of this object", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IGH_GeometricGoo geoGoo = null;
            if (!DA.GetData("Geometry", ref geoGoo)) return;

            if (!geoGoo.IsReferencedGeometry)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "This geometry is not referenced from Rhino.");
                return;
            }

            var rhinoObj = Rhino.RhinoDoc.ActiveDoc.Objects.Find(geoGoo.ReferenceID);
            var name = rhinoObj.Attributes.Name;
            if (!name.Contains("["))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "This geometry does not have element ID embedded in its name.");
                return;
            }

            var id = name.Split(new[] {'[', ']'}).LastOrDefault(s => !String.IsNullOrEmpty(s));

            DA.SetData("Element ID", id);
        }
    }
}
