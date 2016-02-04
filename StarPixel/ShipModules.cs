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
    class Component
    {
        // do i have component parent classes?
        // I dunno. It gets messy, but i feel like i need them to abstract out component generation
        public float max_hp = 100;
        public float hp;

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


    class Thrusters : Component
    {
        public float main_thrust = 40;
        public float manouvering_thrust = 10;

        public Vector2 control_thrust_vector;
        public float control_torque_scalar;
        
        public Thrusters(Ship ship) : base(ship)
        {
        }

        public override void Update()
        {

        }
    }
}




