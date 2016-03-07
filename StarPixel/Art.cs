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

    public static class ArtManager
    {

        public static Dictionary<string, ArtSpriteResource> sprites = new Dictionary<string, ArtSpriteResource>();
        
        public static Dictionary<string, ArtVentResource> vents = new Dictionary<string, ArtVentResource>();

        public static Dictionary<string, ArtExplosionResource> explosions = new Dictionary<string, ArtExplosionResource>();

        public static Dictionary<string, ArtShieldResource> shields = new Dictionary<string, ArtShieldResource>();


        public static Texture2D pixel;

        public static void Load(ContentManager content)
        {
            pixel = content.Load<Texture2D>("px");

            foreach (ArtSpriteResource sprite in sprites.Values)
            {
                sprite.Load(content);
            }

            foreach (ArtVentResource vent in vents.Values)
            {
                vent.Load(content);
            }

            foreach ( ArtExplosionResource exp in explosions.Values)
            {
                exp.Load(content);
            }

            foreach (ArtShieldResource shield in shields.Values)
            {
                shield.Load(content);
            }
        }

        // TODO, change all these to GetTemplate form
        public static ArtSpriteResource GetSpriteResource(string key)
        {
            return sprites[key];
        }

        public static ArtVentResource GetVentResource(string key)
        {
            return vents[key];
        }

        public static ArtExplosionResource GetExplosionResource(string key)
        {
            return explosions[key];
        }

        public static ArtShieldResource GetShieldResource(string key)
        {
            return shields[key];
        }
    }

    public static class ColorManager
    {
        public static Color[] thermo_colors;

        public static float max_temperature = 6000;
        public static float thermo_color_constant;
        public static int thermo_scale_length;

        public static void Load(ContentManager content)
        {
            Texture2D thermal_scale = content.Load<Texture2D>("thermal_scale");


            thermo_scale_length = thermal_scale.Bounds.Width;
            thermo_colors = new Color[thermo_scale_length];
            thermal_scale.GetData<Color>(thermo_colors);

            thermo_color_constant = thermo_scale_length / (max_temperature + 1);
        }

        public static Color GetThermo(float temp)
        {
            if (temp < 0) { return thermo_colors[0]; }
            else if (temp > max_temperature) { return thermo_colors[thermo_scale_length - 1]; }
            return thermo_colors[(int)(thermo_color_constant * temp)];
        }
    }


    public static class ArtLine
    {
        public static void DrawLineU( Camera camera, Vector2 p1, Vector2 p2, Color color, float width)
        {
            Vector2 center = (p1 + p2) / 2;
            Vector2 stretch = new Vector2(Vector2.Distance(p1, p2), width);
            float angle2 = Utility.Angle(p1 - p2);
            camera.batch.Draw(ArtManager.pixel, center, null, color, angle2, new Vector2(0.5f, 0.5f), stretch, SpriteEffects.None, 0);
        }



        const int ARC_COUNT = 80;

        public static void DrawArcU( Camera camera, Vector2 center, float angle_start, float arc_length, float radius, Color color, float width )
        {
            angle_start = Utility.WrapAngle(angle_start);

            int segments = (int)(ARC_COUNT * arc_length / MathHelper.TwoPi);

            Vector2 p1 = Utility.CosSin(angle_start, radius) + center;
            for (int i = 0; i < (segments+1); i++)
            {
                Vector2 p2 = Utility.CosSin(angle_start + (i * MathHelper.TwoPi / ARC_COUNT), radius) + center;

                DrawLineU(camera, p1, p2, color, width);

                p1 = p2;
            }


            float last = (arc_length - (((float)segments / ARC_COUNT) * MathHelper.TwoPi));
            last /= (MathHelper.TwoPi / ARC_COUNT);

            Vector2 p3 = Utility.CosSin(angle_start + ((segments+1) * MathHelper.TwoPi / ARC_COUNT), radius) + center;

            DrawLineU(camera, p1, p3, color * last, width);
            
        }

    }


    public class ArtTemporary
    {
        public virtual bool ReadyForRemoval()
        {
            return true;
        }

        public virtual void Update()
        {
        }

        public virtual bool InView(Camera camera)
        {
            return false;
        }

        public virtual void Draw(Camera camera)
        {
        }
    }
       

}





