﻿
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


// This is where i'd put my #defines
// IF I HAD ANY.


namespace StarPixel
{
    public static class Utility
    {
        // lol, 32KB right here. Ez.
        const int SINE_RESOLUTION = 4000;
        static float[] sin_values = new float[SINE_RESOLUTION+1];
        static float[] cos_values = new float[SINE_RESOLUTION+1];


        // TODO: Add in an EXP table

        const float LOOKUP_CONSTANT = SINE_RESOLUTION / MathHelper.TwoPi; // or just (1 / rads step)

        static Random random;


        // There is a lot of inlining done in this library.
        // its nasty and it takes ages to change stuff, but this stuff is used a lot.. so..
        static Utility()
        {
            random = new Random();

            // build the cos and sine tables used for Cos() and Sin()
            for (int i = 0; i < (SINE_RESOLUTION + 1); i++)
            {
                float angle = i * MathHelper.TwoPi / SINE_RESOLUTION;

                sin_values[i] = (float)Math.Sin(angle);
                cos_values[i] = (float)Math.Cos(angle);
            }
        }


        // here i provide a fast cos and sin function
        // Its a simple lookup table. Its more convenient than doing (float)Math.Cos(rads)
        // And about twice as fast. possibly.
        public static float Cos(float alpha)
        {
            // Utility.WrapAngle(alpha) inline
            alpha = alpha % MathHelper.TwoPi;
            if (alpha < 0) { alpha += MathHelper.TwoPi; }

            return cos_values[ (int)(alpha * LOOKUP_CONSTANT)];
        }

        public static float Sin(float alpha)
        {
            // Utility.WrapAngle(alpha) inline
            alpha = alpha % MathHelper.TwoPi;
            if (alpha < 0) { alpha += MathHelper.TwoPi; }
            return sin_values[(int)(alpha * LOOKUP_CONSTANT)];
        }

        // maybe i should build an atan2 table.... nah..


        // rotates a vector by alpha rads, about its origin
        // This is the very lifeblood of my code.
        public static Vector2 Rotate(Vector2 point, float alpha)
        {
            // Utility.WrapAngle(alpha) inline
            alpha = alpha % MathHelper.TwoPi;
            if (alpha < 0) { alpha += MathHelper.TwoPi; }

            float c = cos_values[(int)(alpha * LOOKUP_CONSTANT)];
            float s = sin_values[(int)(alpha * LOOKUP_CONSTANT)];

            return new Vector2( (c * point.X) - (s * point.Y),
                                (s * point.X) + (c * point.Y) );

        }


        // returns a vector at a given angle.
        // length can be specified for speed.
        public static Vector2 CosSin(float alpha)
        {
            // Utility.WrapAngle(alpha) inline
            alpha = alpha % MathHelper.TwoPi;
            if (alpha < 0) { alpha += MathHelper.TwoPi; }

            float c = cos_values[(int)(alpha * LOOKUP_CONSTANT)];
            float s = sin_values[(int)(alpha * LOOKUP_CONSTANT)];
            return new Vector2(c, s);
        }

        public static Vector2 CosSin(float alpha, float scale)
        {
            // Utility.WrapAngle(alpha) inline
            alpha = alpha % MathHelper.TwoPi;
            if (alpha < 0) { alpha += MathHelper.TwoPi; }

            float c = cos_values[(int)(alpha * LOOKUP_CONSTANT)];
            float s = sin_values[(int)(alpha * LOOKUP_CONSTANT)];
            return new Vector2(c*scale, s*scale);
        }


        public static float WrapAngle(float alpha)
        {
            alpha = alpha % MathHelper.TwoPi;
            if (alpha < 0) { return alpha + MathHelper.TwoPi; }
            return alpha;
        }


        // returns true if the value is between the minimum and maximum angles
        // It does take care of the angle wrapping.
        public static bool AngleWithin(float value, float min, float max)
        {
            max -= min;
            value -= min;

            // here i am just inlining the contents of WrapAngle
            max = max % MathHelper.TwoPi;
            if (max < 0) { max += MathHelper.TwoPi; }
            value = value % MathHelper.TwoPi;
            if (value < 0) { value += MathHelper.TwoPi; }

            return (value <= max);
        }

        
        // returns a vector of length scale with a random angle
        // Nice for generating a circular distribution
        public static Vector2 RandVec(float scale)
        {
            int a = random.Next(SINE_RESOLUTION);
            scale = Rand(scale);

            float c = cos_values[a];
            float s = sin_values[a];
            return new Vector2(c*scale, s*scale);
        }

        // returns a random value
        public static float Rand(float max)
        {
            return (max * random.Next(10000) / 10000f);
        }

        // option for min & max
        public static float Rand(float min, float max)
        {
            return min + ((max - min) * random.Next(10000) / 10000f);
        }

        public static float Clamp(float value, float min = -1.0f, float max = 1.0f)
        {
            if (value < min) { return min; }
            if (value > max) { return max; }
            return value;
        }

        // I wish i could inline this shit
        public static float Abs(float value)
        {
            if (value < 0) { return -value; }
            return value;
        }

        // Checks whether two points are within a windows distance of each other
        public static bool Window(Vector2 p1, Vector2 p2, Vector2 window)
        {
            return p1.X + window.X > p2.X &&
                   p1.Y + window.Y > p2.Y &&
                   p1.X - window.X < p2.X &&
                   p1.Y - window.Y < p2.Y;
        }

        public static bool Window(Vector2 p1, Vector2 p2, float window)
        {
            return p1.X + window > p2.X &&
                   p1.Y + window > p2.Y &&
                   p1.X - window < p2.X &&
                   p1.Y - window < p2.Y;
        }

        
        // gets the angle from the origin to the given point
        public static float Angle( Vector2 point )
        {
            return (float)Math.Atan2(point.Y, point.X);
        }

        // modulus with positive results only
        public static float Mod( float a, float b )
        {
            float m = a % b;
            return (m > 0) ? m : m + b;
        }

        // gets the shortest angle to the target angle from the current angle (with correct sign!)
        // It can be used to tell you which direction you should turn
        public static float AngleDelta( float target, float current )
        {
            float a = target - current;
            return Mod(a + MathHelper.Pi, MathHelper.TwoPi) - MathHelper.Pi;
        }


    }
}
