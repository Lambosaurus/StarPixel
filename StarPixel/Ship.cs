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
        
        private void SelectMe(object sender, EventArgs eventArgs)
        {
            selected = !selected;
        }

        public Ship() : base()
        {
            mass = 500;
            inertia = 2000;

            hull_sprite = ArtManager.NewArtSprite("ship");
            
            thrusters = new Thrusters(this);

            ai = new IntellegenceHuman();

            Clicked += SelectMe;
        }

        public override void Update()
        {
            
            if (ai != null)
            {
                IntOutputs orders = ai.Process(new IntInputs());

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
