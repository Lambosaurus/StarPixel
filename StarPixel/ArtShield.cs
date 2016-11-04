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
    public class ArtShieldResource : ArtParticleResource
    {
        public float particle_velocity;
           

        public float shield_particle_speed; // is this an angular speed? I do not recall.

        // particle_life will behave as a half life

        public float shield_particle_size_min;
        public float shield_particle_size_max;

        public float shield_particle_halflife;

        public float reform_particle_rate;
        
        public ArtShieldResource(string particle_name) : base(particle_name)
        {
            // these are for the pop effect
            size_start = new Vector2(0.3f, 1.0f);
            size_end = new Vector2(1.0f, 0.3f);
            
        }
        

        public ArtShield New(float radius, float size)
        {
            float rad_sq = Utility.Sqrt(radius);
            int count = (int)(particle_count * rad_sq);

            ArtShield shield = new ArtShield(this, count, size, radius);
            
            return shield;
        }
    }


    public class ArtShield
    {
        const int UPDATES_PER_ANGLEWRAP = 4000;

        ArtShieldResource resource;

        Vector2 pos;
        float radius;
        float scalar;

        public float particle_decay;

        int count;
        public float[] depth;
        public float[] speed;
        public float[] angle;
        public float[] alpha;
        public float[] size;

        float total_alpha = 0.0f;

        int angle_wrap_counter = 0;
        
        int reform_index;
        float reform_counter;
        float reform_rate;

        public ArtShield(ArtShieldResource arg_resource, int arg_count, float arg_size, float arg_radius)
        {
            resource = arg_resource;

            radius = arg_radius;
            scalar = arg_size;

            count = arg_count;

            depth = new float[count];
            speed = new float[count];
            angle = new float[count];
            alpha = new float[count];
            size = new float[count];

            reform_rate = (arg_resource.reform_particle_rate / arg_size) / GameConst.framerate;
            
            float rad_sq = Utility.Sqrt(radius);


            particle_decay = Utility.DecayConstant(resource.shield_particle_halflife * GameConst.framerate * arg_size);

            arg_size = Utility.Sqrt(arg_size);

            for (int i = 0; i < count; i++)
            {
                // this picks a radius, which is heavily encouraged to be near max
                // and will be in the outer 21%
                float r = Utility.Rand(0.00f, 0.6f);
                depth[i] = (1 - (r * r * r)) * radius;

                // particle size scalar
                float mass = Utility.Rand(1.0f);
                size[i] = arg_size * (resource.shield_particle_size_min + mass * resource.shield_particle_size_max);

                // note that the speed is an angular value
                // the speed is randomly generated, but then modified by the mass
                // this should encourage, but not force, heavy particles to be slow
                speed[i] = Utility.Rand(-resource.shield_particle_speed, resource.shield_particle_speed) / (rad_sq * (0.5f + mass));

                angle[i] = Utility.RandAngle();
            }
        }

        public void Update(Vector2 arg_pos)
        {
            pos = arg_pos;


            if (reform_index > 0)
            {
                reform_counter += reform_rate;

                while ((reform_index > 0) && (reform_counter > 0))
                {
                    reform_counter--;
                    alpha[--reform_index] = 1;
                    total_alpha = 1f;
                }
            }


            // why bother doing particles if particles not visible?
            if (total_alpha < 0.01) { return; }
            
            total_alpha *= particle_decay;
            
            for (int i = 0; i < count; i++)
            {
                angle[i] += speed[i];
                alpha[i] *= particle_decay;
            }


            // this means after 4000 angle updates, the angles will be wrapped back into a safe range
            // Otherwise floating point errors will accrue
            // 4000 updates is a little over a minute. Keep in mind updates only occurr if the shield is visible
            if (angle_wrap_counter++ > UPDATES_PER_ANGLEWRAP)
            {
                angle_wrap_counter = 0;
                for (int i = 0; i < count; i++)
                {
                    angle[i] = Utility.WrapAngle(angle[i]);
                }
            }

        }

        public void Ping( Vector2 arg_pos, float dmg )
        {
            total_alpha = 1.0f;

            Vector2 rel = arg_pos - pos;

            float dmg_const = dmg * 2;

            for (int i = 0; i < count; i++)
            {
                float dst = dmg_const / ( size[i] * ((Utility.CosSin(angle[i], depth[i]) - rel).LengthSquared()));
                alpha[i] += dst;
                alpha[i] = Utility.Clamp(alpha[i], 0, 1);
            }
        }

        public ArtTemporary Pop( Vector2 arg_velocity )
        {
            reform_index = 0;
            total_alpha = 0.0f;

            ArtShieldPop pop = new ArtShieldPop(resource, scalar, radius, count, pos, arg_velocity);

            for (int i = 0; i < count; i++)
            {
                Vector2 sincos = Utility.CosSin(angle[i]);
                float vel = Utility.Rand(resource.particle_velocity);
                pop.Add(sincos * depth[i], sincos * vel, angle[i], size[i], Utility.Clamp(alpha[i] + Utility.Rand(0.25f)) );

                alpha[i] = 0;
            }


            return pop;
        }
        

        public void Reform()
        {
            reform_index = count;
        }


        public bool InView(Camera camera)
        {
            // shield not visible if alpha very low
            if (total_alpha < 0.05) { return false; }
            return camera.ContainsCircle(pos, radius);
        }

        public void Draw(Camera camera)
        {
            if (!InView(camera)) { return; }


            for (int i = 0; i < count; i++)
            {

                Vector2 ppos = camera.Map( pos + Utility.CosSin(angle[i], depth[i]) );

                if (alpha[i] > 0.05)
                {
                    //Color k = color * alpha[i];
                    Color k = Color.Lerp(resource.particle_color_end, resource.particle_color_start, alpha[i]) * alpha[i];
                    camera.batch.Draw(resource.sprite, ppos, null, k, angle[i], resource.sprite_center, size[i] * new Vector2(0.3f, 1.0f) * camera.scale, SpriteEffects.None, 0);
                }
            }
        }
            
    }



    public class ArtShieldPop : ArtParticleCloud
    {
        
        int index;

        Vector2 cloud_velocity;



        public ArtShieldPop(ArtShieldResource arg_resource, float arg_size, float shield_radius, int arg_count, Vector2 arg_center, Vector2 arg_velocity) : base(arg_resource, 1.0f, arg_count, arg_center)
        {
            alpha_decay /= arg_size;
            radius = arg_resource.particle_velocity * (1.0f / alpha_decay);
            radius += shield_radius;

            index = 0;

            cloud_velocity = arg_velocity;
        }

        public void Add(Vector2 offset, Vector2 vel, float arg_angle, float arg_scale, float arg_alpha = 1.0f)
        {
            position[index] = center + offset;
            velocity[index] = vel + cloud_velocity;
            angle[index] = arg_angle;
            scale[index] = arg_scale;
            alpha[index] = arg_alpha;

            index++;
        }
        
        public override void Update()
        {
            center += cloud_velocity;
            base.Update();
        }
    }
}








