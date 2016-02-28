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

        public Hitbox()
        {
        }

        public virtual bool Contains( Vector2 point )
        {
            return false;
        }

        public virtual Hitbox Copy()
        {
            return null;
        }

        public virtual float SurfaceNormal(Vector2 point)
        {
            return 0.0f;
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
            return (pos - point).LengthSquared() < (radius_squared);
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
}
