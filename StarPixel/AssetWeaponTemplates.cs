﻿using System;
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

        // explosion to be multiplied by collision force
        public static Explosion collision_explosion;

        public static void GenerateAssets()
        {
            ArtExplosionResource collision_cloud = new ArtExplosionResource("particle");
            collision_cloud.coloring_method = ParticleColoring.Temp;
            collision_cloud.particle_count = 8;
            collision_cloud.particle_life = 2f;
            collision_cloud.size_start = new Vector2(4f, 0.8f);
            collision_cloud.size_end = new Vector2(0.5f, 0.5f);
            collision_cloud.temperature = 3000f;
            collision_cloud.temperature_scatter = 1500f;
            collision_cloud.temp_halflife = 0.4f;
            collision_cloud.velocity_bounce = 1.4f;
            collision_cloud.velocity_scatter = 0.8f;
            collision_cloud.particle_size_scatter = 1.4f;
            collision_cloud.bidirectional_scatter = true;
            ArtManager.explosions["phys_collision"] = collision_cloud;

            collision_explosion = new Explosion(new Damage(0.5f), ArtManager.explosions["phys_collision"], 1.0f / 100);



            /*
            ArtParticleResource collision_particle = new ArtParticleResource("particle_large");
            collision_particle.coloring_method = ParticleColoring.Temp;
            collision_particle.particle_life = 0.6f;
            collision_particle.size_start = new Vector2(2f, 2f);
            collision_particle.size_end = new Vector2(4f, 4f);
            collision_particle.temperature = 3000f;
            collision_particle.temperature_scatter = 1500f;
            ArtManager.particles["phys_collision"] = collision_particle;
            */


            // SHOOTER - END //
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

            Explosion exp = new Explosion(new Damage(3), exp_art);



            WeaponTemplate template = new WeaponTemplate();

            template.projectile_color = new Color(1f, 0.9f, 0.5f);
            template.projectile_sprite_resource = "bullet";
            template.projectile_explosion_resource = "boom";
            template.projectile_scale = new Vector2(0.66f, 0.2f);
            template.fire_rate = 8;
            template.projectile_velocity = 6.5f;
            template.projectile_scatter = 0.5f;
            template.explosion = exp;

            weapon_templates.Add("shooter", template);

            ArtManager.sprites.Add("bullet", new ArtSpriteResource("bullet"));
            // SHOOTER - START //


            // BOOMER - START //
            exp_art = new ArtExplosionResource("particle");

            exp_art.particle_count = 20;
            exp_art.size_start = new Vector2(6.0f, 1f);
            exp_art.size_end = new Vector2(1f, 1f);
            exp_art.velocity_bounce = 2f;
            exp_art.velocity_scatter = 1.5f;
            exp_art.particle_size_scatter = 1.7f;
            exp_art.particle_life = 1.2f;
            exp_art.coloring_method = ParticleColoring.Temp;
            //exp_art.particle_color_start = Color.White;
            //exp_art.particle_color_end = Color.LimeGreen;
            exp_art.temperature = 5000;
            exp_art.temperature_scatter = 2000;
            exp_art.temp_halflife = 0.2f;


            ArtManager.explosions.Add("boom2", exp_art);

            exp = new Explosion(new Damage(20), exp_art);


            template = new WeaponTemplate();

            template.projectile_color = new Color(1f, 0.4f, 0.2f);
            template.projectile_sprite_resource = "bullet";
            template.projectile_explosion_resource = "boom2";
            template.projectile_scale = new Vector2(2.5f, 0.5f);
            template.fire_rate = 0.9f;
            template.projectile_velocity = 7.5f;
            template.projectile_scatter = 0.1f;
            template.explosion = exp;

            weapon_templates.Add("boomer", template);

            //ArtManager.sprites.Add("bullet", new ArtSpriteResource("bullet"));
            // BOOMER - END //
        }
    }
}