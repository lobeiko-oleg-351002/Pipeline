using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceChannelManager
{
    public class ConnectionFailedException : Exception
    {
        public ConnectionFailedException() 
        {

        }

        public ConnectionFailedException(string message) : base(message)
        {
        }
    }
}
