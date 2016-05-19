#region Namespaces
using Autodesk.Revit.UI;
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
    const string _url = "https://roomedit3d.herokuapp.com/";

    #region External event subscription and handling
    static bool _subscribed = false;

    /// <summary>
    /// Store the external event.
    /// </summary>
    static ExternalEvent _event = null;

    /// <summary>
    /// Store the socket.
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
    /// Toggle on and off subscription to automatic 
    /// BIM update from cloud. Return true when subscribed.
    /// </summary>
    public static bool ToggleSubscription()
    {
      if ( _subscribed )
      {
        Util.Log( "Unsubscribing..." );

        _socket.Disconnect();
        _socket = null;

        _event.Dispose();
        _event = null;

        Util.Log( "Unsubscribed." );
      }
      else
      {
        Util.Log( "Subscribing..." );

        _socket = IO.Socket( _url );

        _socket.On( "transform", data => Util.Log( data.ToString() ) );

        _event = ExternalEvent.Create( 
          new BimUpdater() );

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
