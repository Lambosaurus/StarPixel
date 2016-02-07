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
        public Vector2 pos;
        public float angle;
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

            
            outputs.control_thrust.X = key_state.IsKeyDown(Keys.W) ? 1.0f : ((key_state.IsKeyDown(Keys.S)) ? -1.0f : 0.0f);
            
            outputs.control_thrust.Y = key_state.IsKeyDown(Keys.D) ? 1.0f : ((key_state.IsKeyDown(Keys.A)) ? -1.0f : 0.0f);

            outputs.control_torque = key_state.IsKeyDown(Keys.E) ? 1.0f : ((key_state.IsKeyDown(Keys.Q)) ? -1.0f : 0.0f);
            return outputs;
        }
    }


    public class IntellegenceHunter : Intellegence
    {
        Ship target;


        PID angle_tracker;
        PID x_tracker;
        PID y_tracker;

        public IntellegenceHunter( Ship arg_target )
        {
            target = arg_target;

            /*
            PID angle_tracker = new PID(10f, 0.1f, 10f);
            PID x_tracker = new PID(0.05f, 0.01f, 0.5f);
            PID y_tracker = new PID(0.05f, 0.01f, 0.5f);
            */

            angle_tracker = new PID(10f * Utility.random.Next(5, 15) / 10f, 0.1f * Utility.random.Next(5, 15) / 10f, 10f * Utility.random.Next(5, 15) / 10f);
            x_tracker = new PID(0.05f * Utility.random.Next(5, 15) / 10f, 0.01f * Utility.random.Next(5, 15) / 10f, 0.5f * Utility.random.Next(5, 15) / 10f);
            y_tracker = new PID(0.05f * Utility.random.Next(5, 15) / 10f, 0.01f * Utility.random.Next(5, 15) / 10f, 0.5f * Utility.random.Next(5, 15) / 10f);
        }

        public override IntOutputs Process(IntInputs inputs)
        {

            Vector2 mov;
            mov.X = x_tracker.Update(target.pos.X - inputs.pos.X);
            mov.Y = y_tracker.Update(target.pos.Y - inputs.pos.Y);

            //float a_mov = Utility.Angle(mov); // Utility.Angle( target.pos );
            //float a_mov = angle_tracker.Update( Utility.AngleDelta( Utility.Angle(target.pos - inputs.pos)  , inputs.angle) );

            // Ya, this version is way cooler.
            float a_mov = angle_tracker.Update( Utility.AngleDelta( Utility.Angle(mov), inputs.angle) );

            mov = Utility.Rotate(mov, -inputs.angle);
            
            outputs.control_thrust.X = Utility.Clamp( mov.X );
            outputs.control_thrust.Y = Utility.Clamp( mov.Y );
            outputs.control_torque = Utility.Clamp(a_mov);

            return outputs;
        }
    }
}





