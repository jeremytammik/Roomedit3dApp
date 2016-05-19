#region Namespaces
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion

namespace Roomedit3dApp
{
  [Transaction( TransactionMode.ReadOnly )]
  public class Command : IExternalCommand
  {
    public Result Execute(
      ExternalCommandData commandData,
      ref string message,
      ElementSet elements )
    {
      bool subscribed = App.ToggleSubscription();

      TaskDialog.Show( "Toggled Subscription",
        ( subscribed ? "S" : "Uns" ) + "ubscribed." );

      return Result.Succeeded;
    }
  }
}
