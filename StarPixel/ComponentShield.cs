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
    public class ShieldTemplate : ComponentTemplate
    {
        public string art_resource;

        public Resistance shield_resistance = Resistance.Zero;

        public float integrity = 120.0f;
        public float reform_integrity = 0.5f; // this is a percentage of maximum
        public float regen = 3f;
        
        public ShieldTemplate()
        {
            symbol = Symbols.Component.Shield;
        }

        public ComponentShield New( Ship ship )
        {
            ComponentShield shield = new ComponentShield(ship, ship.template.component_shield_size, this);
            
            return shield;
        }
    }

    public class ComponentShield : Component
    {
        public static Resistance SHIELD_BASE_RESISTANCE = new Resistance(0, -0.15f, 0, 0.7f);

        ShieldTemplate template;

        public ArtShield art;

        public Hitbox hitbox;

        public float regen_rate;
        public bool active;
        public float integrity;
        public float reform_integrity;
        public float max_integrity;

        public float radius;

        public Resistance shield_resistance;

        public ComponentShield( Ship arg_ship, float arg_size, ShieldTemplate arg_template): base(arg_ship, arg_ship.template.component_shield_pos, arg_size, arg_template)
        {
            template = arg_template;

            radius = arg_ship.template.shield_radius;

            hitbox = new HitboxCircle(radius);
            art = ArtManager.shields[template.art_resource].New(radius, size);

            shield_resistance = template.shield_resistance * SHIELD_BASE_RESISTANCE;

            max_integrity = (arg_template.integrity * arg_size * Utility.Sqrt(arg_size));
            regen_rate = (arg_template.regen / GameConst.framerate) * arg_size;
            integrity = max_integrity;
            reform_integrity = max_integrity * template.reform_integrity;
            active = true;
        }

        public void BlockDamage(Damage dmg, Vector2 arg_pos)
        {
            float total_dmg = shield_resistance.EvaluateDamage(dmg);
            integrity -= total_dmg;


            art.Ping(arg_pos, total_dmg );

            if (integrity < 0)
            {
                this.Pop();
            }
        }

        public void Pop()
        {
            active = false;
            ship.universe.AddArtTemp(art.Pop(ship.velocity));
        }

        public void Reform()
        {
            active = true;
            art.Reform();
        }

        public override void Update()
        {
            base.Update();

            hitbox.Update(ship.pos, ship.angle);

            if (!destroyed)
            {
                integrity += regen_rate;

                if (!active && integrity > reform_integrity)
                {
                    this.Reform();
                }
                
                integrity = Utility.Clamp(integrity, 0.0f, max_integrity);
            }

            art.Update(ship.pos);
        }

        public override void Destroy()
        {
            integrity = 0.0f;
            if (active) { this.Pop(); }

            base.Destroy();
        }

        public void Draw(Camera camera)
        {
            art.Draw(camera);
        }
    }
}
