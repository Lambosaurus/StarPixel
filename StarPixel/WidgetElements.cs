using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace StarPixel
{
    public class WidgetElement
    {

        Vector2 pos;
        Vector2 size;

        public WidgetElement()
        {

        }

    }



    public class HitboxArmorMarker
    {
        public List<Vector2>[] nodes;

        public Armor armor;


        public HitboxArmorMarker(HitboxPolygon hitbox, Armor arg_armor, float displacement, float segment_separation)
        {
            // we need to construct the lines to represent our armor segments.
            // We have to project the armors angular system onto the hitbox.

            armor = arg_armor;
            nodes = new List<Vector2>[armor.segment_count];

            Vector2 p1 = hitbox.corners[0];
            int first_segment = armor.GetSegmentLocal(p1);
            int segment = first_segment;
            nodes[segment] = new List<Vector2>();

            List<Vector2> popped = null;

            for (int j = 0; j < hitbox.count; j++)
            {
                // iterate through all hitbox segments
                int i = j + 1;
                if (i == hitbox.count) { i = 0; }
                Vector2 p2 = hitbox.corners[i];

                // if this line has crossed an armor intersection
                int new_segment = armor.GetSegmentLocal(p2);
                if (new_segment != segment)
                {
                    // We find the midpoint of the the line where it croses the armor angle
                    Vector2 cossin = Utility.CosSin(-armor.GetSegmentStartAngle(segment));
                    float p1ry = p1.X * cossin.Y + p1.Y * cossin.X;
                    float p2ry = p2.X * cossin.Y + p2.Y * cossin.X;
                    float alpha = p1ry / (p1ry - p2ry);
                    Vector2 mid = ((p2 - p1) * alpha) + p1;


                    if (new_segment == first_segment)
                    {
                        // this segment already has data in it.
                        // we will add this back in later.
                        popped = nodes[new_segment];
                    }

                    // the new segment starts at the midpoint
                    nodes[new_segment] = new List<Vector2>();
                    nodes[new_segment].Add(mid);
                    nodes[new_segment].Add(p2);

                    // the previous segment ends at the midpoint
                    nodes[segment].Add(mid);

                    segment = new_segment;

                }
                else
                {
                    nodes[segment].Add(p2);
                }


                p1 = p2;
            }


            if (popped != null)
            {
                // We popped some of the data from the first segment, time to get it back
                foreach (Vector2 pt in popped)
                {
                    nodes[first_segment].Add(pt);
                }
            }


            float sep = segment_separation / 2.0f;

            foreach (List<Vector2> node in nodes)
            {
                trim(node, 1, sep);
                trim(node, -1, sep);
            }

            /*
            List<Vector2> displacements = new List<Vector2>();

            Vector2 last = nodes.Last().Last();

            foreach (List<Vector2> node in nodes)
            {
                foreach (Vector2 p in node)
                {
                    Vector2 delta = last - p;
                    delta.Normalize();

                    displacements.Add(Utility.RotateNeg(delta) * displacement);

                    last = p;
                }
            }

            int k = 0;
            foreach (List<Vector2> node in nodes)
            {
                for ( int i = 0; i < node.Count; i++)
                {
                    int kl = k + 1;
                    //if (kl < 0) { kl = displacements.Count - 1; }
                    if (kl > displacements.Count - 1) { kl = 0; }

                    node[i] += displacements[k];
                    node[i] += displacements[kl];

                    k++;
                }
            }
            */
        }

        void trim(List<Vector2> points, int direction, float reminaing)
        {
            int k = (direction == 1) ? 0 : points.Count - 1;

            Vector2 delta = points[k + direction] - points[k];
            float len = delta.Length();
            if (reminaing < len)
            {
                delta.Normalize();
                points[k] += delta * reminaing;
            }
            else
            {
                reminaing -= len;
                points.RemoveAt(k);
                if (points.Count > 2)
                {
                    trim(points, direction, reminaing);
                }
            }
        }

        
        public void Draw(Camera camera, float line_width)
        {
            for (int i = 0; i < armor.segment_count; i++)
            {
                Color color = ColorManager.HPColor(armor.integrity[i] / armor.max_integrity);
                List<Vector2> segments = nodes[i];

                int segment_count = nodes[i].Count - 1;

                Vector2 st = camera.Map(segments[0]);
                for (int k = 0; k < segment_count; k++)
                {
                    Vector2 en = camera.Map(segments[k + 1]);
                    ArtPrimitive.DrawLine(st, en, color, line_width);

                    if (k < segment_count - 1)
                    {
                        ArtPrimitive.DrawCircle(en, color, line_width / 2.0f);
                        st = en;
                    }
                }
            }
        }
    }
}
