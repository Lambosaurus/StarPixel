using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarPixel
{
    interface IComms
    {
        List<int> Ping(Ship ship);

    }
}
