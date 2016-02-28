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
    public class ArtExplosionResource
    {
        const float natural_log_half = -0.693147180559f; // close enough....


        public int std_particle_count = 10;

        public float std_velocity = 4;
        public float std_particle_life = 0.5f;
        public float std_temp_halflife = 0.2f;
        public float std_temperature = 2000f;

        public float std_particle_width = 1.0f;
        public float std_particle_length = 1.0f;
        public float std_particle_stretch_length = 1.0f;
        public float std_particle_stretch_width = 1.0f;

        string sprite_name;
        public Texture2D sprite;
        public Vector2 sprite_center;

        public ArtExplosionResource(string particle_name)
        {
            sprite_name = particle_name;
        }

        public void Load(ContentManager content)
        {
            sprite = content.Load<Texture2D>(sprite_name);

            Vector2 size = new Vector2(sprite.Bounds.Width, sprite.Bounds.Height);
            sprite_center = size / 2;
        }

        public ArtExplosion New( float scale, Vector2 cloud_center, Vector2 cloud_velocity, Vector2 skew )
        {
            scale = Utility.Sqrt(scale);
            int count = (int)(std_particle_count * scale);

            ArtExplosion exp = new ArtExplosion(this, count, cloud_center, cloud_velocity);

            exp.temperature_decay = (float)Math.Exp(natural_log_half / (std_temp_halflife * 60 * scale)); // I KNEW THIS SHIT WOULD COME IN HANDY ONE DAY!
            exp.alpha_decay = 1.0f / (std_particle_life * scale * 60);

            exp.radius = std_velocity * (1.0f / exp.alpha_decay);

            exp.alpha_max = 1.5f;

            // this might look a bit nasty.
            for (int i = 0; i < count; i++ )
            {
                //float angle = Utility.RandAngle();
                //Vector2 vel = Utility.CosSin(angle, Utility.Rand(0.5f * std_velocity, std_velocity));

                Vector2 vel = Utility.RandVec(std_velocity) + skew/4;
                float angle = Utility.Angle(vel);

                exp.Add(vel, angle, std_temperature, Utility.Rand(1.0f, 1.5f));
            }

            exp.particle_size_0.Y = (std_particle_width * std_particle_stretch_width) * scale;
            exp.particle_size_1.Y = (std_particle_width - std_particle_width * (std_particle_stretch_width)) * scale;

            exp.particle_size_0.X = (std_particle_length * std_particle_stretch_length) * scale;
            exp.particle_size_1.X = (std_particle_length - std_particle_length * (std_particle_stretch_length)) * scale;

            return exp;
        }
    }


    public class ArtExplosion : ArtTemporary
    {
        ArtExplosionResource resource;

        int count;
        int index;

        public Vector2 particle_size_0;
        public Vector2 particle_size_1;

        Vector2 cloud_center;
        Vector2 cloud_velocity;
        public float alpha_max;

        Vector2[] position;
        Vector2[] velocity;
        float[] angle;
        float[] alpha;
        float[] temperature;

        public float alpha_decay;
        public float temperature_decay;

        public float radius;

        public ArtExplosion( ArtExplosionResource arg_resource, int arg_count, Vector2 arg_center, Vector2 arg_velocity )
        {
            resource = arg_resource;

            count = arg_count;

            cloud_center = arg_center;
            cloud_velocity = arg_velocity;

            position = new Vector2[count];
            velocity = new Vector2[count];
            angle = new float[count];
            alpha = new float[count];
            temperature = new float[count];

            index = 0;

            alpha_max = 0.0f;
        }
        
        public void Add( Vector2 vel, float arg_angle, float temp, float arg_alpha = 1.0f )
        {
            position[index] = cloud_center;
            velocity[index] = vel + cloud_velocity;
            angle[index] = arg_angle;
            temperature[index] = temp;

            alpha[index] = arg_alpha;

            index++;
        }

        public override void Update( )
        {
            cloud_center += cloud_velocity;
            alpha_max -= alpha_decay;

            for (int i = 0; i < count; i++)
            {
                if ( alpha[i] > 0 )
                {
                    alpha[i] -= alpha_decay;
                    position[i] += velocity[i];
                    temperature[i] *= temperature_decay;
                }
            }
        }

        public override bool ReadyForRemoval()
        {
            return alpha_max <= 0;
        }

        public override bool InView(Camera camera)
        {
            // get the position of the most recent particle....
            Vector2 onscreen = camera.Map( cloud_center );

            float cull_radius = radius * camera.scale;

            return onscreen.X + cull_radius > 0 &&
                   onscreen.Y + cull_radius > 0 &&
                   onscreen.X - cull_radius < camera.res.X &&
                   onscreen.Y - cull_radius < camera.res.Y;
        }

        public override void Draw(Camera camera)
        {
            if (!InView(camera)) { return; }

            for (int i = 0; i < count; i++)
            {
                if ( alpha[i] > 0 )
                {
                    float alpha2 = Utility.Clamp(alpha[i], 0, 1);
                    Color k = ColorManager.GetThermo(temperature[i]) * (alpha2);
                    Vector2 transform = particle_size_0 + (particle_size_1 * alpha2);

                    camera.batch.Draw(resource.sprite, camera.Map(position[i]), null, k, angle[i], resource.sprite_center, transform * camera.scale, SpriteEffects.None, 0);

                }
            }

        }
    }
}








