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

        public static Dictionary<string, ArtThermoparticleResource> thermoparticles = new Dictionary<string, ArtThermoparticleResource>();
        static ArtThermoparticleResource thermoparticle_default;

        public static void Load(ContentManager content)
        {

            sprite_default = new ArtSpriteResource("default_sprite");
            sprite_default.Load(content);

            foreach (ArtSpriteResource sprite in sprites.Values)
            {
                sprite.Load(content);
            }


            thermoparticle_default = new ArtThermoparticleResource("particle", 200, 1, 0.98f );
            thermoparticle_default.Load(content);

            foreach (ArtThermoparticleResource thermoparticle in thermoparticles.Values)
            {
                thermoparticle.Load(content);
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

        public static ArtThermoparticle NewArtThermoparticle(string key)
        {
            if (thermoparticles.ContainsKey(key))
            {
                return thermoparticles[key].New();
            }

            return thermoparticle_default.New();
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

            thermo_color_constant = thermo_scale_length / (max_temperature+1);
        }

        public static Color GetThermo( float temp )
        {
            if ( temp < 0 ) { return thermo_colors[0]; }
            else if (temp > max_temperature) { return thermo_colors[thermo_scale_length - 1]; }
            return thermo_colors[(int)(thermo_color_constant * temp)];
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

        public void Add(Vector2 new_position, Vector2 new_velocity, float new_temperature)
        {
            position[write_index] = new_position;
            velocity[write_index] = new_velocity;
            temperature[write_index] = new_temperature;
            alpha[write_index] = 255;

            // get to next write index
            write_index++;
            if (write_index >= resource.max_particle_count) { write_index = 0; }
        }

        public void Update()
        {
            bool needs_wrap = read_index >= write_index;

            int i = read_index;
            while ( i < write_index || needs_wrap )
            {
                alpha[i] -= resource.alpha_decay;
                if (alpha[i] < 0.0f)
                {
                    read_index = i;
                }
                else
                {
                    position[i] += velocity[i];
                    temperature[i] *= resource.temperature_decay;
                }

                i++;
                if (i >= resource.max_particle_count) { i = 0; needs_wrap = false; }
            }
        }

        public bool InView(Camera camera)
        {
            return true; // yep. Nothing wrong this this. We fine boys.
        }

        public void Draw(Camera camera)
        {
            if (!InView(camera)) { return; }

            bool needs_wrap = read_index >= write_index;

            int i = read_index;
            while (i < write_index || needs_wrap)
            {
                Color k = ColorManager.GetThermo(temperature[i]);
                k.A = (byte)alpha[i];
                camera.batch.Draw(resource.sprite, camera.Map(position[i]), k );

                i++;
                if (i >= resource.max_particle_count) { i = 0; needs_wrap = false; }
            }

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





