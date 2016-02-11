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
    public class Component
    {
        // do i have component parent classes?
        // I dunno. It gets messy, but i feel like i need them to abstract out component generation
        public float max_hp = 100;
        public float hp;
        public float mass;
        int max_size;
        public float max_usage;
        public float usage;

        public bool destroyed;

        public Ship ship;

        public Component( Ship arg_ship )
        {
            destroyed = false;
            hp = max_hp;

            ship = arg_ship;
        }

        public virtual void Damage(float damage)
        {
            hp -= damage;
            
            if (hp <= 0)
            {
                if (!destroyed)
                {
                    destroyed = true;
                    Destroy();
                }
            }
        }

        public virtual void Update()
        {
        }

        public virtual void Destroy()
        {
        }
        public virtual void CalculateUsage()
        {
    }
    
    }



    public class Thrusters : Component
    {
        public float main_thrust = 4;
        public float manouvering_thrust = 2;
        public float torque = 0.5f;

        public Vector2 control_thrust_vector;
        public float control_torque_scalar;
        public float thrust_temperature = 5000;

        public List<ThrusterPort> particle_ports = new List<ThrusterPort>();
        public List<ArtVent> particle_vents = new List<ArtVent>();

        ArtVent sparkles;
        
        public float efficiency;
        
        public Thrusters(Ship ship) : base(ship)
        {
            control_thrust_vector = new Vector2(0, 0);
            control_torque_scalar = 0;

            efficiency = 1.0f;

            sparkles = ArtManager.NewArtVent("sparkles", 0.75f);

            foreach( ThrusterPort port in ship.template.thruster_ports )
            {
                particle_ports.Add(new ThrusterPort(port));
                particle_vents.Add(ArtManager.NewArtVent("TODO", port.size));
            }


            max_usage = 4.0f; // TODO: This is very wrong.
            usage = 0.0f;
        }

        public override void CalculateUsage()
        {
            // TODO: This calcualtes the total thrust output. Not quite the usage.
            // perhaps this works if max_usage is equal to the sum of the thrusts and torques.
            usage = ((control_thrust_vector.X > 0.0f) ? (control_thrust_vector.X * main_thrust) : (control_thrust_vector.X * manouvering_thrust)) +
                (control_thrust_vector.Y * manouvering_thrust) +
                (control_torque_scalar * manouvering_thrust);
        }

        public override void Update()
        {
            float control_x = Utility.Clamp(control_thrust_vector.X);
            float control_y = Utility.Clamp(control_thrust_vector.Y);

            float control_t = Utility.Clamp(control_torque_scalar);




            float output_torque = control_t * torque;
            float output_thrust_y = control_y * manouvering_thrust;
            float output_thrust_x = control_x * ((control_x > 0) ? main_thrust : manouvering_thrust);


            int i = 0;
            foreach (ThrusterPort port in particle_ports)
            {
                float strength = (port.kx * control_x) + (port.ky * control_y) + (port.kt * control_t);
                if (strength > 0)
                {
                    particle_vents[i].Generate(ship.pos + Utility.Rotate(port.position, ship.angle), ship.velocity, port.angle + ship.angle, strength);
                }
                particle_vents[i].Update();

                i++;
            }
            
            
            // TODO. this is nihilistic and therefore: not practical.
            if( control_x > 0 && Utility.Randf(1.0f) > 0.94f / control_x ) {
                sparkles.Generate(ship.pos + Utility.Rotate(particle_ports[0].position, ship.angle), ship.velocity, ship.angle + MathHelper.Pi, 1);
            }
            sparkles.Update();

            // out thrust vector we have calculated needs to be rotated by the ships angle.
            ship.Push( Utility.Rotate( new Vector2(output_thrust_x, output_thrust_y), ship.angle ) , output_torque);
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



    enum ReactorType
    {
        URANIUM, THORIUM, PLUTONIUM
    }


    public class Reactor : Component
    {
        float Max_output;
        float fuel_efficiency;
        ReactorType fuel_type;
        float power_usage;
        List<Component> connected;

        public Reactor(Ship ship) : base(ship)
        {
            Max_output = 3.0f;
            connected.Add(ship.thrusters);
        }

        public override void Update()
        {
            foreach (Component comp in connected)
            {
                
            }
        }
    }

}




