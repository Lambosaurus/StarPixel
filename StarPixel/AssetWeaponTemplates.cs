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
    static class AssetWeaponTemplates
    {
        public static Dictionary<string, WeaponTemplate> weapon_templates = new Dictionary<string, WeaponTemplate>();

        public static void GenerateAssets()
        {
            WeaponTemplate template = new WeaponTemplate();

            template.projectile_color = new Color(1f, 0.9f, 0.5f);
            template.projectile_sprite_resource = "bullet";
            template.projectile_explosion_resource = "boom";
            template.projectile_scale = new Vector2(1, 0.4f);
            template.fire_rate = 10;
            template.projectile_velocity = 6f;
            template.projectile_scatter = 0.5f;

            weapon_templates.Add("shooter",template);
            
            ArtManager.sprites.Add("bullet", new ArtSpriteResource("bullet"));


            ArtExplosionResource explosion = new ArtExplosionResource("particle");

            explosion.std_particle_count = 10;
            explosion.std_particle_length = 0.75f;
            explosion.std_particle_stretch_width = 1;
            explosion.std_particle_stretch_length = 6;
            explosion.std_particle_width = 0.5f;
            explosion.std_temperature = 4500;
            explosion.std_temp_halflife = 0.15f;
            explosion.std_velocity = 1f;
            explosion.std_scatter = 0.5f;
            explosion.std_particle_life = 0.4f;

            ArtManager.explosions.Add("boom", explosion);


        }
    }
}