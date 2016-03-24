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



            ArtExplosionResource pop = new ArtExplosionResource("particle");
            pop.coloring_method = ParticleColoring.Blend;
            pop.particle_color_start = Color.White;
            pop.particle_color_end = Color.Blue;
            pop.particle_life = 8f;
            pop.size_start = new Vector2(0.3f, 1.0f);
            pop.size_end = new Vector2(1.4f, 0.4f);
            pop.velocity_scatter = 0.5f;
            pop.velocity_bounce = 0.0f;
            pop.particle_size_scatter = 1.0f;
            ArtManager.explosions["shieldpop"] = pop;

            ArtShieldResource art = new ArtShieldResource("particle");
            art.particle_color_start = Color.White;
            art.particle_color_end = Color.Blue;
            art.particle_count = 16;
            art.particle_size_max = 1.2f;
            art.particle_size_min = 0.5f;
            art.particle_speed = 0.06f;
            art.particle_life = 1.0f;
            ArtManager.shields["default"] = art;
            art.pop_resource = pop;





            template = new ShieldTemplate();

            template.art_resource = "green";

            shield_templates.Add("green", template);


            pop = new ArtExplosionResource("particle");
            pop.coloring_method = ParticleColoring.Blend;
            pop.particle_color_start = Color.White;
            pop.particle_color_end = Color.LimeGreen;
            pop.particle_life = 4f;
            pop.size_start = new Vector2(0.3f, 1.0f);
            pop.size_end = new Vector2(1.4f, 0.4f);
            pop.velocity_scatter = 0.5f;
            pop.velocity_bounce = 0.0f;
            pop.particle_size_scatter = 1.0f;
            ArtManager.explosions["shieldpop2"] = pop;

            art = new ArtShieldResource("particle");
            art.particle_color_start = Color.White;
            art.particle_color_end = Color.LimeGreen;
            art.particle_count = 18;
            art.particle_size_max = 1.0f;
            art.particle_size_min = 0.4f;
            art.particle_speed = 0.07f;
            art.particle_life = 0.8f;
            ArtManager.shields["green"] = art;
            art.pop_resource = pop;


        }
    }
}