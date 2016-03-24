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
    static class AssetShieldTemplates
    {
        public static Dictionary<string, ShieldTemplate> shield_templates = new Dictionary<string, ShieldTemplate>();

        public static void GenerateAssets()
        {
            ShieldTemplate template = new ShieldTemplate();

            template.art_resource = "default";

            shield_templates["default"] = template;
            
            
            ArtShieldResource art = new ArtShieldResource("particle");
            art.particle_color_start = Color.White;
            art.particle_color_end = Color.Blue;
            art.particle_count = 16;
            art.shield_particle_size_max = 1.2f;
            art.shield_particle_size_min = 0.5f;
            art.shield_particle_speed = 0.06f;
            art.shield_particle_halflife = 1.0f;

            art.particle_velocity = 0.5f;
            art.particle_life = 8f;
            //art.size_end = new Vector2(0.5f, 0.5f);

            ArtManager.shields["default"] = art;






            template = new ShieldTemplate();

            template.art_resource = "green";

            shield_templates["green"] = template;


            art = new ArtShieldResource("particle");
            art.particle_color_start = Color.White;
            art.particle_color_end = Color.LimeGreen;
            art.particle_count = 20;
            art.shield_particle_size_max = 0.8f;
            art.shield_particle_size_min = 0.3f;
            art.shield_particle_speed = 0.1f;
            art.shield_particle_halflife = 0.8f;

            art.particle_velocity = 0.7f;
            art.particle_life = 4f;
            //art.size_end = new Vector2(0.5f, 0.5f);

            ArtManager.shields["green"] = art;


        }
    }
}