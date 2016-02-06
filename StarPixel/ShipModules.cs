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
    

    public class Thrusters : Component
    {
        public float main_thrust = 4;
        public float manouvering_thrust = 1;

        public Vector2 control_thrust_vector;
        public float control_torque_scalar;

        public Thrusters(Ship ship) : base(ship)
        {
            control_thrust_vector = new Vector2(0, 0);
            control_torque_scalar = 0;
            efficiency = 1.0f;
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

            // out thrust vector we have calculated needs to be rotated by the ships angle.
            ship.Push( Utility.Rotate( new Vector2(output_thrust_x, output_thrust_y), ship.angle ) , output_torque);
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




