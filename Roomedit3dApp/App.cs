#region Namespaces
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
#endregion

namespace Roomedit3dApp
{
  class App : IExternalApplication
  {
    /// <summary>
    /// Caption
    /// </summary>
    public const string Caption = "Roomedit3d";

    /// <summary>
    /// Socket broadcast URL.
    /// </summary>
    const string _url = "https://roomedit3d.herokuapp.com:443";

    #region External event subscription and handling
    /// <summary>
    /// Store the external event.
    /// </summary>
    static ExternalEvent _event = null;

    /// <summary>
    /// Store the external event.
    /// </summary>
    static BimUpdater _bimUpdater = null;

    /// <summary>
    /// Store the socket we are listening to.
    /// </summary>
    static Socket _socket = null;

    /// <summary>
    /// Provide public read-only access to external event.
    /// </summary>
    public static ExternalEvent Event
    {
      get { return _event; }
    }

    /// <summary>
    /// Enqueue a new BIM updater task.
    /// </summary>
    static void Enqueue( object data )
    {
      JObject data2 = JObject.FromObject( data );

      string s = string.Format(
        "transform: uid={0} ({1:0.00},{2:0.00},{3:0.00})",
        data2["externalId"], data2["offset"]["x"],
        data2["offset"]["y"], data2["offset"]["z"] );

      Util.Log( "Enqueue task " + s );

      string uid1 = data2["externalId"].ToString();
      XYZ offset1 = new XYZ(
        double.Parse( data2["offset"]["x"].ToString() ),
        double.Parse( data2["offset"]["y"].ToString() ),
        double.Parse( data2["offset"]["z"].ToString() ) );

      string uid = (string) data2["externalId"];
      XYZ offset = new XYZ(
        (double) data2["offset"]["x"],
        (double) data2["offset"]["y"],
        (double) data2["offset"]["z"] );

      _bimUpdater.Enqueue( uid, offset );
      _event.Raise();
    }

    /// <summary>
    /// Toggle on and off subscription to automatic 
    /// BIM update from cloud. Return true when subscribed.
    /// </summary>
    public static bool ToggleSubscription()
    {
      if ( null != _event )
      {
        Util.Log( "Unsubscribing..." );

        _socket.Disconnect();
        _socket = null;

        _bimUpdater = null;

        _event.Dispose();
        _event = null;

        Util.Log( "Unsubscribed." );
      }
      else
      {
        Util.Log( "Subscribing..." );

        _bimUpdater = new BimUpdater();

        var options = new IO.Options()
        {
          IgnoreServerCertificateValidation = true,
          AutoConnect = true,
          ForceNew = true
        };

        _socket = IO.Socket( _url, options );

        _socket.On( Socket.EVENT_CONNECT, ()
          => Util.Log( "Connected" ) );

        _socket.On( "transform", data
          => Enqueue( data ) );

        _event = ExternalEvent.Create( _bimUpdater );

        Util.Log( "Subscribed." );
      }
      return null != _event;
    }
    #endregion // External event subscription and handling

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
