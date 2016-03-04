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
    public class Projectile : Entity
    {
        public int life;

        public ArtSprite sprite;

        public Ship parent;

        public ArtExplosionResource explosion_resource;

        public float explosion_size;

        public float penetration;

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
                ComponentShield shield = phys.GetActiveShield();
                if ( shield != null)
                {
                    if ( shield.hitbox.Contains(pos) )
                    {

                        Vector2 bounce = this.CalcBounceAngle(phys.velocity, shield.hitbox);


                        universe.art_temp.Add(explosion_resource.New(explosion_size, pos, phys.velocity, bounce));

                        shield.ExternalDamage(null, pos);

                        this.Destory();

                        return true;
                    }
                }
                else if (phys.hitbox.Contains(pos))
                {
                    Vector2 bounce = this.CalcBounceAngle(phys.velocity, phys.hitbox);
                    universe.art_temp.Add(explosion_resource.New(explosion_size, pos, phys.velocity, bounce));

                    phys.Damage(null, pos);

                    this.Destory();

                    return true;
                }
            }
            return false;
        }

        Vector2 CalcBounceAngle( Vector2 surface_velocity, Hitbox surface_hitbox )
        {
            Vector2 relative_velocity = velocity - surface_velocity;

            float normal_angle = surface_hitbox.SurfaceNormal(pos - relative_velocity);            


            relative_velocity = Utility.Rotate(relative_velocity, -normal_angle);
            relative_velocity.X *= -1;
            relative_velocity = Utility.Rotate(relative_velocity, normal_angle);

            return relative_velocity;
        }
    }

}
