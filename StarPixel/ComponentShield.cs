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

        public Resistance resistance = Resistance.Zero;

        public float integrity = 120.0f;
        public float reform_integrity = 0.3f; // this is a percentage of maximum
        public float regen = 3f;

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

        public Resistance resistance;

        public ComponentShield( Ship arg_ship, float arg_size, ShieldTemplate arg_template): base(arg_ship, arg_size, arg_template)
        {
            template = arg_template;

            radius = arg_ship.template.shield_radius;

            hitbox = new HitboxCircle(radius);
            art = ArtManager.shields[template.art_resource].New(radius, size);

            resistance = template.resistance * SHIELD_BASE_RESISTANCE;

            max_integrity = (arg_template.integrity * arg_size * Utility.Sqrt(arg_size));
            regen_rate = (arg_template.regen / GameConst.framerate) * arg_size;
            integrity = max_integrity;
            reform_integrity = max_integrity * template.reform_integrity;
            active = true;
        }

        public void AdsorbExplosion(Explosion exp, Vector2 arg_pos)
        {
            float total_dmg = resistance.EvaluateDamage(exp.dmg);
            integrity -= total_dmg;


            art.Ping(arg_pos, total_dmg);

            if (integrity < 0)
            {
                this.Pop();
            }
        }

        public void Pop()
        {
            active = false;
            ship.universe.art_temp.Add(art.Pop(ship.velocity));
        }

        public void Reform()
        {
            active = true;
            art.Reform();
        }

        public override void Update()
        {
            hitbox.Update(ship.pos, ship.angle);
            
            base.Update();

            integrity += regen_rate;

            if (!active && integrity > reform_integrity)
            {
                this.Reform();
            }

            art.Update(ship.pos);

            integrity = Utility.Clamp(integrity, 0.0f, max_integrity);
        }

        public void Draw(Camera camera)
        {
            art.Draw(camera);
        }
    }
}
