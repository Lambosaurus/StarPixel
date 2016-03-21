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

            shield_templates.Add("default",template);


            ArtShieldResource art = new ArtShieldResource("particle");
            art.particle_color_start = Color.SkyBlue;
            art.particle_count = 16;
            art.particle_size_max = 1.2f;
            art.particle_size_min = 0.5f;
            art.particle_speed = 0.06f;
            art.particle_life = 1.0f;
            ArtManager.shields["default"] = art;
        }
    }
}