using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

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
