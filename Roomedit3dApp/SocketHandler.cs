using System.Threading;
using Autodesk.Revit.UI;

namespace Roomedit3dApp
{
  class SocketHandler
  {
    /// <summary>
    /// Separate thread accepting socket broadcasts 
    /// and raising the external event.
    /// </summary>
    Thread _thread = null;

    /// <summary>
    /// Toggle subscription to automatic database 
    /// updates. Forward the call to the external 
    /// application that creates the external event,
    /// store it and launch a separate thread checking 
    /// for database updates. When changes are pending,
    /// invoke the external event Raise method.
    /// </summary>
    public SocketHandler(
      UIApplication uiapp )
    {
      // Start a new thread to regularly check the
      // database status and raise the external event
      // when updates are pending.

      _thread = new Thread(
        CheckForPendingDatabaseChanges );

      _thread.Start();
    }

  }
}
