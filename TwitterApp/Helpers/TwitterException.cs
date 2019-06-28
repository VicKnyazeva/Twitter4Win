using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterApp.Helpers
{
    class TwitterException : Exception
    {
        public TwitterException(string message)
            : base(message)
        {

        }

        public TwitterException(string message, Exception innerException)
            : base(message + "\n\t" + innerException.Message, innerException)
        {

        }
    }
}
