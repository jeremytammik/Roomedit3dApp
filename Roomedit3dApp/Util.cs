#region Namespaces
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion // Namespaces

namespace Roomedit3dApp
{
  class Util
  {
    #region Messages
    /// <summary>
    /// Return an English plural suffix 's' or
    /// nothing for the given number of items.
    /// </summary>
    public static string PluralSuffix( int n )
    {
      return 1 == n ? "" : "s";
    }

    /// <summary>
    /// Display a short big message.
    /// </summary>
    public static void InfoMsg( string msg )
    {
      Util.Log( msg );
      TaskDialog.Show( App.Caption, msg );
    }

    /// <summary>
    /// Display a longer message in smaller font.
    /// </summary>
    public static void InfoMsg2(
      string instruction,
      string msg,
      bool prompt = true )
    {
      Util.Log( string.Format( "{0}: {1}", 
        instruction, msg ) );

      if ( prompt )
      {
        TaskDialog dlg = new TaskDialog( App.Caption );
        dlg.MainInstruction = instruction;
        dlg.MainContent = msg;
        dlg.Show();
      }
    }

    /// <summary>
    /// Display an error message.
    /// </summary>
    public static void ErrorMsg( string msg )
    {
      Util.Log( msg );
      TaskDialog dlg = new TaskDialog( App.Caption );
      dlg.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
      dlg.MainInstruction = msg;
      dlg.Show();
    }

    /// <summary>
    /// Print a debug log message with a time stamp
    /// to the Visual Studio debug output window.
    /// </summary>
    public static void Log( string msg )
    {
      string timestamp = DateTime.Now.ToString(
        "HH:mm:ss.fff" );

      Debug.Print( timestamp + " " + msg );
    }
    #endregion // Messages
  }
}
