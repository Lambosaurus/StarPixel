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
    public abstract class UIMarker
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

    public abstract class MarkerNode : UIMarker
    {
        public List<MarkerIcon> icons_top;
        public List<MarkerIcon> icons_bot;

        public float line_thickness = 4;
        public Color line_color;
        public Color fill_color;

        public float minimum_radius = 8;

        protected const float ICON_HEIGHT_OFFSET = 10;
        protected const float FILL_ALPHA = 0.2f;

        public MarkerNode()
        {
            icons_top = new List<MarkerIcon>();
            icons_bot = new List<MarkerIcon>();
        }

        // gives the radius of the object when approached from the given angle.
        // This is needed to make it look nice when areas and lines are joined.
        public virtual float LineRadius(float scale, float angle)
        {
            return radius;
        }

        public void DrawIcons(Camera camera)
        {
            float offset = 0.0f;

            if (icons_top.Count != 0)
            {
                foreach (MarkerIcon icon in icons_top) { offset += icon.center.X; }
                foreach (MarkerIcon icon in icons_top)
                {
                    offset -= icon.center.X;
                    Vector2 position = camera.Map(pos + new Vector2(0, -radius)) - new Vector2(offset, ICON_HEIGHT_OFFSET + line_thickness + (icon.center.Y));
                    icon.Draw(camera, position);
                    offset -= icon.center.X;
                }
            }

            if (icons_bot.Count != 0)
            {

                offset = 0.0f;
                foreach (MarkerIcon icon in icons_bot) { offset += icon.center.X; }
                foreach (MarkerIcon icon in icons_bot)
                {
                    offset -= icon.center.X;
                    Vector2 position = camera.Map(pos + new Vector2(0, radius)) - new Vector2(offset, -ICON_HEIGHT_OFFSET - line_thickness - (icon.center.Y));
                    icon.Draw(camera, position);
                    offset -= icon.center.X;
                }
            }
        }
    }

    public class MarkerPoint : MarkerNode
    {
        public float line_join_radius = 0.0f;

        public MarkerIcon icon { get; private set; } = null; 


        public MarkerPoint(Vector2 position, float arg_size, Color arg_color)
        {
            line_color = arg_color;
            pos = position;
            
            radius = arg_size; // this doesnt work when zoom.
        }

        public MarkerPoint( Vector2 position, MarkerIcon arg_icon )
        {
            icon = arg_icon;
            pos = position;
            line_color = arg_icon.color; // needed so that a marker line will correctly inherit color
            radius = arg_icon.scale * arg_icon.center.Y; // we assume Y is greater than Y. This is a bit dodge, but it works for existing icons.
            line_join_radius = radius * 1.5f;
        }

        public override bool InView(Camera camera)
        {
            return camera.ContainsCircle(pos, radius * camera.pixel_constant);
        }

        public override float LineRadius(float scale, float angle)
        {
            return line_join_radius / scale;
        }

        public override void Draw(Camera camera)
        {
            if (!InView(camera)) { return; }

            if (icon == null)
            {
                ArtPrimitive.DrawCircle(camera.Map(pos), line_color, radius);
            }
            else
            {
                icon.Draw(camera, camera.Map(pos));
            }

            DrawIcons(camera);
        }


    }

    public class MarkerCircle : MarkerNode
    {
        public ArtPrimitive.CircleDashing dashing = ArtPrimitive.CircleDashing.None;

        public MarkerCircle(Vector2 arg_pos, float arg_radius, Color color)
        {
            radius = arg_radius;
            pos = arg_pos;
            line_color = color;
            fill_color = color * FILL_ALPHA;
        }
        
        public override void Draw(Camera camera)
        {
            if (!InView(camera)) { return; }

            Vector2 center = camera.Map(pos);
            float radius_s = radius / camera.pixel_constant;

            if (radius_s / camera.pixel_constant < minimum_radius)
            {
                ArtPrimitive.DrawCircle(center, line_color, minimum_radius);
            }
            else
            {
                ArtPrimitive.DrawCircle(center, fill_color, radius_s);

                if (line_color != Color.Transparent)
                {
                    ArtPrimitive.DrawArc(center, 0.0f, MathHelper.TwoPi,
                        radius_s, line_color, line_thickness, dashing);
                }
            }

            DrawIcons(camera);
        }
    }
    

    public class MarkerPoly : MarkerNode
    {
        public enum Type { Tri = 3, Quad = 4, Pent = 5 };

        public ArtPrimitive.ShapeDashing dashing = ArtPrimitive.ShapeDashing.None;

        public float angle;
        public float inner_radius;

        int n;

        public MarkerPoly(Vector2 arg_pos, float arg_radius, Color color, Type sides, bool rotate = false )
        {
            n = (int)sides;
            pos = arg_pos;
            angle = (rotate ? 0 : MathHelper.Pi/ n) + MathHelper.PiOver2;
            inner_radius = arg_radius;
            radius = inner_radius / Utility.Cos(MathHelper.Pi / n);
            line_color = color;
            fill_color = color * FILL_ALPHA;
        }

        
        public override float LineRadius(float scale, float arg_angle)
        {
            float da = MathHelper.TwoPi / (int)n;
            // sweet pants, i can apply this equation to any shape i want!
            float equiv_angle = Utility.Mod(arg_angle - angle, da) - (da/2);
            return inner_radius / Utility.Cos(equiv_angle);
        }

        public void DrawCenter(Camera camera, Vector2 center, Color color, float in_rad)
        {
            if (n == 3)
            {
                ArtPrimitive.DrawTriangle(center, new Vector2(in_rad, in_rad), color, angle);
            }
            else if (n == 4)
            {
                ArtPrimitive.DrawSquare(center, new Vector2(in_rad, in_rad), color, angle + MathHelper.PiOver4);
            }
            else if (n == 5)
            {
                ArtPrimitive.DrawPentagon(center, new Vector2(in_rad, in_rad), color, angle + MathHelper.TwoPi/5.0f);
            }
        }

        public override void Draw(Camera camera)
        {
            if (!InView(camera)) { return; }

            Vector2 center = camera.Map(pos);
            float radius_s = inner_radius / camera.pixel_constant;

            if (radius_s / camera.pixel_constant < minimum_radius)
            {
                float mrs = minimum_radius;
                DrawCenter(camera, center, line_color, mrs);
            }
            else
            {
                DrawCenter(camera, center, fill_color, radius_s);
                ArtPrimitive.DrawPolyLine(center, radius_s, n, line_color, line_thickness, angle, dashing);
            }

            DrawIcons(camera);
        }
    }


    public class MarkerLine : UIMarker
    {
        public int line_count = 1;

        public float line_thickness = 4;
        public Color line_color;

        public MarkerNode startpoint = null;
        public MarkerNode endpoint = null;

        public bool draw_startpoint = true;
        public bool draw_endpoint = true;

        public MarkerLine(Vector2 line_start, Vector2 line_end, Color color)
        {
            line_color = color;

            startpoint = DefaultMarkerNode(line_start);
            endpoint = DefaultMarkerNode(line_end);

            startpoint.fill_color = color;
            endpoint.fill_color = color;   
        }

        public MarkerLine(MarkerNode arg_startpoint, MarkerNode arg_endpoint)
        {
            startpoint = arg_startpoint;
            endpoint = arg_endpoint;
            line_color = endpoint.line_color;
        }

        void DoubleCheckSubMarkers()
        {
            if (startpoint == null) { startpoint = DefaultMarkerNode( new Vector2(0,0) ); }
            if (endpoint == null) { endpoint = DefaultMarkerNode( new Vector2(0,0) ); }
        }

        public MarkerNode DefaultMarkerNode(Vector2 pos)
        {
            return new MarkerPoint(pos, 4.0f, line_color);
        }
        

        public override void Draw(Camera camera)
        {
            DoubleCheckSubMarkers();
            
            radius = (startpoint.pos - endpoint.pos).Length() / 2.0f;
            pos = (startpoint.pos + endpoint.pos) / 2.0f;
            
            if (InView(camera))
            {
                Vector2 line_vector = (endpoint.pos - startpoint.pos);

                if (line_vector != Vector2.Zero) // if the startpoint and endpoint are set to the same, then line_vector.normalize fails
                {

                    line_vector.Normalize();
                    float angle = Utility.Angle(line_vector);

                    Vector2 st = startpoint.pos + (startpoint.LineRadius(camera.scale, angle) * line_vector);
                    Vector2 en = endpoint.pos - (endpoint.LineRadius(camera.scale, angle - MathHelper.TwoPi) * line_vector);

                    if (Utility.Dot(en - st, line_vector) > 0.0f)
                    {
                        float line_width = line_thickness *2.0f / (line_count + 1.0f);

                        if (line_count < 1) { line_count = 1; }
                        else if (line_count > 3) { line_count = 3; } // make sure its in range

                        else if (line_count == 1 || line_count == 3) // odd numbers get a center line
                        {
                            ArtPrimitive.DrawLine(camera.Map(st), camera.Map(en), line_color, line_width);
                        }
                        
                        if (line_count > 1) // two and three get edge lines
                        {
                            Vector2 delta = Utility.RotatePos(line_vector) * line_width * (line_count == 2 ? 1f: 2f);
                            ArtPrimitive.DrawLine(camera.Map(st) + delta, camera.Map(en) + delta, line_color, line_width);
                            ArtPrimitive.DrawLine(camera.Map(st) - delta, camera.Map(en) - delta, line_color, line_width);
                        }

                    }
                }
            }

            if (draw_startpoint) { startpoint.Draw(camera); }
            if (draw_endpoint) { endpoint.Draw(camera); }
        }
    }


    public class MarkerIcon
    {
        int symbol_no;

        public Color color;
        public float scale { get; private set; }

        public Vector2 center { get; private set; }

        SpriteTileSheet tile_sheet;

        public MarkerIcon(Symbols.GreekL arg_symbol, Color arg_color, float arg_scale = 1.0f)
        {
            this.Init(Symbols.greek_sheet, ((int)arg_symbol) + 24, arg_color, arg_scale);
        }

        public MarkerIcon(Symbols.GreekU arg_symbol, Color arg_color, float arg_scale = 1.0f)
        {
            this.Init(Symbols.greek_sheet, (int)arg_symbol, arg_color, arg_scale);
        }

        public MarkerIcon(Symbols.Number arg_symbol, Color arg_color, float arg_scale = 1.0f)
        {
            this.Init(Symbols.number_sheet, (int)arg_symbol, arg_color, arg_scale);
        }
        
        void Init(SpriteTileSheet arg_sheet, int number, Color arg_color, float arg_scale)
        {
            tile_sheet = arg_sheet;
            symbol_no = number;
            color = arg_color;
            scale = arg_scale;

            center = new Vector2(tile_sheet.tile_width, tile_sheet.tile_height) * scale / 2.0f;
        }
        
        public void Draw(Camera camera, Vector2 position)
        {
            // scale divided by 2, because there was a bug, and now im used to that size
            tile_sheet.Draw(camera.batch, symbol_no, position, scale / 2.0f, color);
        }
    }


    public class MarkerPhysicalDefence : UIMarker
    {
        static float armor_bar_sep = 0.05f;

        static Color shield_bar_color = Color.Lerp(Color.DeepSkyBlue, Color.Blue, 0.5f);
        static Color dead_shield_bar_color = Color.Lerp(Color.Lerp(Color.DeepSkyBlue, Color.Blue, 0.5f), Color.Black, 0.6f);

        static float line_width = 4.0f;

        Physical phys;

        const float MINIMUM_RADIUS = 16f;

        public MarkerPhysicalDefence(Physical arg_phys)
        {
            phys = arg_phys;

            radius = phys.radius;
            pos = phys.pos;
        }
        
        public override void Draw(Camera camera)
        {
            pos = camera.Map(phys.pos);
            
            float angle = phys.angle;
            float a_radius = radius / camera.pixel_constant;
            float s_radius = a_radius + (line_width* 2);

            float m_radius = MINIMUM_RADIUS;
            bool compact_view = s_radius < m_radius;
            
            
            if (phys.shield != null)
            {
                float s_integrity = phys.shield.integrity / phys.shield.max_integrity;

                if (compact_view)
                {
                    if (phys.shield.active)
                    {
                        Color shcolor = Color.Lerp(Color.Black, shield_bar_color, s_integrity);
                        ArtPrimitive.DrawCircle(pos, shcolor, m_radius);
                    }
                }
                else
                {
                    Color shcolor = (phys.shield.active) ? shield_bar_color : dead_shield_bar_color;
                    ArtPrimitive.DrawArc(pos, -MathHelper.PiOver2, MathHelper.TwoPi * s_integrity, s_radius, shcolor, line_width);
                }
            }

            if (phys.armor != null)
            {
                if (compact_view)
                {
                    float a_integrity = 0.0f;
                    for (int i = 0; i < phys.armor.segment_count; i++)
                    {
                        a_integrity += phys.armor.integrity[i] / phys.armor.max_integrity;
                    }
                    a_integrity /= phys.armor.segment_count;

                    ArtPrimitive.DrawCircleTag(pos, m_radius * 0.9f, ColorManager.HPColor(a_integrity), phys.angle);

                }
                else
                {
                    float a1 = phys.armor.start_angle + angle;
                    a1 = Utility.WrapAngle(a1);

                    for (int i = 0; i < phys.armor.segment_count; i++)
                    {
                        float a2 = a1 + phys.armor.per_segment_angle;

                        float k = phys.armor.integrity[i] / phys.armor.max_integrity;
                        
                        ArtPrimitive.DrawArc(pos, a1 + armor_bar_sep, phys.armor.per_segment_angle - (2 * armor_bar_sep),
                                a_radius, ColorManager.HPColor(k), line_width);
                        

                        a1 = a2;
                    }
                }
            }
        }
    }

    public class HitboxArmorMarker
    {
        public List<Vector2>[] nodes;

        public Armor armor;

        
        public HitboxArmorMarker( HitboxPolygon hitbox, Armor arg_armor, float segment_separation )
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
                    Vector2 cossin = Utility.CosSin(- armor.GetSegmentStartAngle(segment));
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

            foreach ( List<Vector2> node in nodes )
            {
                trim(node, 1, sep);
                trim(node, -1, sep);
            }
        }

        void trim( List<Vector2> points, int direction, float reminaing )
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
        
        public void Draw( Camera camera, float line_width )
        {
            for ( int i = 0; i < armor.segment_count; i++ )
            {
                Color color = ColorManager.HPColor(armor.integrity[i] / armor.max_integrity);
                List<Vector2> segments = nodes[i];

                int segment_count = nodes[i].Count - 1;

                Vector2 st = camera.Map(segments[0]);
                for ( int k = 0; k < segment_count; k++ )
                {
                    Vector2 en = camera.Map(segments[k+1]);
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
