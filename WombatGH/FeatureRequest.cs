using System;
using System.Collections.Generic;

using System.Windows.Forms;

using Grasshopper;
using Grasshopper.GUI;

using Grasshopper.Kernel;
using Grasshopper.Plugin;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel.Attributes;
using WombatGH.Properties;

using Rhino.Geometry;
using System.Diagnostics;

namespace WombatGH
{
    public class FeatureRequest : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the FeatureRequest class.
        /// </summary>
        public FeatureRequest()
          : base("FeatureRequest", "Request",
              "Request a new Wombat tool",
              "Wombat", "Wombat")
        {

        }

        public override void CreateAttributes()
        {
            m_attributes = new FeatureRequestAttributes(this);
        }

        public override void AddedToDocument(GH_Document document)
        {
            Process.Start("https://github.com/woodsbagot/WombatGH/issues/new/choose");
            base.AddedToDocument(document);
            GH_Document doc = Grasshopper.Instances.ActiveCanvas.Document;
            doc.RemoveObject(Attributes, true);

        }
        public class FeatureRequestAttributes : GH_ComponentAttributes
        {
            public FeatureRequestAttributes(IGH_Component FeatureRequest) : base(FeatureRequest) { }

            
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.WombatGH_FeatureRequest; 

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("82fde8cd-3e18-4185-9ec4-e649cc993137"); }
        }
    }
}