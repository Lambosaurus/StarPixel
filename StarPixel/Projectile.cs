using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;




namespace StarPixel
{
    public class Projectile : Entity
    {
        public int life;

        public ArtSprite sprite;

        public Ship parent;

        public Explosion explosion;
        
        public override void Update()
        {
            if (life-- <= 0)
            {
                this.Destory();
            }

            base.Update();

            sprite.Update(pos, angle);
        }

        public override void Draw(Camera camera)
        {
            sprite.Draw(camera);
        }

        public bool HitCheck(Universe universe, Physical phys)
        {
            if (phys != parent)
            {
                Vector2 bounce;

                if ( phys.shield != null && phys.shield.active)
                {
                    ComponentShield shield = phys.shield;

                    Intersection sect = shield.hitbox.Intersect(pos);
                    if ( sect != null )
                    {
                        bounce = Utility.Bounce(velocity - phys.velocity, sect.surface_normal);
                        bounce.Normalize();

                        explosion.Explode(universe, pos, phys.velocity, bounce);
                        shield.BlockDamage(explosion.dmg, pos);
                        this.Destory();
                        return true;
                    }
                }
                else
                {
                    Intersection sect = phys.hitbox.Intersect(pos);
                    if (sect != null)
                    {
                        bounce = Utility.Bounce(velocity - phys.velocity, sect.surface_normal);
                        bounce.Normalize();

                        explosion.Explode(universe, pos, phys.velocity, bounce);
                        phys.AdsorbExplosion(explosion, pos);
                        this.Destory();
                        return true;
                    }
                }    
            }
            return false;
        }

    }

}
