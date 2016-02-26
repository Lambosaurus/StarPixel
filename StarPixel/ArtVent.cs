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

    public class ArtVentResource
    {
        string sprite_name;

        public Texture2D sprite;
        public Vector2 sprite_center;

        const float natural_log_half = -0.693147180559f; // close enough....

        public float std_particle_life = 1.0f;
        public float std_temp_halflife = 1.0f;
        public float std_velocity_scatter = 0.1f;
        public float std_temperature_scatter = 0.0f;

        public float std_ejection_velocity = 1.0f;
        public float std_ejection_temperature = 1000f;

        public float std_particle_width = 1.0f;
        public float std_particle_length = 1.0f;
        public float std_particle_stretch_length = 1.0f;
        public float std_particle_stretch_width = 1.0f;


        public int std_particle_count;


        public ArtVentResource(string spritename)
        {
            sprite_name = spritename;
            
        }

        public void Load(ContentManager content)
        {
            sprite = content.Load<Texture2D>(sprite_name);

            Vector2 size = new Vector2(sprite.Bounds.Width, sprite.Bounds.Height);
            sprite_center = size / 2;
        }

        public ArtVent New( float scale )
        {
            // TODO: fill out properties
            ArtVent vent = new ArtVent(this, (int)(std_particle_count*scale) );

            vent.temperature_decay = (float)Math.Exp(natural_log_half / (std_temp_halflife * 60 * scale)); // I KNEW THIS SHIT WOULD COME IN HANDY ONE DAY!
            vent.alpha_decay = 1.0f / (std_particle_life * scale * 60);

            vent.temperature_scatter = std_temperature_scatter;
            vent.velocity_scatter = std_velocity_scatter * scale;

            vent.particle_size_0.Y = (std_particle_width * std_particle_stretch_width) * scale;
            vent.particle_size_1.Y = (std_particle_width - std_particle_width * (std_particle_stretch_width)) * scale;

            vent.particle_size_0.X = (std_particle_length * std_particle_stretch_length) * scale;
            vent.particle_size_1.X = (std_particle_length - std_particle_length *(std_particle_stretch_length)) * scale;

            vent.ejection_temperature = std_ejection_temperature;
            vent.ejection_velocity = std_ejection_velocity;

            return vent;
        }
    }



    public class ArtVent
    {
        public float alpha_decay;
        public float temperature_decay;

        public float ejection_velocity;
        public float ejection_temperature;

        //float position_scatter;
        public float velocity_scatter;
        public float temperature_scatter;


        public Vector2 particle_size_0;
        public Vector2 particle_size_1;


        Vector2[] position;
        Vector2[] velocity;
        float[] alpha;
        float[] temperature;
        float[] angle;

        int index_max;
        int index_start;
        int index_end;

        ArtVentResource resource;

        float generator_counter;

        public ArtVent( ArtVentResource arg_resource, int particle_max )
        {
            // TODO: fill out properties

            generator_counter = 0.0f;

            resource = arg_resource;
            
            index_max = particle_max;
            index_end = 0;
            index_start = 0;

            position = new Vector2[particle_max];
            velocity = new Vector2[particle_max];
            alpha = new float[particle_max];
            temperature = new float[particle_max];
            angle = new float[particle_max];
        }

        public void Generate(Vector2 pos, Vector2 vel, float p_angle, float rate)
        {
            if (rate > 0.0f)
            {
                generator_counter += rate;
                while (generator_counter > 1)
                {
                    generator_counter--;
                    Add(pos, vel, p_angle);
                }
            }
        }

        public void Add(Vector2 pos, Vector2 vel, float p_angle)
        {
            position[index_end] = pos;
            velocity[index_end] = vel + Utility.CosSin(p_angle, ejection_velocity) + Utility.RandVec(velocity_scatter);

            temperature[index_end] = ejection_temperature + (temperature_scatter * Utility.Rand(-1, 1));
            alpha[index_end] = 1.0f;
            angle[index_end] = p_angle;

            // get to next write index
            index_end++;
            if (index_end >= index_max) { index_end = 0; }

            if (index_end == index_start)
            {
                index_start++;
                if (index_start >= index_max) { index_start = 0; }
            }
        }

        public void Update()
        {
            int i = index_start;
            int c = index_end - index_start;
            if (c < 0) { c += index_max; }

            while (c > 0)
            {
                alpha[i] -= alpha_decay;
                if (alpha[i] < 0.0f)
                {
                    index_start = i + 1;
                    if (index_start >= index_max) { index_start = 0; }
                }
                else
                {
                    position[i] += velocity[i];
                    temperature[i] *= temperature_decay;
                }

                c--;
                i++;
                if (i >= index_max) { i = 0; }
            }
        }

        public bool InView(Camera camera)
        {
            return true; // TODO: yep. Nothing wrong this this. We fine boys.
        }

        public void Draw(Camera camera)
        {
            if (!InView(camera)) { return; }


            int i = index_start;
            int c = index_end - index_start;
            if (c < 0) { c += index_max; }

            while (c > 0)
            {
                Color k = ColorManager.GetThermo(temperature[i]) * (alpha[i]);

                //Vector2 transform = new Vector2( particle_length_0 + (particle_length_1*alpha[i]), particle_length_0 + (particle_length_1 * alpha[i]));
                Vector2 transform = particle_size_0 + (particle_size_1*alpha[i]);

                // TODO: fill this shit out.
                camera.batch.Draw(resource.sprite, camera.Map(position[i]), null, k, angle[i], resource.sprite_center, transform * camera.scale, SpriteEffects.None, 0);

                c--;
                i++;
                if (i >= index_max) { i = 0; }
            }
        }

        
    }
}
