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
        static float[] sin_values = new float[SINE_RESOLUTION];
        static float[] cos_values = new float[SINE_RESOLUTION];
        
        const float LOOKUP_CONSTANT = SINE_RESOLUTION / MathHelper.TwoPi; // or just (1 / rads step)

        public static float Cos(float rads)
        {
            return cos_values[ (int)(rads * LOOKUP_CONSTANT)];
        }

        public static float Sin(float rads)
        {
            return sin_values[(int)(rads * LOOKUP_CONSTANT)];
        }

        public static void Load()
        {
            for ( int i = 0; i < SINE_RESOLUTION; i++)
            {
                float angle = i * MathHelper.TwoPi / SINE_RESOLUTION;

                sin_values[i] = (float)Math.Sin(angle);
                cos_values[i] = (float)Math.Cos(angle);
            }
        }

        public static float Clamp(float value, float min = 1.0f, float max = 1.0f)
        {
            if (min > value) { return min; }
            if (value > max) { return max; }
            return value;
        }

        // i wish i could inline this shit
        public static float Abs(float value)
        {
            if (value < 0) { return -value; }
            return value;
        }


    }
}
