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
    public class HullThrusterPort
    {
        public Vector2 position;
        public float angle;
        
        public float kx;
        public float ky;
        public float kt;

        public HullThrusterPort(Vector2 arg_pos, float arg_angle, float x_response, float y_response, float t_response)
        {
            position = arg_pos;
            angle = arg_angle;
            
            kx = x_response;
            ky = y_response;
            kt = t_response;
        }
    }



    public class HullTemplate
    {
        public string hull_art_resource;

        public float base_mass;
        public float base_intertia;


        public List<HullThrusterPort> thruster_ports;


        public Ship New()
        {
            Ship ship = new Ship();
            return ship;
        }
    }





    public class Ship : Physical
    {

        public ArtSprite hull_sprite;
        
        public Intellegence ai;

        public Thrusters thrusters;

        private bool selected = false;
        

        public IntInputs ai_inputs = new IntInputs();
        

        public Ship() : base()
        {
            mass = 200;
            inertia = 800;

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

            /*
            if (selected)
            {
                hull_sprite.color = Color.CadetBlue;
            }
            else
            {
                hull_sprite.color = Color.White;
            }
            */
        }

        public override void Draw(Camera camera)
        {
            thrusters.Draw(camera);
            hull_sprite.Draw(camera);
            
        }
    }
}
