//***Automaticall generate unique int; Thread-safe lock-free counter
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DreamFlights.Services
{
    public sealed class IncrementCounter
    {
        // use a meaningful name, 'i' by convention should only be used in a for loop.
        private int current = 1;
        //设置递增整数的初始值
        static volatile public int currentStatic = 1;

        // update the method name to imply that it returns something.
        public int NextValue()
        {
            // prefix fields with 'this'
            //return Interlocked.Increment(ref this.current);
            return currentStatic++;
        }

        public void Reset()
        {
            this.current = 0;
        }
    }
}
