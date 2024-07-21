using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace ServerCore
{
    public static class SocketAsyncEventArgsEx
    {
        public static bool IsRecvSuccess(this SocketAsyncEventArgs args)
        {
            return (args.SocketError == SocketError.Success && args.BytesTransferred > 0);
        }
    }
}
