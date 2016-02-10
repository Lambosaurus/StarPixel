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
        static ArtSpriteResource sprite_default;

        public static Dictionary<string, ArtVentResource> vents = new Dictionary<string, ArtVentResource>();
        static ArtVentResource vent_default;

        public static void Load(ContentManager content)
        {

            sprite_default = new ArtSpriteResource("default_sprite");
            sprite_default.Load(content);

            foreach (ArtSpriteResource sprite in sprites.Values)
            {
                sprite.Load(content);
            }


            vent_default = new ArtVentResource("particle");
            vent_default.std_ejection_temperature = 2000;
            vent_default.std_particle_count = 200;
            vent_default.std_particle_length = 1.0f;
            vent_default.std_particle_stretch = 4f;
            vent_default.std_particle_life = 0.75f;
            vent_default.std_particle_width = 0.75f;
            vent_default.std_temp_halflife = 0.33f;
            vent_default.std_temperature_scatter = 0.0f;
            vent_default.std_velocity_scatter = 0.1f;
            vent_default.std_ejection_velocity = 1f;


            vent_default.Load(content);

            foreach (ArtVentResource vent in vents.Values)
            {
                vent.Load(content);
            }
        }

        public static ArtSprite NewArtSprite(string key)
        {
            if (sprites.ContainsKey(key))
            {
                return sprites[key].New();
            }

            return sprite_default.New();
        }

        public static ArtVent NewArtVent(string key, float scale)
        {
            if (vents.ContainsKey(key))
            {
                return vents[key].New(scale);
            }

            return vent_default.New(scale);
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


    public class ArtSpaceDust
    {
        public Vector2 TileSize;
    }

}





