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

    public class WeaponPort
    {
        public Vector2 position;

        public float angle_min;
        public float angle_max;

        public float size;

        public WeaponPort(Vector2 arg_pos, float arg_size, float arc_center, float arc_width)
        {
            position = arg_pos;
            size = arg_size;

            angle_min = arc_center - (arc_width / 2);
            angle_max = arc_center + (arc_width / 2);
        }

        public WeaponPort( WeaponPort example )
        {
            size = example.size;
            position = example.position;
            angle_min = example.angle_min;
            angle_max = example.angle_max;
        }
    }



    public class WeaponTemplate
    {
        public WeaponTemplate()
        {
        }

        public virtual ComponentWeapon New(Ship arg_ship, WeaponPort arg_port)
        {
            return null;
        }

    }



    public class ComponentWeapon : Component
    {

        WeaponPort port;

        public ComponentWeapon(Ship arg_ship, WeaponPort arg_port) : base(arg_ship, arg_port.size )
        {
            port = arg_port;
        }

        public virtual void Fire(Universe universe, float angle )
        {

        }
    }



    public class ProjectileSlug : Projectile
    {
        
    }



    public class WeaponTemplateSlug : WeaponTemplate
    {

        Vector2 projectile_scale;

        float projectile_temperature;
        float projectile_life;
        float projectile_thermo_halflife;
        float projectile_velocity;

        public override ComponentWeapon New(Ship arg_ship, WeaponPort port)
        {
            return new ComponentWeaponSlug(arg_ship, this, port);
        }

    }


    public class ComponentWeaponSlug : ComponentWeapon
    {
        public WeaponTemplateSlug template;

        public ComponentWeaponSlug( Ship arg_ship, WeaponTemplateSlug arg_template, WeaponPort arg_port) : base(arg_ship, arg_port)
        {
            
            template = arg_template;
        }

        public override void Fire(Universe universe, float angle)
        {
            ProjectileSlug proj = new ProjectileSlug();
            
            base.Fire(universe, angle);
        }

    }

}

















