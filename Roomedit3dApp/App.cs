#region Namespaces
using Autodesk.Revit.UI;
#endregion

namespace Roomedit3dApp
{
  class App : IExternalApplication
  {
    static bool _subscribed = false;

    /// <summary>
    /// Toggle subscription state and return the new state.
    /// </summary>
    /// <returns></returns>
    static public bool ToggleSubscription()
    {
      return _subscribed = !_subscribed;
    }

    public Result OnStartup( UIControlledApplication a )
    {
      return Result.Succeeded;
    }

    public Result OnShutdown( UIControlledApplication a )
    {
      return Result.Succeeded;
    }
  }
}
