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
    public static class GameConst
    {
        public const int framerate = 60;
        
        public const float collision_friction = 0.2f;
        public const float collision_elasticity = 0.9f;
        
        public const float angular_velocity_limit = MathHelper.PiOver4;

        public const int upsample = 2; // our options are 1, 2 or violent crash
        public const float scroll_zoom = 1.14f;

        public const float minimum_draw_radius = 1.5f;
        

        public static float C(int c)
        {
            float size = 1.0f;
            while (c-- > 1)
            {
                size *= 1.5f;
            }
            return size;
        }

    }
}