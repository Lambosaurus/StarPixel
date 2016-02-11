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
    public class ThrusterPort
    {
        public Vector2 position;
        public float angle;
        
        public float kx;
        public float ky;
        public float kt;

        public float size;

        public ThrusterPort(Vector2 arg_pos, float arg_angle, float arg_size, float x_response, float y_response, float t_response)
        {
            size = arg_size;

            position = arg_pos;
            angle = arg_angle;
            
            kx = x_response;
            ky = y_response;
            kt = t_response;
        }

        public ThrusterPort(ThrusterPort example )
        {
            position = example.position;
            angle = example.angle;

            kx = example.kx;
            ky = example.ky;
            kt = example.kt;
        }
    }



    public class ShipTemplate
    {
        public string hull_art_resource;

        public float base_mass;
        public float base_intertia;


        public List<ThrusterPort> thruster_ports = new List<ThrusterPort>();


        public Ship New()
        {
            Ship ship = new Ship(this);
            return ship;
        }

        public void AddPort(Vector2 arg_pos, float arg_angle, float arg_size, float x_response, float y_response, float t_response)
        {
            thruster_ports.Add(new ThrusterPort(arg_pos, arg_angle, arg_size, x_response, y_response, t_response));
        }
    }





    public class Ship : Physical
    {
        public ShipTemplate template;
        public ArtSprite hull_sprite;
        
        
        public Thrusters thrusters;

        
        public Intellegence ai;
        public IntInputs ai_inputs = new IntInputs();


        private bool selected = false;


        public Ship(ShipTemplate arg_template ) : base()
        {
            template = arg_template;

            mass = template.base_mass;
            inertia = template.base_intertia;

            hull_sprite = ArtManager.NewArtSprite( template.hull_art_resource );
            
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
