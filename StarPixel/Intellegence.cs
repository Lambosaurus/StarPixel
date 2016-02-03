using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarPixel
{
    public class IntInputs
    {

    }

    public class IntOutputs
    {

    }



    public class Intellegence
    {
        public Intellegence()
        {

        }

        public virtual IntOutputs Process( IntInputs inputs )
        {
            return null;
        }
    }


    public class IntellegenceHuman : Intellegence
    {
        public IntellegenceHuman()
        {

        }

        public override IntOutputs Process(IntInputs inputs)
        {
            return null;
        }
    }

}
