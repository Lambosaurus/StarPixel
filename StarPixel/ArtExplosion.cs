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
        public int std_particle_count = 10;

        public float std_bounce = 4;
        public float std_scatter = 2;
        public float std_particle_life = 0.5f; // in seconds

        // if you set use_particle_color to true, you only need to set particle_color
        public bool use_particle_color = false;
        public Color particle_color;

        // if use particle_color = false, then you need to set these
        public float std_temp_halflife = 0.2f;
        public float std_temperature = 2000f;

        public float std_particle_width = 1.0f; // this is a multiplier on the sprite dimensions
        public float std_particle_length = 1.0f;
        public float std_particle_size_scatter = 2.0f; // this a maximum multiplier that may be rolled. (multiplier applied ontop of existing std sizes) 
        public float std_particle_stretch_length = 1.0f; // this an additional multiplier that will be applied as the particle decays
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

            ArtExplosion exp = new ArtExplosion(this, scale, count, cloud_center, cloud_velocity);

            skew.Normalize();
            skew *= std_bounce;

            float std_scatter_min = 1.0f / std_particle_size_scatter;

            // this might look a bit nasty.
            for (int i = 0; i < count; i++ )
            {
                //float angle = Utility.RandAngle();
                //Vector2 vel = Utility.CosSin(angle, Utility.Rand(0.5f * std_velocity, std_velocity));

                Vector2 vel = Utility.RandVec(std_scatter) + skew;
                float angle = Utility.Angle(vel);
                float particle_scale = 1.0f / Utility.Rand(std_scatter_min, std_particle_size_scatter);

                exp.Add(vel * particle_scale, angle, particle_scale, std_temperature, Utility.Rand(1.0f, 1.5f));
            }

            return exp;
        }
    }


    public class ArtColorExplosion : ArtTemporary
    {

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
        float[] scale;
        float[] angle;
        float[] alpha;

        float[] temperature;

        public float alpha_decay;
        public float temperature_decay;

        public float radius;

        public ArtExplosion( ArtExplosionResource arg_resource, float size, int arg_count, Vector2 arg_center, Vector2 arg_velocity )
        {
            resource = arg_resource;

            count = arg_count;

            cloud_center = arg_center;
            cloud_velocity = arg_velocity;



            temperature_decay = (float)Math.Exp(Utility.natural_log_half / (resource.std_temp_halflife * 60 * size)); // I KNEW THIS SHIT WOULD COME IN HANDY ONE DAY!
            alpha_decay = 1.0f / (resource.std_particle_life * size * 60);

            radius = resource.std_bounce * (1.0f / alpha_decay);

            particle_size_0.Y = (resource.std_particle_width * resource.std_particle_stretch_width) * size;
            particle_size_1.Y = (resource.std_particle_width - resource.std_particle_width * (resource.std_particle_stretch_width)) * size;

            particle_size_0.X = (resource.std_particle_length * resource.std_particle_stretch_length) * size;
            particle_size_1.X = (resource.std_particle_length - resource.std_particle_length * (resource.std_particle_stretch_length)) * size;


            // TODO. Have this assigned somewhere.
            alpha_max = 1.5f;
            

            position = new Vector2[count];
            velocity = new Vector2[count];
            angle = new float[count];
            alpha = new float[count];
            temperature = new float[count];
            scale = new float[count];

            index = 0;
            
        }
        
        public void Add( Vector2 vel, float arg_angle, float arg_scale, float temp, float arg_alpha = 1.0f )
        {
            position[index] = cloud_center;
            velocity[index] = vel + cloud_velocity;
            angle[index] = arg_angle;
            temperature[index] = temp;
            scale[index] = arg_scale;

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
            return camera.ContainsCircle(cloud_center, radius);
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
                    Vector2 transform = (particle_size_0 + (particle_size_1 * alpha2));
                    transform.X *= scale[i];
                    
                    camera.batch.Draw(resource.sprite, camera.Map(position[i]), null, k, angle[i], resource.sprite_center, transform * (camera.scale), SpriteEffects.None, 0);

                }
            }

        }
    }
}








