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
            art.color = Color.SkyBlue;
            art.particle_density = 16;
            art.std_particle_size_max = 1.2f;
            art.std_particle_size_min = 0.5f;
            art.std_particle_speed = 0.06f;
            art.std_particle_halflife = 1.0f;
            ArtManager.shields["default"] = art;
        }
    }
}