//
//  GIMP plugin
//  Region Filling and Object Removal by Exemplar-Based Image Inpainting
//  Rafal Lukaszewski, 2013
//

using ExemplarInpaintingPlugin.Consts;
using Gimp;
using Gtk;

namespace ExemplarInpaintingPlugin
{
    public class Dialog : GimpDialog
    {
        #region Ctors

        public Dialog(VariableSet variables) :
            base(_("Object removal and region filling"), variables)
        {
            var table = new GimpTable(3, 4) { ColumnSpacing = 6, RowSpacing = 6 };
            VBox.PackStart(table, false, false, 0);

            var iterationsEntry = new ScaleEntry(table, 0, 0, _("_Iterations"), 150, 10,
                                                 GetVariable<int>(VariablesConsts.ITERATIONS),
                                                 1.0, 500.0, 1.0, 10.0, 0);
            
        }

        #endregion
    }
}