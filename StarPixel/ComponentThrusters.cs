﻿using System;
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
        public Vector2 position { private set; get; }
        public float angle { private set; get; }

        public float kx { private set; get; }
        public float ky { private set; get; }
        public float kt { private set; get; }

        public float size { private set; get; }

        public ThrusterPort(Vector2 arg_pos, float arg_angle, float arg_size, float x_response, float y_response, float t_response)
        {
            size = arg_size;

            position = arg_pos;
            angle = arg_angle;

            kx = x_response;
            ky = y_response;
            kt = t_response;
        }

        public ThrusterPort Copy()
        {
            return new ThrusterPort(position, angle, size, kx, ky, kt);
        }
    }





    public class ThrusterTemplate : ComponentTemplate
    {
        public float main_thrust;
        public float reverse_thrust;
        public float side_thrust;
        public float torque;

        public string particle_effects;
        public string sparkle_effects;

        public ThrusterTemplate()
        {
        }

        public Thruster New(Ship ship)
        {
            Thruster thruster = new Thruster(ship, ship.template.component_thruster_size, this);
            return thruster;
        }
    }


    public class Thruster : Component
    {
        // TODO: Add sparkles

        public float main_thrust { private set; get; }
        public float reverse_thrust { private set; get; }
        public float side_thrust { private set; get; }
        public float torque { private set; get; }


        public Vector2 control_thrust_vector = new Vector2(0,0);
        public float control_torque_scalar = 0;
        

        public List<ThrusterPort> particle_ports { private set; get; }
        public List<ArtVent> particle_vents { private set; get; }

        ArtVent sparkles;

        public ThrusterTemplate template { private set; get; }

        public float efficiency { private set; get; } = 1.0f;

        public Thruster(Ship ship, float arg_size, ThrusterTemplate arg_template) : base(ship, arg_size, arg_template)
        {

            template = arg_template;
            side_thrust = template.side_thrust * size;
            reverse_thrust = template.reverse_thrust * size;
            main_thrust = template.main_thrust * size;
            torque = template.torque * size;

            // this needs to be dynamic
            sparkles = ArtManager.GetVentResource(template.sparkle_effects).New(0.5f);


            // I was copying particle_ports from the template, when there is no need to. Just take a pointer.
            //particle_ports = new List<ThrusterPort>();
            particle_ports = ship.template.thruster_ports;
            particle_vents = new List<ArtVent>();

            foreach (ThrusterPort port in ship.template.thruster_ports)
            {
                //particle_ports.Add( port.Copy() );
                ArtVent vent = ArtManager.GetVentResource(template.particle_effects).New(port.size);
                particle_vents.Add(vent);
            }
        }


        public override void Update()
        {
            float control_x = Utility.Clamp(control_thrust_vector.X);
            float control_y = Utility.Clamp(control_thrust_vector.Y);

            float control_t = Utility.Clamp(control_torque_scalar);




            float output_torque = control_t * torque;
            float output_thrust_y = control_y * side_thrust;
            float output_thrust_x = control_x * ((control_x > 0) ? main_thrust : reverse_thrust);


            int i = 0;
            foreach (ThrusterPort port in particle_ports)
            {
                float strength = (port.kx * control_x) + (port.ky * control_y) + (port.kt * control_t) ;
                if (strength > 0)
                {
                    particle_vents[i].Generate(ship.pos + Utility.Rotate(port.position, ship.angle), ship.velocity, port.angle + ship.angle, strength);
                }
                particle_vents[i].Update();

                i++;
            }


            // TODO. this is nihilistic and therefore: not practical.
            if (control_x > 0 && Utility.Rand(1.0f) > 0.94f / control_x)
            {
                sparkles.Generate(ship.pos + Utility.Rotate(particle_ports[0].position, ship.angle), ship.velocity, ship.angle + MathHelper.Pi, 1);
            }
            sparkles.Update();

            // out thrust vector we have calculated needs to be rotated by the ships angle.
            ship.Push(Utility.Rotate(new Vector2(output_thrust_x, output_thrust_y), ship.angle), output_torque);
        }

        public void Draw(Camera camera)
        {
            sparkles.Draw(camera);

            foreach (ArtVent vent in particle_vents)
            {
                vent.Draw(camera);
            }

        }
    }
}
