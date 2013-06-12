using Gimp;
using Gtk;

namespace ExemplarInpaintingPlugin
{
    public class Dialog : GimpDialog
    {
        #region Ctors

        public Dialog(VariableSet variables) :
            base(_("Hello World"), variables)
        {
            VBox.Add(new Label(_("Hello world from visual studio build action")));
        }

        #endregion
    }
}