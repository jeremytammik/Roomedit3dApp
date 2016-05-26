using System.Threading;
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
using System;

namespace Roomedit3dSocketTest
{
  class Program
  {
    const string _url = "https://roomedit3d.herokuapp.com:443";

    static void Main( string[] args )
    {
      var options = new IO.Options()
      {
        IgnoreServerCertificateValidation = true,
        AutoConnect = true,
        ForceNew = true
      };

      Socket socket = IO.Socket( _url, options );

      socket.On( Socket.EVENT_CONNECT, () =>
        Console.WriteLine( "Connected" ) );

      socket.On( "transform", (data) =>
      {
        JObject data2 = JObject.FromObject( data );

        Console.WriteLine( string.Format(
          "transform: externalId={0} ({1:0.00},{2:0.00},{3:0.00})", 
          data2["externalId"], data2["offset"]["x"], 
          data2["offset"]["y"], data2["offset"]["z"] ) );
      } );

      while ( true ) { Thread.Sleep( 100 ); }
    }
  }
}
