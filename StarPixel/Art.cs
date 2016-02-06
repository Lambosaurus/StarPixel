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

        public static void Load(ContentManager content)
        {

            sprite_default = new ArtSpriteResource("default_sprite");
            sprite_default.Load(content);

            foreach (ArtSpriteResource sprite in sprites.Values)
            {
                sprite.Load(content);
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
    }


    public class ArtThermoparticleResource
    {
        public int max_particle_count;
        public float alpha_decay;
        public float temperature_decay;

        string sprite_name;
        public Texture2D sprite;
        
        public ArtThermoparticleResource(string particle_name, int max_particles, float alpha_decay_rate, float temp_decay_constant)
        {
            sprite_name = particle_name;
            max_particle_count = max_particles;
            alpha_decay = alpha_decay_rate;
            temperature_decay = temp_decay_constant;
        }

        public ArtThermoparticle New()
        {
            return new ArtThermoparticle(this);
        }

        public void Load(ContentManager content)
        {
            sprite = content.Load<Texture2D>(sprite_name);
        }
    }

    public class ArtThermoparticle
    {
        public string name;

        Vector2[] position;
        Vector2[] velocity;
        float[] alpha;
        float[] temperature;

        int write_index;
        int read_index;

        ArtThermoparticleResource resource;


        public ArtThermoparticle(ArtThermoparticleResource arg_resource)
        {
            resource = arg_resource;

            position = new Vector2[resource.max_particle_count];
            velocity = new Vector2[resource.max_particle_count];
            alpha = new float[resource.max_particle_count];
            temperature = new float[resource.max_particle_count];
        }

        public void Add(Vector2 new_position, Vector2 new_velocity, float temperature)
        {
        }

        public void Draw( Camera camera )
        {
            Color color = new Color();
            color.A = 255;
            camera.batch.Draw(resource.sprite, camera.Map(position[0]), color);
        }
    }


    public class ArtSpriteResource
    {
        public Texture2D sprite;
        public string name;

        public Vector2 size;
        public Vector2 center;

        public float radius;

        public float scale;

        public ArtSpriteResource(string arg_name, float arg_scale = 1.0f)
        {
            name = arg_name;
            scale = arg_scale; // TODO i dont even want the scale parameter.
        }
        

        public void Load( ContentManager content )
        {
            sprite = content.Load<Texture2D>(name);
            size = new Vector2(sprite.Bounds.Width, sprite.Bounds.Height);
            center = size / 2;

            radius = center.Length();
        }

        public ArtSprite New(  )
        {
            return new ArtSprite(this);
        }
    }

    public class ArtSprite
    {
        ArtSpriteResource resource;

        public Vector2 pos;
        public float angle;

        public ArtSprite( ArtSpriteResource arg_resource )
        {
            resource = arg_resource;
        }

        public bool InView( Camera camera )
        {
            Vector2 onscreen = camera.Map(pos);
            float cull_radius = resource.radius * camera.scale;

            return onscreen.X + cull_radius > 0 &&
                   onscreen.Y + cull_radius > 0 &&
                   onscreen.X - cull_radius < camera.res.X &&
                   onscreen.Y - cull_radius < camera.res.Y;
        }

        public void Draw( Camera camera )
        {
            if (!InView(camera)) { return; }

            camera.batch.Draw(resource.sprite, camera.Map(pos), null, Color.White, angle, resource.center, camera.scale * resource.scale, SpriteEffects.None, 0);
            
        }
    }
}





