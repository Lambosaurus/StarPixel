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
    public class Ship : Physical
    {

        ArtSprite hull_sprite;
        
        public Intellegence ai;

        public Thrusters thrusters;

        private bool selected = false;
        

        public IntInputs ai_inputs = new IntInputs();
        

        public Ship() : base()
        {
            mass = 200;
            inertia = 1000;

            hull_sprite = ArtManager.NewArtSprite("ship");
            
            thrusters = new Thrusters(this);

        }

        public override void Update()
        {
            
            if (ai != null)
            {
                ai_inputs.pos = pos;
                ai_inputs.angle = angle;
                IntOutputs orders = ai.Process(ai_inputs);

                thrusters.control_thrust_vector = orders.control_thrust;
                thrusters.control_torque_scalar = orders.control_torque;
                
            }

            
            thrusters.Update();
            
            base.Update();

            hull_sprite.pos = pos;
            hull_sprite.angle = angle;

            if (selected)
            {
                hull_sprite.color = Color.CadetBlue;
            }
            else
            {
                hull_sprite.color = Color.White;
            }
        }

        public override void Draw(Camera camera)
        {
            thrusters.Draw(camera);
            hull_sprite.Draw(camera);
            
        }
    }
}
