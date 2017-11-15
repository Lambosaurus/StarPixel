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
        public Vector2 pos { get; protected set; }
        public Vector2 size { get; protected set; }

        public WidgetElement()
        {
        }

        public virtual void Draw(Camera camera)
        {

        }
    }

    public class WidgetElementBar : WidgetElement
    {
        public Color full_color;
        public Color empty_color;
        public float fill = 0.5f;

        public float angle { get; protected set; }

        public WidgetElementBar(Vector2 arg_pos, Vector2 arg_size, float arg_angle = 0.0f)
        {
            angle = arg_angle;
            pos = arg_pos;
            size = arg_size;
        }

        public override void Draw(Camera camera)
        {
            fill = Utility.Clamp(fill, 0.0f, 1.0f);
            Vector2 length = Utility.CosSin(angle) * size.X;
            Vector2 posm = camera.Map(pos);
            Vector2 mid = camera.Map(pos + length * fill);
            Vector2 end = camera.Map(pos + length);

            ArtPrimitive.DrawLine(posm, mid, full_color, size.Y);
            ArtPrimitive.DrawLine(mid, end, empty_color, size.Y);
        }
    }

    public class WidgetElementArcBar : WidgetElementBar
    {
        public float radius;
        const float maximum_radius = 2e3f;

        Vector2 origin;
        float start_angle;
        float end_angle;
        float delta_angle;

        public WidgetElementArcBar(Vector2 arg_pos, Vector2 arg_size, float arg_angle, float arg_radius)
            : base( arg_pos, arg_size, arg_angle )
        {
            radius = arg_radius;
            Calc();
        }

        public void Calc()
        {
            float mid_length = size.X / 2.0f;

            bool left = false;
            if (radius < 0)
            {
                // the below sqrt calc fails for negative numbers
                radius = -radius;
                left = true;
            }

            radius = Utility.Clamp(radius, mid_length, maximum_radius); // problem is unsolvable if this is the case
            float offset_length = Utility.Sqrt((radius * radius) - (mid_length * mid_length));
            if (left) { offset_length = -offset_length; }

            Vector2 origin_offset = Utility.Rotate(new Vector2(mid_length, offset_length), angle);

            start_angle = Utility.WrapAngle( Utility.Angle(new Vector2(-mid_length, -offset_length)) + angle);
            end_angle = Utility.WrapAngle( Utility.Angle(new Vector2(mid_length, -offset_length)) + angle);
            origin = pos + Utility.Rotate(origin_offset, angle);
            delta_angle = end_angle - start_angle;
        }

        public override void Draw(Camera camera)
        {
            Vector2 center = camera.Map(origin);
            ArtPrimitive.DrawArc(center, start_angle, delta_angle * fill, radius, full_color, size.Y, segments_per_2pi:64);
            ArtPrimitive.DrawArc(center, end_angle, delta_angle * (fill - 1), radius, empty_color, size.Y, segments_per_2pi:64);
        }
    }


    public class HitboxArmorMarker
    {
        public List<Vector2>[] nodes;

        public Armor armor;

        float line_width;
        
        public HitboxArmorMarker(HitboxPolygon hitbox, Armor arg_armor, float displacement, float segment_separation, float arg_line_width)
        {
            // we need to construct the lines to represent our armor segments.
            // We have to project the armors angular system onto the hitbox.

            armor = arg_armor;
            nodes = new List<Vector2>[armor.segment_count];
            
            line_width = arg_line_width;

            int count = hitbox.count;
            Vector2[] corners = new Vector2[count];
            Vector2[] displacements = new Vector2[count];

            Vector2 last = hitbox.corners.Last();
            
            for (int i = 0; i < count; i++)
            {
                Vector2 p = hitbox.corners[i];

                Vector2 delta = last - p;
                delta.Normalize();

                displacements[i] = Utility.RotateNeg(delta) * displacement / 2.0f;
                corners[i] = p;
                last = p;
            }
            for (int i = 0; i < count; i++)
            {
                int k = (i == count - 1) ? 0 : i + 1;

                corners[i] += displacements[i];
                corners[i] += displacements[k];
            }



            Vector2 p1 = corners[0];
            int first_segment = armor.GetSegmentLocal(p1);
            int segment = first_segment;
            nodes[segment] = new List<Vector2>();

            List<Vector2> popped = null;

            for (int j = 0; j < count; j++)
            {
                // iterate through all hitbox segments
                int i = j + 1;
                if (i == count) { i = 0; }
                Vector2 p2 = corners[i];

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
                Trim(node, 1, sep);
                Trim(node, -1, sep);
            }
            
        }

        void Trim(List<Vector2> points, int direction, float reminaing)
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
                    Trim(points, direction, reminaing);
                }
            }
        }

        
        public void Draw(Camera camera)
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
                    ArtPrimitive.DrawLineAA(st, en, color, line_width * camera.ui_feature_scale);

                    if (k < segment_count - 1)
                    {
                        ArtPrimitive.DrawCircle(en, color, line_width * camera.ui_feature_scale / 2.0f);
                        st = en;
                    }
                }
            }
        }
    }
}
