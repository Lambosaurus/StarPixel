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

        public ThrusterPort(ThrusterPort example)
        {
            size = example.size;

            position = example.position;
            angle = example.angle;

            kx = example.kx;
            ky = example.ky;
            kt = example.kt;
        }
    }





    public class ThrusterTemplate
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
    }


    public class Thruster : Component
    {
        // TODO: Add sparkles

        public float main_thrust;
        public float reverse_thrust;
        public float side_thrust;
        public float torque;


        public Vector2 control_thrust_vector;
        public float control_torque_scalar;
        

        public List<ThrusterPort> particle_ports;
        public List<ArtVent> particle_vents;

        ArtVent sparkles;

        public float efficiency;

        ThrusterTemplate template;

        public Thruster(Ship ship, float arg_size) : base(ship, arg_size)
        {
            control_thrust_vector = new Vector2(0, 0);
            control_torque_scalar = 0;

            efficiency = 1.0f;

            max_usage = 4.0f; // TODO: This is very wrong on multiple levels.
            usage = 0.0f;
        }

        public void ApplyTemplate(ThrusterTemplate arg_template)
        {
            // so all kinds of errors will occurr if a thruster is used without a template being applied
            template = arg_template;
            side_thrust = template.side_thrust * size;
            reverse_thrust = template.reverse_thrust * size;
            main_thrust = template.main_thrust * size;
            torque = template.torque * size;

            // this needs to be dynamic
            sparkles = ArtManager.NewArtVent(arg_template.sparkle_effects, 0.5f);

            particle_ports = new List<ThrusterPort>();
            particle_vents = new List<ArtVent>();

            foreach (ThrusterPort port in ship.template.thruster_ports)
            {
                particle_ports.Add(new ThrusterPort(port));
                ArtVent vent = ArtManager.NewArtVent(arg_template.particle_effects, port.size);
                particle_vents.Add(vent);
            }
        }

        public void ApplyTemplate(string template_name)
        {
            ApplyTemplate(AssetThrusterTemplates.thruster_templates[template_name]);
        }

        public override void CalculateUsage()
        {
            // TODO: This calcualtes the total thrust output. Not quite the usage.
            // perhaps this works if max_usage is equal to the sum of the thrusts and torques.
            usage = ((control_thrust_vector.X > 0.0f) ? (control_thrust_vector.X * main_thrust) : (control_thrust_vector.X * reverse_thrust)) +
                (control_thrust_vector.Y * side_thrust) +
                (control_torque_scalar * torque);
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
                strength *= 0.5f;
                if (strength > 0)
                {
                    particle_vents[i].Generate(ship.pos + Utility.Rotate(port.position, ship.angle), ship.velocity, port.angle + ship.angle, strength);
                }
                particle_vents[i].Update();

                i++;
            }


            // TODO. this is nihilistic and therefore: not practical.
            if (control_x > 0 && Utility.Randf(1.0f) > 0.94f / control_x)
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
