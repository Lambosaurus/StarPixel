
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
    public static class Utility
    {
        // lol, 32KB right here. Ez.
        const int SINE_RESOLUTION = 4000;
        static float[] sin_values = new float[SINE_RESOLUTION+1];
        static float[] cos_values = new float[SINE_RESOLUTION+1];
        
        const float LOOKUP_CONSTANT = SINE_RESOLUTION / MathHelper.TwoPi; // or just (1 / rads step)


        public static float WrapAngle(float alpha)
        {
            alpha = alpha % MathHelper.TwoPi;
            return (alpha > 0) ? alpha : alpha + MathHelper.TwoPi;
        }

        // here i provide a fast cos and sin function
        // Its a simple lookup table. Its more convenient than doing (float)Math.Cos(rads)
        // And about twice as fast. possibly.
        public static float Cos(float alpha)
        {
            // Utility.WrapAngle(alpha) inline
            alpha = alpha % MathHelper.TwoPi;
            alpha = alpha > 0 ? alpha : alpha + MathHelper.TwoPi;

            return cos_values[ (int)(alpha * LOOKUP_CONSTANT)];
        }

        public static float Sin(float alpha)
        {
            // Utility.WrapAngle(alpha) inline
            alpha = alpha % MathHelper.TwoPi;
            alpha = alpha > 0 ? alpha : alpha + MathHelper.TwoPi;
            return sin_values[(int)(alpha * LOOKUP_CONSTANT)];
        }

        // maybe i should build an atan2 table.... nah..


        // rotates a vector by alpha rads, about its origin
        // This is the very lifeblood of my code.
        public static Vector2 Rotate(Vector2 point, float alpha)
        {
            // Utility.WrapAngle(alpha) inline
            alpha = alpha % MathHelper.TwoPi;
            alpha = alpha > 0 ? alpha : alpha + MathHelper.TwoPi;

            float c = cos_values[(int)(alpha * LOOKUP_CONSTANT)];
            float s = sin_values[(int)(alpha * LOOKUP_CONSTANT)];

            return new Vector2( (c * point.X) + (s * point.Y),
                                (s * point.X) - (c * point.Y) );

        }

        public static Vector2 CosSin(float alpha)
        {
            // Utility.WrapAngle(alpha) inline
            alpha = alpha % MathHelper.TwoPi;
            alpha = alpha > 0 ? alpha : alpha + MathHelper.TwoPi;

            float c = cos_values[(int)(alpha * LOOKUP_CONSTANT)];
            float s = sin_values[(int)(alpha * LOOKUP_CONSTANT)];
            return new Vector2(c, s);
        }

        static Utility()
        {
            // build the cos and sine tables used for Cos() and Sin()
            for ( int i = 0; i < (SINE_RESOLUTION+1); i++)
            {
                float angle = i * MathHelper.TwoPi / SINE_RESOLUTION;

                sin_values[i] = (float)Math.Sin(angle);
                cos_values[i] = (float)Math.Cos(angle);
            }
        }

        public static float Clamp(float value, float min = 1.0f, float max = 1.0f)
        {
            if (min < value) { return min; }
            if (value > max) { return max; }
            return value;
        }

        // i wish i could inline this shit
        public static float Abs(float value)
        {
            if (value < 0) { return -value; }
            return value;
        }

        public static bool CompareVector2(Vector2 vect1, Vector2 vect2, float cullRadius)
        {
            return vect1.X + cullRadius > vect2.X &&
                   vect1.Y + cullRadius > vect2.Y &&
                   vect1.X - cullRadius < vect2.X &&
                   vect1.Y - cullRadius < vect2.Y;
        }

        public static bool CompareVector2(Vector2 vect1, Vector2 vect2)
        {
            return CompareVector2(vect1, vect2, 10.0f);
        }


    }
}
