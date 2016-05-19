#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion

namespace Roomedit3dApp
{
  /// <summary>
  /// BIM updater, driven both via external 
  /// command and external event handler.
  /// </summary>
  class BimUpdater : IExternalEventHandler
  {
    Queue<object> _queue = null;

    public BimUpdater()
    {
      _queue = new Queue<object>();
    }

    /// <summary>
    /// Execute method invoked by Revit via the 
    /// external event as a reaction to a call 
    /// to its Raise method.
    /// </summary>
    public void Execute( UIApplication a )
    {
      Document doc = a.ActiveUIDocument.Document;

      using ( Transaction t = new Transaction( doc ) )
      {
        t.Start( GetName() );

        while( 0 < _queue.Count )
        {
          Debug.Print( _queue.Dequeue().ToString() );
        }

        t.Commit();
      }
    }

    /// <summary>
    /// Required IExternalEventHandler interface 
    /// method returning a descriptive name.
    /// </summary>
    public string GetName()
    {
      return App.Caption + " " + GetType().Name;
    }

    /// <summary>
    /// Enqueue a BIM update action to be performed.
    /// </summary>
    /// <param name="data"></param>
    public void Enqueue( object data )
    {
      _queue.Enqueue( data );
    }
  }
}
