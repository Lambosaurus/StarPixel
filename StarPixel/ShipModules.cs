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
        public float efficiency;

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
    }
    


    public struct ThrusterNozzle
    {
        public Vector2 position;
        public float particle_gen;
        public Vector2 velocity;

        public ThrusterNozzle( Vector2 pos, Vector2 vel )
        {
            position = pos;
            velocity = vel;
            particle_gen = 0.0f;
        }
    }


    public class Thrusters : Component
    {
        public float main_thrust = 4;
        public float manouvering_thrust = 1;

        public Vector2 control_thrust_vector;
        public float control_torque_scalar;
        
        public enum PortDirections { Rear, Front, LeftRear, RightRear, LeftFront, RightFront };
        public ThrusterNozzle[] nozzles = new ThrusterNozzle[6];

        ArtThermoparticle particles;
        
        public Thrusters(Ship ship) : base(ship)
        {
            control_thrust_vector = new Vector2(0, 0);
            control_torque_scalar = 0;

            particles = ArtManager.NewArtThermoparticle("lol");
            efficiency = 1.0f;



            nozzles[(int)PortDirections.Rear] = new ThrusterNozzle(new Vector2(0, -10), new Vector2(0, -1));
            nozzles[(int)PortDirections.Front] = new ThrusterNozzle(new Vector2(0, 10), new Vector2(0, 1));

            nozzles[(int)PortDirections.LeftFront] = new ThrusterNozzle(new Vector2(-4, 8), new Vector2(-0.5f, 0));
            nozzles[(int)PortDirections.LeftRear] = new ThrusterNozzle(new Vector2(-4, -8), new Vector2(-0.5f, 0));
            nozzles[(int)PortDirections.RightFront] = new ThrusterNozzle(new Vector2(4, 8), new Vector2(0.5f, 0));
            nozzles[(int)PortDirections.RightRear] = new ThrusterNozzle(new Vector2(4, -8), new Vector2(0.5f, 0));
        }

        public override void Update()
        {
            float control_x = Utility.Clamp(control_thrust_vector.X);
            float control_y = Utility.Clamp(control_thrust_vector.Y);

            float control_t = Utility.Clamp(control_torque_scalar);


            // Torque and thrust use all use the side thrusters, so they conflict a little.
            float side_thruster_sum = Utility.Abs(control_x) + Utility.Abs(control_t);
            if (side_thruster_sum > 1.0f )
            {
                // we set them in proportion to their ratio
                control_t = control_t / (side_thruster_sum);
                control_x = control_x / (side_thruster_sum);

                // in the simplest case, if control_x is full and torque is full, then they both get reduced to half.
            }
            
            float output_torque = control_t * manouvering_thrust;
            float output_thrust_x = control_x * manouvering_thrust;
            float output_thrust_y = control_y * ((control_y > 0) ? main_thrust : manouvering_thrust);


            nozzles[(int)PortDirections.Rear].particle_gen += (control_y > 0) ? control_y*0.5f : 0;
            nozzles[(int)PortDirections.Front].particle_gen += (control_y < 0) ? -control_y*0.125f : 0;

            nozzles[(int)PortDirections.LeftRear].particle_gen += ((control_x - control_t > 0) ? (control_x - control_t) * 0.125f : 0);
            nozzles[(int)PortDirections.LeftFront].particle_gen += ((control_x + control_t > 0) ? (control_x + control_t) * 0.125f : 0);

            nozzles[(int)PortDirections.RightFront].particle_gen += ((-control_x - control_t > 0) ? (-control_x - control_t) * 0.125f : 0);
            nozzles[(int)PortDirections.RightRear].particle_gen += ((-control_x + control_t > 0) ? (-control_x + control_t) * 0.125f : 0);

            for ( int i = 0; i < 6; i++ )
            {
                while (nozzles[i].particle_gen > 1)
                {
                    nozzles[i].particle_gen--;
                    particles.Add(ship.pos + Utility.Rotate(nozzles[i].position, ship.angle), ship.velocity + Utility.Rotate(nozzles[i].velocity + Utility.Rand(0.1f), ship.angle), 3000);
                }
            }
            

            particles.Update();

            // out thrust vector we have calculated needs to be rotated by the ships angle.
            ship.Push( Utility.Rotate( new Vector2(output_thrust_x, output_thrust_y), ship.angle ) , output_torque);
        }

        public void Draw(Camera camera)
        {
            particles.Draw(camera);
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

        public Reactor(Ship ship) : base(ship)
        {

        }

        public override void Update()
        {
        }
    }

}




