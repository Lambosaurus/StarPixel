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
    public class WeaponFacade : ComponentFacade
    {
        // standard variables of the weapon hardpoint
        public float hardpoint_size { get { return port.size; } }
        public Vector2 hardpoint_position { get { return port.position; } }
        public float hardpoint_arc_min { get { return port.angle_min; } }
        public float hardpoint_arc_max { get { return port.angle_max; } }
        public bool weapon_present { get { return component != null; } }

        // weapon related fields
        // dont read these unless you check that the weapon is present
        public float projectile_velocity { get { return component.projectile_velocity; } }
        public Damage projectile_damage { get { return component.explosion.dmg; } }
        public float projectile_life { get { return component.projectile_frame_life; } }
        public float projectile_scatter { get { return component.projectile_scatter; } }
        public float weapon_period { get { return component.cooldown; } }
        public bool ready_to_fire { get { return component.ReadyToFire(); } }
        
        // writable variables, which the AI can mess with
        public bool fire = false;
        public float target_angle = 0.0f;
        
        // we have a private ptr to the component, which should be safe
        private ComponentWeapon component;
        private WeaponPort port;

        public WeaponFacade( WeaponPort arg_port, ComponentWeapon arg_component ) : base(arg_component)
        {
            component = arg_component;
            port = arg_port;
        }
    }



    public class WeaponTemplate : ComponentTemplate
    {
        public string projectile_sprite_resource;
        public string projectile_explosion_resource;
        //public string weapon_sprite_resource;
        public Color projectile_color;

        public float projectile_velocity = 4;
        public float projectile_scatter = 0.04f;
        public Vector2 projectile_scale = new Vector2(1,1);
        public float projectile_range = 2000f;

        public float fire_rate = 1.0f;

        public Explosion explosion;

        public WeaponTemplate()
        {
        }

        public virtual ComponentWeapon New(Ship arg_ship, WeaponPort arg_port)
        {
            float size = arg_port.size;
            ComponentWeapon weapon = new ComponentWeapon(arg_ship, arg_port, size, this);

            weapon.cooldown = 60f / (fire_rate);
            weapon.projectile_velocity = projectile_velocity;
            weapon.projectile_frame_life = (int)(projectile_range * size / projectile_velocity);
            weapon.projectile_scatter = projectile_scatter;
            weapon.projectile_scale = projectile_scale * size;

            return weapon;
        }

    }



    public class ComponentWeapon : Component
    {

        public WeaponPort port;

        public WeaponTemplate template;

        // we use floats here, because then a CD of 1.5 will allow bonus shots  
        public float current_cooldown;
        public float cooldown;

        public float projectile_velocity;
        public float projectile_scatter;
        public int projectile_frame_life;
        public Vector2 projectile_scale;
        
        public Explosion explosion { get; private set; }

        public WeaponFacade facade { get; private set; }

        public ComponentWeapon(Ship arg_ship, WeaponPort arg_port, float arg_size, WeaponTemplate arg_template) : base(arg_ship, arg_size, arg_template)
        {
            port = arg_port;
            template = arg_template;

            explosion = template.explosion * size;

            facade = new WeaponFacade(port, this);
        }

        public bool ReadyToFire()
        {
            return current_cooldown <= 0 && !destroyed;
        }

        public virtual void Fire(float angle )
        {
            if (this.ReadyToFire())
            {
                facade.fire = false;

                if ( Utility.AngleWithin(angle, port.angle_min, port.angle_max) )
                {
                    this.SpawnProjectile(angle);

                    current_cooldown = cooldown;
                }
            }
        }

        public override void Update()
        {
            if (!destroyed)
            {
                if (current_cooldown > 0)
                {
                    current_cooldown--;
                }

                base.Update();


                if (facade.fire)
                {
                    this.Fire(facade.target_angle);
                }
            }
        }

        public virtual void SpawnProjectile(float angle )
        {
            Projectile projectile = new Projectile();

            float fire_angle = angle + ship.angle;

            projectile.angle = fire_angle;
            projectile.velocity = ship.velocity + Utility.CosSin(fire_angle, projectile_velocity) + Utility.RandVec(projectile_scatter);
            projectile.life = projectile_frame_life;
            projectile.pos = ship.pos + Utility.Rotate(port.position, ship.angle);

            projectile.sprite = ArtManager.GetSpriteResource(template.projectile_sprite_resource).New();
            projectile.sprite.color = template.projectile_color;
            projectile.sprite.scale = new Vector2(Utility.Sqrt(projectile_scale.X), Utility.Sqrt(projectile_scale.Y) );

            projectile.explosion = explosion;
            projectile.parent = ship;

            ship.universe.AddProjectile(projectile);
        }
    }

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

        public WeaponPort(WeaponPort example)
        {
            size = example.size;
            position = example.position;
            angle_min = example.angle_min;
            angle_max = example.angle_max;
        }
    }


}

















