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
    public class UIMarker
    {
        public float radius;
        public Vector2 pos;

        public virtual bool InView(Camera camera)
        {
            return camera.ContainsCircle(pos, radius);
        }

        public virtual void Draw(Camera camera)
        {
            
        }
    }

    public class MarkerCircle : UIMarker
    {
        public float line_thickness = 4;
        public Color line_color;
        public Color fill_color;

        public int dash = 0;
        
        public MarkerCircle(Vector2 arg_pos, float arg_radius, Color color)
        {
            radius = arg_radius;
            pos = arg_pos;
            line_color = color;
            fill_color = color * 0.3f;
        }
        
        public override void Draw(Camera camera)
        {
            if (!InView(camera)) { return; }

            ArtLine.DrawCircle(camera, camera.Map(pos), fill_color, radius);


            ArtLine.DrawArc(camera, camera.Map( pos ), 0.0f, MathHelper.TwoPi,
                radius * camera.scale, line_color, line_thickness * camera.upsample_multiplier, dash);
        }
    }



    public static class ArtLine
    {
        public static void DrawLine(Camera camera, Vector2 p1, Vector2 p2, Color color, float width)
        {
            Vector2 center = (p1 + p2) / 2;
            Vector2 stretch = new Vector2(Vector2.Distance(p1, p2), width);
            float angle2 = Utility.Angle(p1 - p2);
            camera.batch.Draw(ArtManager.pixel, center, null, color, angle2, new Vector2(0.5f, 0.5f), stretch, SpriteEffects.None, 0);
        }

        
        const int ARC_COUNT = 40;

        public static void DrawArc(Camera camera, Vector2 center, float angle_start, float arc_length, float radius, Color color, float width, int dashed = 0)
        {
            angle_start = Utility.WrapAngle(angle_start);
            int segments = (int)(ARC_COUNT * arc_length / MathHelper.TwoPi);

            float da = arc_length / segments;

            float a = angle_start;
            Vector2 p1 = Utility.CosSin(a, radius) + center;
            for (int i = 0; i < segments; i++)
            {
                a += da;
                Vector2 p2 = Utility.CosSin(a, radius) + center;

                if ((dashed == 0) || ( (i % (dashed*2)) < dashed))
                {
                    DrawLine(camera, p1, p2, color, width);
                }

                p1 = p2;

            }
        }
        
        public static void DrawCircle(Camera camera, Vector2 center, Color color, float radius)
        {
            radius *= camera.scale / 49.0f; // circle.png is a circle of about 49 pix radius
            camera.batch.Draw(ArtManager.circle, center, null, color, 0.0f, new Vector2(50f, 50f), new Vector2(radius, radius), SpriteEffects.None, 0);
        }
    }

}
