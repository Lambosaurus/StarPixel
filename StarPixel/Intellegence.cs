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
        public Vector2 control_thrust = new Vector2(0,0);
        public float control_torque = 0;
    }



    public class Intellegence
    {
        protected IntOutputs outputs = new IntOutputs(); // we keep a copy of this, because its faster

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

            KeyboardState key_state = Keyboard.GetState();

            
            outputs.control_thrust.Y = key_state.IsKeyDown(Keys.W) ? 1.0f : ((key_state.IsKeyDown(Keys.S)) ? -1.0f : 0.0f);
            
            outputs.control_thrust.X = key_state.IsKeyDown(Keys.D) ? 1.0f : ((key_state.IsKeyDown(Keys.A)) ? -1.0f : 0.0f);

            outputs.control_torque = key_state.IsKeyDown(Keys.E) ? 1.0f : ((key_state.IsKeyDown(Keys.Q)) ? -1.0f : 0.0f);


            return outputs;
        }
    }
}





