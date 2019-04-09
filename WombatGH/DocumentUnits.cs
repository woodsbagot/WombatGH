using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;
using WombatGH.Properties;

namespace WombatGH
{
    public class DocumentUnits : GH_Component
    {

        public DocumentUnits()
          : base("Document Units", "Doc Units", "Returns unit system of active Rhino document.", "Wombat", "Document")
        {
        }

        public override Guid ComponentGuid => new Guid("{a2128790-7d1a-491a-a3ed-e288db57560c}");
        protected override Bitmap Icon => Resources.WombatGH_DocUnits;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Document Units", "U", "Active Rhino document unit system.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            UnitSystem docUnits = RhinoDoc.ActiveDoc.ModelUnitSystem;
            DA.SetData("Document Units", docUnits);
        }

        public override void AddedToDocument(GH_Document document)
        {
            if (Locked)
            {
                RemoveHandlers();
            }
            else
            {
                AddHandlers();
            }
        }

        public override void RemovedFromDocument(GH_Document document)
        {
            RemoveHandlers();
        }

        private void AddHandlers()
        {
            RemoveHandlers();
            RhinoDoc.DocumentPropertiesChanged += OnUnitsChanged;
        }

        private void RemoveHandlers()
        {
            RhinoDoc.DocumentPropertiesChanged -= OnUnitsChanged;
        }

        private UnitSystem cachedUnitSystem = RhinoDoc.ActiveDoc.ModelUnitSystem;

        private void OnUnitsChanged(object sender, DocumentEventArgs e)
        {
            if (e.Document.ModelUnitSystem != cachedUnitSystem)
            {
                cachedUnitSystem = e.Document.ModelUnitSystem;
                ExpireSolution(true);
            }
        }
    }
}