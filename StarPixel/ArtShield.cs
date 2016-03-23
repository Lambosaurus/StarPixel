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
    public class ArtShieldResource : ArtParticleCloudResource
    {
        
        public float particle_speed; // is this an angular speed? I do not recall.
                                     // I am considering replaceing angular speeds with a dynamic force based speed

        // particle_life will behave as a half life

        public float particle_size_min;
        public float particle_size_max;

        public ArtShieldResource(string particle_name) : base(particle_name)
        {
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

        public float particle_decay;

        int count;
        public float[] depth;
        public float[] speed;
        public float[] angle;
        public float[] alpha;
        public float[] size;

        public Color color;

        float total_alpha = 0.0f;

        int angle_wrap_counter = 0;

        public ArtShield(ArtShieldResource arg_resource, int arg_count, float arg_size, float arg_radius)
        {
            resource = arg_resource;

            radius = arg_radius;

            count = arg_count;

            depth = new float[count];
            speed = new float[count];
            angle = new float[count];
            alpha = new float[count];
            size = new float[count];

            color = resource.particle_color_start;

            float rad_sq = Utility.Sqrt(radius);


            particle_decay = Utility.DecayConstant(resource.particle_life * GameConst.framerate * arg_size);

            arg_size = Utility.Sqrt(arg_size);

            for (int i = 0; i < count; i++)
            {
                // this picks a radius, which is heavily encouraged to be near max
                // and will be in the outer 21%
                float r = Utility.Rand(0.00f, 0.6f);
                depth[i] = (1 - (r * r * r)) * radius;

                // particle size scalar
                float mass = Utility.Rand(1.0f);
                size[i] = arg_size * (resource.particle_size_min + mass * resource.particle_size_max);

                // note that the speed is an angular value
                // the speed is randomly generated, but then modified by the mass
                // this should encourage, but not force, heavy particles to be slow
                speed[i] = Utility.Rand(-resource.particle_speed, resource.particle_speed) / (rad_sq * (0.5f + mass));

                angle[i] = Utility.RandAngle();
            }
        }

        public void Update(Vector2 arg_pos)
        {
            pos = arg_pos;

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
            for (int i = 0; i < count; i++)
            {
                float dst = dmg / ( 2 * size[i] * ((Utility.CosSin(angle[i], depth[i]) - rel).LengthSquared()));
                alpha[i] += dst;
                alpha[i] = Utility.Clamp(alpha[i], 0, 1);
            }
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
                    Color k = color * alpha[i];
                    camera.batch.Draw(resource.sprite, ppos, null, k, angle[i], resource.sprite_center, size[i] * new Vector2(0.3f, 1.0f) * camera.scale, SpriteEffects.None, 0);
                }
            }
        }
            
    }
}








