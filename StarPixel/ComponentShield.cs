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
    public class ShieldTemplate
    {
        public string art_resource;


        public ComponentShield New( Ship ship )
        {
            ComponentShield shield = new ComponentShield(ship, ship.template.component_shield_size, this);

            shield.art = ArtManager.shields[art_resource].New();
            
            return shield;
        }
    }

    public class ComponentShield : Component
    {
        ShieldTemplate template;

        public ArtShield art;

        public Hitbox hitbox;

        public bool active;


        public ComponentShield( Ship arg_ship, float arg_size, ShieldTemplate arg_template): base(arg_ship, arg_size)
        {
            template = arg_template;

            hitbox = ship.template.shield_hitbox.Copy();
            

            active = true;
        }

        public void ExternalDamage(Damage dmg, Vector2 arg_pos)
        {
            art.Ping(arg_pos);
        }

        public override void Update()
        {
            hitbox.Update(ship.pos, ship.angle);

            art.Update(ship.pos);

            base.Update();
        }

        public void Draw(Camera camera)
        {
            art.Draw(camera);
        }
    }
}
