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

    public class ArtVentResource : ArtParticleCloudResource
    {
        public float velocity_scatter = 0.1f;
        public float velocity_ejection = 1.0f;

        public float generation_frequency = 0.4f; // This should not exceed 1.
        
        
        public ArtVentResource(string particle_name) : base(particle_name)
        {
            coloring_method = ParticleColoring.Temp;
            particle_count = -1;
        }

        
        public ArtVent New( float scale )
        {
            int count;
            if (particle_count != -1)
            {
                count = particle_count;
            }
            else
            {
                // Yea, so i estimate the count based on the theoretical maximum count
                count = (int)(scale * particle_life * GameConst.framerate * generation_frequency) + 1;
            }


            // TODO: fill out properties
            ArtVent vent = new ArtVent(this, scale, count );

            return vent;
        }
    }



    public class ArtVent : ArtParticleCloud
    {

        public float spawn_temperature_min;
        public float spawn_temperature_max;
        public float velocity_scatter;
        public float velocity_ejection;

        
        int index_start;
        int index_end;
        
        //ArtVentResource resource;

        float gen_counter;
        float gen_freq;

        public ArtVent( ArtVentResource arg_resource, float arg_size, int arg_count ) : base(arg_resource, arg_size, arg_count, new Vector2(0,0) )
        {
            gen_counter = 0.0f;

            //resource = arg_resource;

            gen_freq = arg_resource.generation_frequency;

            velocity_scatter = arg_resource.velocity_scatter * arg_size;
            velocity_ejection = arg_resource.velocity_ejection;

            spawn_temperature_min = arg_resource.temperature - arg_resource.temperature_scatter;
            spawn_temperature_max = arg_resource.temperature + arg_resource.temperature_scatter;

            radius = velocity_ejection * (arg_resource.particle_life * arg_size * GameConst.framerate);
            radius *= 2;

            index_end = 0;
            index_start = 0;
        }

        public void Generate(Vector2 pos, Vector2 vel, float p_angle, float rate)
        {
            rate = Utility.Clamp(rate, 0, 1);
            if (rate > 0.2f)
            {
                gen_counter += gen_freq;
                if (gen_counter > 1)
                {
                    gen_counter--;
                    Add(pos, vel, p_angle, Utility.Sqrt(rate));
                }
            }

        }

        public void Add(Vector2 arg_center, Vector2 vel, float p_angle, float a_scale)
        {
            center = arg_center;
            position[index_end] = arg_center;
            velocity[index_end] = vel + (Utility.CosSin(p_angle, velocity_ejection) + Utility.RandVec(velocity_scatter)) * a_scale;

            temp[index_end] = Utility.Rand(spawn_temperature_min, spawn_temperature_min);
            alpha[index_end] = 1.0f;
            angle[index_end] = p_angle;
            scale[index_end] = a_scale; //1.0f;

            // get to next write index
            index_end++;
            if (index_end >= count) { index_end = 0; }

            if (index_end == index_start)
            {
                index_start++;
                if (index_start >= count) { index_start = 0; }
            }
        }

        public override void Update()
        {
            int i = index_start;
            int c = index_end - index_start;
            if (c < 0) { c += count; }

            while (c > 0)
            {
                alpha[i] -= alpha_decay;
                if (alpha[i] < 0.0f)
                {
                    index_start = i + 1;
                    if (index_start >= count) { index_start = 0; }
                }
                else
                {
                    position[i] += velocity[i];
                    temp[i] *= temp_decay;
                }

                c--;
                i++;
                if (i >= count) { i = 0; }
            }
        }

        public override bool InView(Camera camera)
        {
            // if index_start == index_end we have no particles left!
            if (index_start != index_end)
            {
                int index_recent = index_end == 0 ? count - 1 : index_end - 1;
                // get the position of the most recent particle....
                return camera.ContainsCircle(position[index_recent], radius);
            }
            return false;
        }

        public override void Draw(Camera camera)
        {
            if (!InView(camera)) { return; }


            int i = index_start;
            int c = index_end - index_start;
            if (c < 0) { c += count; }

            while (c > 0)
            {
                Color k = ColorManager.GetThermo(temp[i]);
                this.DrawParticle(camera, i, k);

                c--;
                i++;
                if (i >= count) { i = 0; }
            }
        }

        
    }
}
