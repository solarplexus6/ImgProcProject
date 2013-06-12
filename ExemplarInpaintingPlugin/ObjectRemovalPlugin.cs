using Gimp;

namespace ExemplarInpaintingPlugin
{
	class ObjectRemovalPlugin : Plugin
	{
        #region Overrides

        protected override GimpDialog CreateDialog()
        {
            gimp_ui_init("HelloPlugin", true);
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
                                 _("Exemplar based "),
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
            };
            GimpMain<ObjectRemovalPlugin>(args, variables);
        }

        #endregion
	}
}
