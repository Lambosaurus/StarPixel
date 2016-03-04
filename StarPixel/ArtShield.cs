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
    public class ArtShieldResource
    {
        const float natural_log_half = -0.693147180559f; // close enough....

        string sprite_name;

        public Texture2D sprite;
        public Vector2 sprite_center;

        public ArtShieldResource(string particle_name)
        {
            sprite_name = particle_name;
        }
        
        public void Load(ContentManager content)
        {
            sprite = content.Load<Texture2D>(sprite_name);

            Vector2 size = new Vector2(sprite.Bounds.Width, sprite.Bounds.Height);
            sprite_center = size / 2;
        }

        public ArtShield New( )
        {
            ArtShield shield = new ArtShield(this);

            return shield;
        }
    }


    public class ArtShield
    {
        ArtShieldResource resource;

        Vector2 pos;
        float radius;


        int count;
        float[] depth;
        float[] speed;
        float[] angle;
        float[] alpha;
        float[] size;

        Color color = Color.LightBlue;


        public ArtShield(ArtShieldResource arg_resource)
        {
            resource = arg_resource;

            radius = 28;

            count = 100;

            depth = new float[count];
            speed = new float[count];
            angle = new float[count];
            alpha = new float[count];
            size = new float[count];

            
            for (int i = 0; i < count; i++)
            {
                float r = Utility.Rand(0.00f, 0.7f);
                depth[i] = (1 - ( r *r *r  )) * radius;
                size[i] = Utility.Rand( 0.5f,1.5f);
                speed[i] = Utility.Rand(-0.03f, 0.03f) / size[i];
                angle[i] = Utility.RandAngle();
                alpha[i] = 0; //Utility.Rand(0.2f, 1.0f);
            }
        }

        public void Update(Vector2 arg_pos)
        {
            pos = arg_pos;

            for (int i = 0; i < count; i++)
            {
                angle[i] += speed[i];
                alpha[i] *= 0.985f;
            }
        }

        public void Ping( Vector2 arg_pos )
        {
            Vector2 rel = arg_pos - pos;
            for (int i = 0; i < count; i++)
            {
                float dst = 1.3f / (size[i] * ((Utility.CosSin(angle[i], depth[i]) - rel).LengthSquared()));
                alpha[i] += dst;
                alpha[i] = Utility.Clamp(alpha[i], 0, 1);
            }
        }
        

        public bool InView(Camera camera)
        {
            // get the position of the most recent particle....
            Vector2 onscreen = camera.Map( pos );

            float cull_radius = radius * camera.scale;

            return onscreen.X + cull_radius > 0 &&
                   onscreen.Y + cull_radius > 0 &&
                   onscreen.X - cull_radius < camera.res.X &&
                   onscreen.Y - cull_radius < camera.res.Y;
        }

        public void Draw(Camera camera)
        {
            if (!InView(camera)) { return; }


            for (int i = 0; i < count; i++)
            {

                Vector2 ppos = camera.Map( pos + Utility.CosSin(angle[i], depth[i]) );

                Color k = color * alpha[i];

                camera.batch.Draw(resource.sprite, ppos, null, k, angle[i], resource.sprite_center, size[i] * new Vector2(0.3f,1.0f) * camera.scale, SpriteEffects.None, 0);
            }
        }
            
    }
}








