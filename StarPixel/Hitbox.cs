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
    // null means no intersection took place.
    // You retard.
    public class Intersection
    {
        public float surface_normal;
        public Vector2 position;
    }

    
    public class Hitbox
    {
        public Vector2 pos;
        public float angle;

        public float radius;
        public float radius_sq;

        public Hitbox()
        {
        }

        // this is a very cheap way of excluding more complex tests
        public bool WithinRadius(Vector2 point)
        {
            return Vector2.DistanceSquared(point, pos) < (radius_sq);
        }

        // as above
        public bool WithinRadius(Hitbox hitbox)
        {
            float combined_radsq = radius + hitbox.radius;
            combined_radsq *= combined_radsq;
            return Vector2.DistanceSquared(pos, hitbox.pos) < (combined_radsq);
        }

        // determines if a point lies within the hitbox.
        // This will NOT deal well with high speed particles.
        // I will proceed to use this for high speed particles untill its a problem.
        public virtual Intersection Intersect( Vector2 point )
        {
            return null;
        }

        public virtual Intersection Intersect(Hitbox hitbox)
        {
            return null;
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


        public virtual void Draw(Camera camera)
        {

        }
    }


    public class HitboxCircle : Hitbox
    {
        public HitboxCircle(float arg_radius)
        {
            radius = arg_radius;
            radius_sq = radius * radius;
        }

        public override Intersection Intersect(Vector2 point)
        {
            if (!this.WithinRadius(point) ) { return null; }

            // the exclusion test is the same as the collision test

            Intersection sect = new Intersection();
            sect.surface_normal = Utility.Angle(point - pos);
            sect.position = point;
            return sect;
        }
        
        public override Hitbox Copy()
        {
            return new HitboxCircle(radius);
        }


        public override Intersection Intersect(Hitbox hitbox)
        {
            if (!this.WithinRadius(hitbox)) { return null; }
            
            if (hitbox is HitboxCircle)
            {
                // if both hitboxes are circles, then the ContainsExclusion test
                // is the same as the collision test.
                // So a collision has occurred

                Intersection sect = new Intersection();

                sect.position = ((hitbox.pos * radius) + (pos * hitbox.radius)) / (radius + hitbox.radius);
                sect.surface_normal = Utility.Angle(pos - hitbox.pos);

                return sect;
            }
            
            if ( hitbox is HitboxPolygon )
            {
                // YEA. DO THAT.
                return ((HitboxPolygon)hitbox).IntersectCircle(this);
            }

            return null;
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
            radius = Utility.Sqrt(radius_sq);
        }

        public override Hitbox Copy()
        {
            return new HitboxPolygon(corners);
        }

        public override Intersection Intersect(Vector2 arg_point)
        {
            // exclusion test because im not an animal
            if (!this.WithinRadius(arg_point)) { return null; }

            // first shift the point into the reference frame of the hitbox
            Vector2 point = Utility.Rotate(arg_point - pos, -angle);

            
            // https://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
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
            
            if (!c) { return null; }

            Intersection sect = new Intersection();
            sect.position = arg_point;
            sect.surface_normal = this.SurfaceNormal(arg_point);
            return sect;
        }
        

        // Not totally stoaked with this implementation
        public float SurfaceNormal(Vector2 point)
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

        
        public Intersection IntersectCircle(HitboxCircle hitbox)
        {
            // first shift the circle origin into the reference frame of the hitbox
            Vector2 point = Utility.Rotate(hitbox.pos - pos, -angle);


            for (int i = 0; i < count; i++)
            {
                // for each point we test if it falls into the circle
                if (  Vector2.DistanceSquared(point, corners[i]) < hitbox.radius_sq )
                {
                    // If one point falls in, we run with it.
                    Intersection sect = new Intersection();

                    // we take the corner, and shift it into global coordinate frame
                    sect.position = pos + Utility.Rotate(corners[i], angle);

                    // the surface normal is calculated like it is for a circle
                    sect.surface_normal = Utility.Angle(pos - hitbox.pos);
                    return sect;
                }
            }

            return null;
        }

        // Returns the intersection of two HitboxPolygons
        public Intersection IntersectPolygon(HitboxPolygon hitbox)
        {
            // checks each point in the hitbox 
            for (int i = 0; i < count; i++)
            {
                // shift the point into global frame
                Vector2 point = pos + Utility.Rotate(corners[i], angle);

                // we then see to if any of the points lie inside this polygon
                Intersection sect = hitbox.Intersect(point);
                if (sect != null) { return sect; }
            }

            // repeat the above, but testing the points in the other polygon
            for (int i = 0; i < hitbox.count; i++)
            {
                Vector2 point = hitbox.pos + Utility.Rotate(hitbox.corners[i], hitbox.angle);

                Intersection sect = this.Intersect(point);
                if (sect != null)
                {
                    // the surface normal should be reversed, because it was calculated from the other hitbox
                    sect.surface_normal += MathHelper.TwoPi;
                    return sect;
                }
            }

            return null; // no intersection found
        }


        public override Intersection Intersect(Hitbox hitbox)
        {
            if (!this.WithinRadius(hitbox)) { return null; }

            if (hitbox is HitboxCircle)
            {
                return this.IntersectCircle((HitboxCircle)hitbox);
            }

            if (hitbox is HitboxPolygon)
            {
                return this.IntersectPolygon((HitboxPolygon)hitbox);
            }

            return null;
        }


        // draws the red outlines of the hitbox onto screen. Kinda usefull.
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

                ArtLine.DrawLineU(camera, p1, p2, Color.Red, 4);

                p1 = p2;
            }
        }
    }
}
