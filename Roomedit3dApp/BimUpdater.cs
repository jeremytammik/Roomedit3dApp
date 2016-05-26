﻿#region Namespaces
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
    /// <summary>
    /// The queue of pending tasks consisting 
    /// of UniqueID and translation offset vector.
    /// </summary>
    Queue<Tuple<string, XYZ>> _queue = null;

    public BimUpdater()
    {
      _queue = new Queue<Tuple<string, XYZ>>();
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

        while ( 0 < _queue.Count )
        {
          Tuple<string, XYZ> task = _queue.Dequeue();

          Debug.Print( "Translating {0} by {1}",
            task.Item1, Util.PointString( task.Item2 ) );

          Element e = doc.GetElement( task.Item1 );
          ElementTransformUtils.MoveElement(
            doc, e.Id, task.Item2 );
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
    /// Enqueue a BIM update action to be performed,
    /// consisting of UniqueID and translation 
    /// offset vector.
    /// </summary>
    public void Enqueue( string uid, XYZ offset )
    {
      _queue.Enqueue(
        new Tuple<string, XYZ>(
          uid, offset ) );
    }
  }
}
