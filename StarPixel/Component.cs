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
    // This is the object that is exposed to the AI
    public class ComponentFacade
    {
        // standard components variables that can be read through the interface
        public float health { get { return component.health; } }
        public float max_health { get { return component.max_health; } }
        public float mass { get { return component.mass; } }
        public float size { get { return component.size; } }
        public float min_power { get { return component.min_power; } }
        public float max_power { get { return component.max_power; } }
        public bool destroyed { get { return component.destroyed; } }

        // writable variables, which the AI can mess with
        public float allocated_power = 0.0f;


        // we have a private ptr to the component, which should be safe
        private Component component;

        public ComponentFacade( Component arg_component )
        {
            component = arg_component;
        }
    }



    public class ComponentTemplate
    {
        public float health = 100;
        public float thermal_capacity = 100;

        public float min_power = 0.0f;
        public float max_power = 0.0f;
        
        public Resistance resistance = Resistance.Zero;
    }

    public class Component
    {
        // do i have component parent classes?
        // I dunno. It gets messy, but i feel like i need them to abstract out component generation
        public float max_health { get; private set; }
        public float health { get; private set; }
        public float mass { get; private set; }

        public float max_power { get; private set; }
        public float min_power { get; private set; }
    

        public float size { get; private set; }
        public bool destroyed { get; private set; }

        public Vector2 pos { get; private set; }

        public Ship ship { get; private set; }

        public ComponentTemplate base_template { get; private set; }

        public Component( Ship arg_ship, float arg_size, ComponentTemplate arg_base_template)
        {
            ship = arg_ship;

            base_template = arg_base_template;
            
            destroyed = false;

            max_health = base_template.health * size;
            size = arg_size;
            max_power = base_template.max_power * size;
            min_power = base_template.min_power * size;

            health = max_health;
            
        }

        public virtual Damage AdsorbDamage( Damage dmg )
        {
            float dmg_dealt = base_template.resistance.EvaluateDamage(dmg);

            if (health < dmg_dealt)
            {
                this.Destroy();
                Damage remaining = base_template.resistance.RemainingDamage(health, dmg);
                return remaining;
            }
            else
            {
                health -= dmg_dealt;
                return null;
            }
        }

        public virtual void Update()
        {
        }

        public virtual void Destroy()
        {
            health = 0.0f;
            destroyed = true;
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




