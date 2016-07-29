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

    public class MarkerFilledCircle : UIMarker
    {
        public float line_thickness = 4;
        public Color line_color = Color.Red;
        public Color fill_color = Color.Transparent;

        
        public MarkerFilledCircle()
        {

        }
        
        public override void Draw(Camera camera)
        {
            if (!InView(camera)) { return; }
           

            ArtLine.DrawArcU(camera, camera.Map( pos ), 0.0f, MathHelper.TwoPi,
                radius * camera.scale, line_color, line_thickness);
        }
    }



    public static class ArtLine
    {
        public static void DrawLineU(Camera camera, Vector2 p1, Vector2 p2, Color color, float width)
        {
            Vector2 center = (p1 + p2) / 2;
            Vector2 stretch = new Vector2(Vector2.Distance(p1, p2), width);
            float angle2 = Utility.Angle(p1 - p2);
            camera.batch.Draw(ArtManager.pixel, center, null, color, angle2, new Vector2(0.5f, 0.5f), stretch, SpriteEffects.None, 0);
        }



        const int ARC_COUNT = 40;

        public static void DrawArcUF(Camera camera, Vector2 center, float angle_start, float arc_length, float radius, Color color, float width)
        {
            angle_start = Utility.WrapAngle(angle_start);

            int segments = (int)(ARC_COUNT * arc_length / MathHelper.TwoPi);

            Vector2 p1 = Utility.CosSin(angle_start, radius) + center;
            for (int i = 0; i < (segments + 1); i++)
            {
                Vector2 p2 = Utility.CosSin(angle_start + (i * MathHelper.TwoPi / ARC_COUNT), radius) + center;

                DrawLineU(camera, p1, p2, color, width);

                p1 = p2;
            }


            float last = (arc_length - (((float)segments / ARC_COUNT) * MathHelper.TwoPi));
            last /= (MathHelper.TwoPi / ARC_COUNT);

            Vector2 p3 = Utility.CosSin(angle_start + ((segments + 1) * MathHelper.TwoPi / ARC_COUNT), radius) + center;

            DrawLineU(camera, p1, p3, color * last, width);

        }

        public static void DrawArcU(Camera camera, Vector2 center, float angle_start, float arc_length, float radius, Color color, float width)
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

                DrawLineU(camera, p1, p2, color, width);

                p1 = p2;

            }
        }
    }

}
