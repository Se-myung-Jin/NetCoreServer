﻿using System.Net.Sockets;

namespace ServerCore
{
    public class SocketAsyncEventArgsPool
    {
        Stack<SocketAsyncEventArgs> pool;

        public SocketAsyncEventArgsPool(int capacity)
        {
            pool = new Stack<SocketAsyncEventArgs>(capacity);
        }

        public void Push(SocketAsyncEventArgs item)
        {
            if (item == null) { throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null"); }
            
            lock (pool)
            {
                pool.Push(item);
            }
        }

        public SocketAsyncEventArgs Pop()
        {
            lock (pool)
            {
                return pool.Pop();
            }
        }

        public int Count => pool.Count;
    }
}
