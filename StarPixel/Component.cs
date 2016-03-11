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
    public class ComponentTemplate
    {
        public float hp = 100;
        public float thermal_capacity = 100;
    }

    public class Component
    {
        // do i have component parent classes?
        // I dunno. It gets messy, but i feel like i need them to abstract out component generation
        public float max_hp { get; private set; }
        public float hp { get; private set; }
        public float mass { get; private set; }

        public float size { get; private set; }

        public bool destroyed { get; private set; }

        public Ship ship { get; private set; }

        public ComponentTemplate base_template { get; private set; }

        public Component( Ship arg_ship, float arg_size, ComponentTemplate arg_base_template)
        {
            ship = arg_ship;

            base_template = arg_base_template;
            size = arg_size;

            destroyed = false;

            max_hp = base_template.hp * size;
            hp = max_hp;

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


    /*
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
    */

}




