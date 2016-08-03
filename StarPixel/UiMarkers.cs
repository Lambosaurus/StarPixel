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

        public void DrawMarkers(Camera camera)
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

        
        public MarkerPoint(Vector2 position, float arg_size, Color arg_color)
        {
            line_color = arg_color;
            pos = position;

            radius = arg_size; // this doesnt work when zoom.
        }

        public override bool InView(Camera camera)
        {
            return camera.ContainsCircle(pos, radius * camera.upsample_multiplier / camera.scale);
        }

        public override float LineRadius(float scale, float angle)
        {
            return line_join_radius;
        }

        public override void Draw(Camera camera)
        {
            if (!InView(camera)) { return; }

            ArtPrimitive.DrawCircle(camera, camera.Map(pos), line_color, radius * camera.upsample_multiplier);

            DrawMarkers(camera);
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
            float radius_s = radius * camera.scale;

            if (radius_s / camera.upsample_multiplier < minimum_radius)
            {
                ArtPrimitive.DrawCircle(camera, center, line_color, minimum_radius * camera.upsample_multiplier);
            }
            else
            {
                ArtPrimitive.DrawCircle(camera, center, fill_color, radius_s);

                if (line_color != Color.Transparent)
                {
                    ArtPrimitive.DrawArc(camera, center, 0.0f, MathHelper.TwoPi,
                        radius_s, line_color, line_thickness, dashing);
                }
            }

            DrawMarkers(camera);
        }
    }
    
    public class MarkerQuad : MarkerNode
    {
        public enum QuadType { Square, Diamond };

        public ArtPrimitive.ShapeDashing dashing = ArtPrimitive.ShapeDashing.None;
        public float angle;
        public float size;

        public MarkerQuad(Vector2 arg_pos, float arg_radius, Color color, QuadType quad = QuadType.Square)
        {
            size = arg_radius;
            radius = arg_radius * Utility.root_two;


            pos = arg_pos;
            line_color = color;
            fill_color = color * FILL_ALPHA;

            angle = (quad == QuadType.Square) ? 0.0f : MathHelper.PiOver4;
        }

        public override float LineRadius(float scale, float arg_angle)
        {
            float equiv_angle = Utility.Mod(arg_angle + MathHelper.PiOver4 - angle, MathHelper.PiOver2) - MathHelper.PiOver4;
            return size / Utility.Cos( equiv_angle );
        }

        public override void Draw(Camera camera)
        {
            if (!InView(camera)) { return; }

            Vector2 center = camera.Map(pos);
            float radius_s = size * camera.scale;

            if (radius * camera.scale / camera.upsample_multiplier < minimum_radius)
            {
                float mrs = minimum_radius * camera.upsample_multiplier;
                ArtPrimitive.DrawSquare(camera, center, new Vector2(mrs, mrs), line_color, angle);
            }
            else
            {
                ArtPrimitive.DrawSquare(camera, center, new Vector2(radius_s, radius_s), fill_color, angle);
                ArtPrimitive.DrawBox(camera, center, new Vector2(radius_s, radius_s), line_color, line_thickness, angle, dashing);
            }

            DrawMarkers(camera);
        }
    }

    public class MarkerLine : UIMarker
    {
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
                line_vector.Normalize();
                float angle = Utility.Angle(line_vector);
                
                Vector2 st = startpoint.pos + (startpoint.LineRadius(camera.scale, angle) * line_vector);
                Vector2 en = endpoint.pos - (endpoint.LineRadius(camera.scale, angle - MathHelper.TwoPi) * line_vector);
                
                if ( Utility.Dot(en - st, line_vector) > 0.0f )
                {
                    ArtPrimitive.DrawLine(camera, camera.Map(st), camera.Map(en), line_color, line_thickness);
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
            tile_sheet.Draw(camera.batch, symbol_no, position, scale, color);
        }
    }


    public class MarkerShipDefence : UIMarker
    {
        static float armor_bar_sep = 0.05f;

        static Color shield_bar_color = Color.Lerp(Color.DeepSkyBlue, Color.Blue, 0.5f);
        static Color dead_shield_bar_color = Color.Lerp(Color.Lerp(Color.DeepSkyBlue, Color.Blue, 0.5f), Color.Black, 0.6f);

        static float line_width = 4.0f;

        Ship ship;

        public MarkerShipDefence(Ship arg_ship)
        {
            ship = arg_ship;

            radius = ship.template.shield_radius;
            pos = ship.pos;
        }
        
        public override void Draw(Camera camera)
        {
            pos = camera.Map(ship.pos);
            
            float angle = ship.angle;
            float s_radius = camera.scale * radius;
            
            if (ship.shield != null)
            {
                Color shcolor = (ship.shield.active) ? shield_bar_color : dead_shield_bar_color;

                ArtPrimitive.DrawArc(camera, pos, -MathHelper.PiOver2,
                    MathHelper.TwoPi * ship.shield.integrity / ship.shield.max_integrity, s_radius, shcolor, line_width);
            }

            if (ship.armor != null)
            {
                float a1 = ship.armor.start_angle + angle;
                a1 = Utility.WrapAngle(a1);

                for (int i = 0; i < ship.armor.segment_count; i++)
                {
                    float a2 = a1 + ship.armor.per_segment_angle;

                    float k = ship.armor.integrity[i] / ship.armor.max_integrity;

                    if (k > 0)
                    {
                        ArtPrimitive.DrawArc(camera, pos, a1 + armor_bar_sep, ship.armor.per_segment_angle - (2 * armor_bar_sep),
                            s_radius - (2*line_width * camera.upsample_multiplier),
                            ColorManager.HPColor(k), line_width);
                    }

                    a1 = a2;
                }
            }
        }
    }
}
