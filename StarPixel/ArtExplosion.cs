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
    public class ArtExplosionResource : ArtParticleCloudResource
    {
        
        public float velocity_bounce = 4;
        public float velocity_scatter = 2;
        public float minimum_spawn_alpha = 0.5f;
        
        public float particle_size_scatter = 2.0f; // this a maximum multiplier that may be rolled. (multiplier applied ontop of existing std sizes) 

        public bool bidirectional_scatter = false;
        

        public ArtExplosionResource(string particle_name) : base (particle_name)
        {
        }
        
        
        public ArtExplosion New( float scale, Vector2 cloud_center, Vector2 cloud_velocity, Vector2 skew )
        {
            scale = Utility.Sqrt(scale);
            int count = (int)(particle_count * scale);
            if (bidirectional_scatter) { count *= 2; }


            ArtExplosion exp = new ArtExplosion(this, scale, count, cloud_center, cloud_velocity);
            
            skew *= velocity_bounce;

            // TODO: Make this equation make sense one day.
            float scatter_min = 1.0f / particle_size_scatter;

            float temp_min = temperature - (temperature_scatter);
            float temp_max = temperature + (temperature_scatter);
            float temp = Utility.Rand(temperature - temperature_scatter, temperature + temperature_scatter);

            // this might look a bit nasty.
            for (int i = 0; i < count; i++ )
            {
                //float angle = Utility.RandAngle();
                //Vector2 vel = Utility.CosSin(angle, Utility.Rand(0.5f * std_velocity, std_velocity));

                Vector2 vel = Utility.RandVec(velocity_scatter) + skew;
                if (bidirectional_scatter) { vel = Utility.RandBool() ? vel : -vel; }
                float angle = Utility.Angle(vel);
                float particle_scale = 1.0f / Utility.Rand(scatter_min, particle_size_scatter);
                float alpha = Utility.Rand(minimum_spawn_alpha, 1.0f);
                

                if (coloring_method == ParticleColoring.Temp)
                {
                    exp.AddTemp(vel * particle_scale, angle, particle_scale, temp, alpha);
                }
                else
                {
                    exp.Add(vel * particle_scale, angle, particle_scale, alpha);
                }
            }

            return exp;
        }
    }


    public class ArtExplosion : ArtParticleCloud
    {
        //ArtExplosionResource resource;
        
        int index;

        Vector2 cloud_velocity;
        
        public ArtExplosion( ArtExplosionResource arg_resource, float size, int arg_count, Vector2 arg_center, Vector2 arg_velocity ) : base ( arg_resource, size, arg_count, arg_center )
        {
            //resource = arg_resource;
        
            radius = arg_resource.velocity_bounce * (1.0f / alpha_decay);
            
            index = 0;

            cloud_velocity = arg_velocity;
        }

        public void Add(Vector2 vel, float arg_angle, float arg_scale, float arg_alpha = 1.0f)
        {
            position[index] = center;
            velocity[index] = vel + cloud_velocity;
            angle[index] = arg_angle;
            scale[index] = arg_scale;
            alpha[index] = arg_alpha;

            index++;
        }

        public void AddTemp( Vector2 vel, float arg_angle, float arg_scale, float arg_temp, float arg_alpha = 1.0f )
        {
            position[index] = center;
            velocity[index] = vel + cloud_velocity;
            angle[index] = arg_angle;
            temp[index] = arg_temp;
            scale[index] = arg_scale;
            alpha[index] = arg_alpha;

            index++;
        }

        public override void Update( )
        {
            center += cloud_velocity;
            base.Update();
        }
    }
}








