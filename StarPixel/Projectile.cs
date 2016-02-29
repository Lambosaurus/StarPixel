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

        public ArtExplosionResource explosion_resource;

        public float explosion_size;

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

        public bool Hits(Physical phys)
        {
            if (phys != parent)
            {
                if (phys.hitbox.Contains(pos))
                {

                    return true;
                }
            }
            return false;
        }

        public void Explode(Universe universe, Physical phys)
        {
            Vector2 normal = phys.hitbox.SurfaceNormal(pos);

            Vector2 relative_velocity = velocity - phys.velocity;

            /*
            relative_velocity = Utility.Rotate(relative_velocity, -normal);
            relative_velocity.X *= -1;
            relative_velocity = Utility.Rotate(relative_velocity, normal);
            */

            relative_velocity = Vector2.Reflect(relative_velocity, normal);

            universe.art_temp.Add(explosion_resource.New(explosion_size, pos, phys.velocity, relative_velocity));

            this.Destory();
        }
    }

}
