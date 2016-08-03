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
    public static class ArtPrimitive
    {
        // All of the functions in this take onscreen_coordinates, not world coordinates.

        public enum CircleDashing { None = 0, Many = 1, Moderate = 2, Quarters = 4, Halves = 8 };
        public enum ShapeDashing { None = 0, Corners = 1, One = 2, Two = 3, Four = 4 };

        static Texture2D pixel;
        static Texture2D circle;

        public static void Load(ContentManager content)
        {
            pixel = content.Load<Texture2D>("px");
            circle = content.Load<Texture2D>("Circle");
        }

        public static void DrawLine(Camera camera, Vector2 p1, Vector2 p2, Color color, float width)
        {
            Vector2 center = (p1 + p2) / 2;
            Vector2 stretch = new Vector2(Vector2.Distance(p1, p2), width * camera.upsample_multiplier);
            float angle2 = Utility.Angle(p1 - p2);
            camera.batch.Draw(pixel, center, null, color, angle2, new Vector2(0.5f, 0.5f), stretch, SpriteEffects.None, 0);
        }

        public static void DrawLineDashed( Camera camera, Vector2 p1, Vector2 p2, Color color, float width, int dashes)
        {
            dashes += 2;
            int slices = (dashes*2) - 1;
            
            Vector2 segment_length = (p2 - p1) / slices;

            Vector2 start = p1;
            Vector2 end;

            for (int i = 0; i < dashes; i++ )
            {
                end = start + segment_length;

                DrawLine(camera, start, end, color, width);

                start = end + segment_length;
            }
        }

        

        const int STD_ARC_COUNT = 32;
        const float HALF_ARC_COUNT_RADIUS = 30f;
        const float DOUBLE_ARC_COUNT_RADIUS = 300f;

        public static void DrawArc(Camera camera, Vector2 center, float angle_start, float arc_length, float radius, Color color, float width, CircleDashing dashing = CircleDashing.None)
        {
            int dash_spacing = (int)dashing;


            // calculate the number of segments we would use on a full circle
            int arc_count = STD_ARC_COUNT;
            if (radius > DOUBLE_ARC_COUNT_RADIUS)
            {
                // if the circle is large, double it
                arc_count *= 2;
                dash_spacing *= 2; // Keeps the dashing consistant
            }
            else if (dash_spacing != 1 && radius < HALF_ARC_COUNT_RADIUS) // we cant half dash_spacing 1, so we cannot change the radius in this case.
            {
                // if small, half it
                arc_count /= 2;
                dash_spacing /= 2;
            }

            angle_start = Utility.WrapAngle(angle_start);

            // Calculate the number of segments to construct the arc.
            int segments = (int)(arc_count * arc_length / MathHelper.TwoPi);
            float da = arc_length / segments; // angle traveled per slice

            
            float a = angle_start;
            Vector2 p1 = Utility.CosSin(a, radius) + center;
            for (int i = 0; i < segments; i++)
            {
                // go through the circle segments
                a += da;
                Vector2 p2 = Utility.CosSin(a, radius) + center;

                if ((dash_spacing == 0) || ((i % (dash_spacing * 2)) < dash_spacing))
                {
                    // we skip every few segments to dash the lines
                    DrawLine(camera, p1, p2, color, width);
                }

                p1 = p2;

            }
        }

        public static void DrawBox( Camera camera, Vector2 center, Vector2 size, Color color, float width, float angle = 0.0f, ShapeDashing dashing = ShapeDashing.None )
        {
            Vector2 sz = Utility.Rotate(size, angle);

            Vector2 p1 = center + sz;
            Vector2 p2 = center + Utility.RotatePos(sz);
            Vector2 p3 = center - sz;
            Vector2 p4 = center + Utility.RotateNeg(sz);

            if (dashing == ShapeDashing.None)
            {
                DrawLine(camera, p1, p2, color, width);
                DrawLine(camera, p2, p3, color, width);
                DrawLine(camera, p3, p4, color, width);
                DrawLine(camera, p4, p1, color, width);
            }
            else
            {
                int dashes = (int)dashing - 1;
                DrawLineDashed(camera, p1, p2, color, width, dashes);
                DrawLineDashed(camera, p2, p3, color, width, dashes);
                DrawLineDashed(camera, p3, p4, color, width, dashes);
                DrawLineDashed(camera, p4, p1, color, width, dashes);
            }
        }

        public static void DrawCircle(Camera camera, Vector2 center, Color color, float radius)
        {
            radius /= 49.0f; // circle.png is a circle of about 49 pix radius
            camera.batch.Draw(circle, center, null, color, 0.0f, new Vector2(50f, 50f), new Vector2(radius, radius), SpriteEffects.None, 0);
        }

        public static void DrawSquare(Camera camera, Vector2 center, Vector2 size, Color color, float angle = 0.0f )
        {
            camera.batch.Draw(pixel, center, null, color, angle, new Vector2(0.5f, 0.5f), size*2, SpriteEffects.None, 0);
        }

    }
}
