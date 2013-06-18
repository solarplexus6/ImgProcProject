//
//  GIMP plugin
//  Region Filling and Object Removal by Exemplar-Based Image Inpainting
//  Rafal Lukaszewski, 2013
//

using ExemplarInpaintingPlugin.Consts;
using Gimp;

namespace ExemplarInpaintingPlugin
{
	class ObjectRemovalPlugin : Plugin
	{
        #region Overrides

        protected override GimpDialog CreateDialog()
        {
            gimp_ui_init(_("ObjectRemovalPlugin"), true);
            return new Dialog(Variables);
        }

        protected override Procedure GetProcedure()
        {
            return new Procedure("plug-in-object-removal",
                                 _("ObjectRemovalPlugin"),
                                 _("ObjectRemovalPlugin"),
                                 "Rafal Lukaszewski",
                                 "(C) Rafal Lukaszewski",
                                 "2013",
                                 _("Inpainting"),
                                 "RGB*, GRAY*") { MenuPath = "<Image>/Filters/Object Removal" };
        }

        override protected void Render(Image image, Drawable drawable)
        {
            var renderer = new Renderer(Variables);
            renderer.Render(image, drawable);
        }

        #endregion
        #region Public static methods

        public static void Main(string[] args)
        {
            var variables = new VariableSet
            {
                new Variable<int>(VariablesConsts.ITERATIONS, _("Number of iterations"), 2)
            };
            GimpMain<ObjectRemovalPlugin>(args, variables);
        }

        #endregion
	}
}
