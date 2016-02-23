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

        public float size;

        public float max_usage;
        public float usage;

        public bool destroyed;

        public Ship ship;

        public Component( Ship arg_ship, float arg_size )
        {
            size = arg_size;

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

        public Reactor(Ship ship, float arg_size) : base(ship, arg_size)
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




