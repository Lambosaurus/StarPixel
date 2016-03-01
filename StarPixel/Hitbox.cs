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

        //public float radius;
        public float radius_sq;

        public Hitbox()
        {
        }

        // this is a very cheap way of excluding more complex tests
        public bool ContainsExclusion(Vector2 point)
        {
            return Vector2.DistanceSquared(point, pos) > (radius_sq);
        }

        // determines if a point lies within the hitbox.
        // This will NOT deal well with high speed particles.
        // I will proceed to use this for high speed particles untill its a problem.
        public virtual bool Contains( Vector2 point )
        {
            return false;
        }

        /*
        public bool ContainsMulti( Vector2 point, Vector2 velocity )
        {
            float new_exclusion_radius_sq = velocity.Length() + radius;
            new_exclusion_radius_sq *= new_exclusion_radius_sq;

            Vector2 step = velocity / 2;

            if (Vector2.DistanceSquared(point, pos) > new_exclusion_radius_sq) { return false; }

            if ( this.Contains(point - step) )
            {
                return true;
            }
            else if (this.Contains(point))
            {
                return true;
            }
            else if ( this.Contains(point + step))
            {
                return true;
            }

            return false;
        }
        */

        public virtual void Update( Vector2 arg_pos, float arg_angle )
        {
            pos = arg_pos;
            angle = arg_angle;
        }

        public virtual Hitbox Copy()
        {
            return null;
        }

        // Warning. Surface normals not guaranteed to have a normal length
        public virtual float SurfaceNormal(Vector2 point)
        {
            return 0.0f;
        }

        public virtual void Draw(Camera camera)
        {

        }
    }


    public class HitboxCircle : Hitbox
    {
        public float radius;

        public HitboxCircle(float arg_radius)
        {
            radius = arg_radius;
            radius_sq = radius * radius;
        }

        public override bool Contains(Vector2 point)
        {
            return !this.ContainsExclusion(point);
        }
        
        public override Hitbox Copy()
        {
            return new HitboxCircle(radius);
        }

        public override float SurfaceNormal( Vector2 point )
        {
            return Utility.Angle(point - pos);
        }
    }


    public class HitboxPolygon : Hitbox
    {
        public Vector2[] corners;
        public int count;

        public HitboxPolygon(Vector2[] arg_corners)
        {

            corners = arg_corners;
            count = arg_corners.Count();

            for (int i = 0; i < count; i++)
            {
                float dist_sq = corners[i].LengthSquared();
                if (dist_sq > radius_sq)
                {
                    radius_sq = dist_sq;
                }
            }
            //radius = Utility.Sqrt(radius_sq);
        }

        public override Hitbox Copy()
        {
            return new HitboxPolygon(corners);
        }

        public override bool Contains(Vector2 point)
        {
            // exclusion test because im not an animal
            if (this.ContainsExclusion(point)) { return false; }

            // first shift the point into the reference frame of the hitbox
            point = Utility.Rotate(point - pos, -angle);



            // https://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
            // http://cristgaming.com/pirate
            // this is some efficient shit man, look at this.

            int i, j, nvert = count;
            bool c = false;

            for (i = 0, j = nvert - 1; i < nvert; j = i++)
            {
                if (((corners[i].Y >= point.Y) != (corners[j].Y >= point.Y)) &&
                    (point.X <= (corners[j].X - corners[i].X) * (point.Y - corners[i].Y) / (corners[j].Y - corners[i].Y) + corners[i].X)
                  )
                    c = !c;
            }

            return c;
        }

        public override float SurfaceNormal(Vector2 point)
        {
            // first shift the point into the reference frame of the hitbox
            point = Utility.Rotate(point - pos, -angle);

            float min_dist = float.MaxValue;
            int best_segment = 0;

            // look through every segment, finding the one closest to the point
            for (int i = 0; i < count; i++)
            {
                Vector2 p1 = corners[i];
                Vector2 p2 = (i == count-1) ? corners[0] : corners[i+1];
                Vector2 pd = p2 - p1;

                float t = Utility.Clamp( Vector2.Dot( point - p1, pd) / pd.LengthSquared() , 0.0f, 1.0f );
                Vector2 projection = p1 + (t * pd);

                float dist_sq = Vector2.DistanceSquared(projection, point);

                if (dist_sq < min_dist)
                {
                    min_dist = dist_sq;
                    best_segment = i;
                }
            }

            // now get the angle of the segment
            Vector2 segment_p1 = corners[best_segment];
            Vector2 segment_p2 = (best_segment == count - 1) ? corners[0] : corners[best_segment + 1];
            float segment_angle = Utility.Angle(segment_p2 - segment_p1);

            // return the normal, and compensate for the hitbox rotation
            return segment_angle + angle - MathHelper.PiOver2;
            
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
                Vector2 stretch = new Vector2(Vector2.Distance(p1, p2), 4);
                float angle2 = Utility.Angle(p1 - p2);
                camera.batch.Draw(ArtManager.pixel, center, null, Color.Red, angle2, new Vector2(0.5f, 0.5f), stretch, SpriteEffects.None, 0);

                p1 = p2;
            }
        }
    }
}
