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
    public class Hitbox
    {

        public Vector2 pos;
        public float angle;

        public Hitbox()
        {
        }

        public virtual bool Contains( Vector2 point )
        {
            return false;
        }

        public virtual void Update( Vector2 arg_pos, float arg_angle )
        {
            pos = arg_pos;
            angle = arg_angle;
        }

        public virtual Hitbox Copy()
        {
            return null;
        }

        public virtual Vector2 SurfaceNormal(Vector2 point)
        {
            return Vector2.UnitX;
        }

        public virtual void Draw(Camera camera)
        {

        }
    }


    public class HitboxCircle : Hitbox
    {
        float radius;
        float radius_squared;

        public HitboxCircle(float arg_radius)
        {
            radius = arg_radius;
            radius_squared = radius * radius;
        }

        public override bool Contains(Vector2 point)
        {
            // we do this test with radius squared, because its faster than
            // the square root involved in gettign a normal length
            return Vector2.DistanceSquared(point, pos) < (radius_squared);
        }
        
        public override Hitbox Copy()
        {
            return new HitboxCircle(radius);
        }

        public override Vector2 SurfaceNormal( Vector2 point )
        {
            return point - pos;
        }
    }


    public class HitboxPolygon : Hitbox
    {
        public Vector2[] corners;
        public float count;

        public HitboxPolygon(Vector2[] arg_corners)
        {
            corners = arg_corners;
            count = arg_corners.Count();
        }

        public override Hitbox Copy()
        {
            return new HitboxPolygon(corners);
        }

        public override void Draw(Camera camera)
        {
            Vector2 p1 = Utility.Rotate( corners[0], angle) + pos;
            p1 = camera.Map(p1);
            Vector2 p2;

            for (int j = 0; j < count; j++)
            {
                int i = j + 1;
                if ( i == count ) { i = 0; }

                p2 = Utility.Rotate(corners[i], angle) + pos;

                p2 = camera.Map(p2);
                Vector2 center = (p1 + p2) / 2;
                Vector2 stretch = new Vector2(Vector2.Distance(p1, p2), 6);
                float angle2 = Utility.Angle(p1 - p2);
                camera.batch.Draw(ArtManager.pixel, center, null, Color.Red, angle2, new Vector2(0.5f, 0.5f), stretch, SpriteEffects.None, 0);

                p1 = p2;
            }



        }
    }
}
