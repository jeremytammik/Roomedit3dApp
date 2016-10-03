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
    /// Switch between earlier v1 and current v2.
    /// </summary>
    const bool _use_forge_oauth = true;

    /// <summary>
    /// Caption
    /// </summary>
    public const string Caption = "Roomedit3d" 
      + (_use_forge_oauth ? "V3" : "");

    /// <summary>
    /// Socket broadcast URL.
    /// </summary>
    const string _url_v1 = "https://roomedit3d.herokuapp.com:443";
    const string _url_v2 = "https://roomedit3dv2.herokuapp.com:443";
    const string _url_v3 = "https://roomedit3dv3.herokuapp.com:443";
    const string _url = _use_forge_oauth ? _url_v3 : _url_v1;

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
    /// Enqueue a new BIM updater task.
    /// </summary>
    static void Enqueue( object data )
    {
      JObject data2 = JObject.FromObject( data );

      string s = string.Format(
        "transform: uid={0} ({1:0.00},{2:0.00},{3:0.00})",
        data2["externalId"], data2["translation"]["x"],
        data2["translation"]["y"], data2["translation"]["z"] );

      Util.Log( "Enqueue task " + s );

      string uid1 = data2["externalId"].ToString();
      XYZ translation1 = new XYZ(
        double.Parse( data2["translation"]["x"].ToString() ),
        double.Parse( data2["translation"]["y"].ToString() ),
        double.Parse( data2["translation"]["z"].ToString() ) );

      string uid = (string) data2["externalId"];
      XYZ translation = new XYZ(
        (double) data2["translation"]["x"],
        (double) data2["translation"]["y"],
        (double) data2["translation"]["z"] );

      _bimUpdater.Enqueue( uid, translation );
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
