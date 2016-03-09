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
    public class ShieldTemplate
    {
        public string art_resource;


        public ComponentShield New( Ship ship )
        {
            ComponentShield shield = new ComponentShield(ship, ship.template.component_shield_size, this);

            return shield;
        }
    }

    public class ComponentShield : Component
    {
        ShieldTemplate template;

        public ArtShield art;

        public Hitbox hitbox;

        public float regen_rate = 0.05f;
        public bool active = false;
        public float integrity = 0f;
        public float max_integrity = 100f;

        public float radius;
        

        public ComponentShield( Ship arg_ship, float arg_size, ShieldTemplate arg_template): base(arg_ship, arg_size)
        {
            template = arg_template;

            radius = arg_ship.template.shield_radius;

            hitbox = new HitboxCircle(radius);
            art = ArtManager.shields[template.art_resource].New(radius, size);


            active = true;
        }

        public void AdsorbDamage(Damage dmg, Vector2 arg_pos)
        {
            integrity -= 1; // quality code

            art.Ping(arg_pos);
        }

        public override void Update()
        {
            hitbox.Update(ship.pos, ship.angle);

            art.Update(ship.pos);
            
            base.Update();

            if (active) { integrity += regen_rate; }
            if ( integrity < 0 ) { active = false; }

            integrity = Utility.Clamp(integrity, 0.0f, max_integrity);
        }

        public void Draw(Camera camera)
        {
            art.Draw(camera);
        }
    }
}
