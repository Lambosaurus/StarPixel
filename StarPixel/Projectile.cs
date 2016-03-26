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

                ComponentShield shield = phys.GetActiveShield();
                if ( shield != null)
                {
                    Intersection sect = shield.hitbox.Intersect(pos);
                    if ( sect != null )
                    {
                        bounce = this.CalcBounceAngle(phys.velocity, sect.surface_normal);
                        explosion.Explode(universe, pos, phys.velocity, bounce);
                        shield.AdsorbExplosion(explosion, pos);
                        this.Destory();
                        return true;
                    }
                }
                else
                {
                    Intersection sect = phys.hitbox.Intersect(pos);
                    if (sect != null)
                    {
                        bounce = this.CalcBounceAngle(phys.velocity, sect.surface_normal);
                        explosion.Explode(universe, pos, phys.velocity, bounce);
                        phys.AdsorbExplosion(explosion, pos);
                        this.Destory();
                        return true;
                    }
                }    
            }
            return false;
        }

        Vector2 CalcBounceAngle( Vector2 surface_velocity, float surface_normal )
        {
            Vector2 relative_velocity = velocity - surface_velocity;
            
            relative_velocity = Utility.Rotate(relative_velocity, -surface_normal);
            relative_velocity.X *= -1;
            relative_velocity = Utility.Rotate(relative_velocity, surface_normal);

            relative_velocity.Normalize();

            return relative_velocity;
        }
    }

}
