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
            ArtExplosionResource collision_art = new ArtExplosionResource("particle");
            collision_art.coloring_method = ParticleColoring.Temp;
            collision_art.particle_count = 8;
            collision_art.particle_life = 2f;
            collision_art.size_start = new Vector2(4f, 0.8f);
            collision_art.size_end = new Vector2(0.5f, 0.5f);
            collision_art.temperature = 3000f;
            collision_art.temperature_scatter = 1500f;
            collision_art.temp_halflife = 0.5f;
            collision_art.velocity_bounce = 1.4f;
            collision_art.velocity_scatter = 0.8f;
            collision_art.particle_size_scatter = 1.4f;
            ArtManager.explosions["phys_collision"] = collision_art;



            ArtExplosionResource exp_art = new ArtExplosionResource("particle");

            
            exp_art.particle_count = 6;
            exp_art.size_start = new Vector2(2.0f, 0.35f);
            exp_art.size_end = new Vector2(0.25f, 0.35f);
            exp_art.velocity_bounce = 0.75f;
            exp_art.velocity_scatter = 0.75f;
            exp_art.particle_size_scatter = 1.5f;
            exp_art.particle_life = 1.0f;
            exp_art.coloring_method = ParticleColoring.Temp;
            //exp_art.particle_color_start = Color.White;
            //exp_art.particle_color_end = Color.LimeGreen;
            exp_art.temperature = 4500;
            exp_art.temperature_scatter = 1000;
            exp_art.temp_halflife = 0.15f; //0.1f;


            ArtManager.explosions.Add("boom", exp_art);

            Explosion exp = new Explosion(new Damage(3), 0, exp_art);



            WeaponTemplate template = new WeaponTemplate();

            template.projectile_color = new Color(1f, 0.9f, 0.5f);
            template.projectile_sprite_resource = "bullet";
            template.projectile_explosion_resource = "boom";
            template.projectile_scale = new Vector2(0.66f, 0.2f);
            template.fire_rate = 10;
            template.projectile_velocity = 6f;
            template.projectile_scatter = 0.5f;
            template.explosion = exp;

            weapon_templates.Add("shooter", template);

            ArtManager.sprites.Add("bullet", new ArtSpriteResource("bullet"));


        }
    }
}