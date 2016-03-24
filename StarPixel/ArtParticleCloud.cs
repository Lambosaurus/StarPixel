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
    public enum ParticleColoring { Blend, Temp }

    public class ArtParticleCloudResource
    {
        public int particle_count = 10;

        // if you set use_particle_color to true, you only need to set particle_color
        public ParticleColoring coloring_method = ParticleColoring.Blend;
        public Color particle_color_start;
        public Color particle_color_end;

        // if use particle_color = false, then you need to set these
        public float temp_halflife = 0.2f;
        public float temperature = 2000f;

        
        public float particle_life = 2.0f; // the time in seconds the particles will live for

        public Vector2 size_start = new Vector2(1.0f, 1.0f);
        public Vector2 size_end = new Vector2(1.0f, 1.0f);
        
        string sprite_name;
        public Texture2D sprite;
        public Vector2 sprite_center;

        public ArtParticleCloudResource(string particle_name)
        {
            sprite_name = particle_name;
        }

        public void Load(ContentManager content)
        {
            sprite = content.Load<Texture2D>(sprite_name);

            Vector2 size = new Vector2(sprite.Bounds.Width, sprite.Bounds.Height);
            sprite_center = size / 2;
        }
    }

    
    public class ArtParticleCloud : ArtTemporary
    {
        protected ArtParticleCloudResource resource;

        protected int count;

        protected Vector2 particle_size_0;
        protected Vector2 particle_size_1;


        protected float alpha_max; // must be set by derivitive, or on particle spawning?

        protected Vector2[] velocity;
        protected Vector2[] position;
        protected float[] scale;
        protected float[] angle;
        protected float[] alpha;

        protected float[] temp;
        protected float temp_decay;

        protected float alpha_decay;

        protected Vector2 center;
        public float radius; // must be set by derivitive

        public ArtParticleCloud( ArtParticleCloudResource arg_resource, float size, int arg_count, Vector2 arg_center)
        {
            resource = arg_resource;

            count = arg_count;

            center = arg_center;

            alpha_max = 1.0f;

            alpha_decay = 1.0f / (resource.particle_life * size * GameConst.framerate);

            
            particle_size_0 = resource.size_end * size;
            particle_size_1 = (resource.size_start - resource.size_end) * size;



            velocity = new Vector2[count];
            position = new Vector2[count];
            angle = new float[count];
            alpha = new float[count];
            scale = new float[count];

            if ( resource.coloring_method == ParticleColoring.Temp )
            {
                temp = new float[count];
                temp_decay = Utility.DecayConstant(resource.temp_halflife * GameConst.framerate * size);
            }
        }

        public bool Visible()
        {
            return alpha_max > 0.05;
        }

        public void UpdateCenter(Vector2 arg_center)
        {
            center = arg_center;
        }

        public override void Update()
        {
            alpha_max -= alpha_decay;


            if (resource.coloring_method == ParticleColoring.Temp)
            {
                for (int i = 0; i < count; i++)
                {
                    if (alpha[i] > 0)
                    {
                        alpha[i] -= alpha_decay;
                        position[i] += velocity[i];
                        temp[i] *= temp_decay;
                    }
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    if (alpha[i] > 0)
                    {
                        alpha[i] -= alpha_decay;
                        position[i] += velocity[i];
                    }
                }
            }
        }

        

        public override bool ReadyForRemoval()
        {
            return !this.Visible();
        }


        public override bool InView(Camera camera)
        {
            return this.Visible() && camera.ContainsCircle(center, radius);
        }

        public void DrawParticle(Camera camera, int i, Color color)
        {
            Color k = color*alpha[i];
            Vector2 transform = (particle_size_0 + (particle_size_1 * alpha[i]));
            transform *= scale[i];

            camera.batch.Draw(resource.sprite, camera.Map(position[i]), null, k, angle[i], resource.sprite_center, transform * (camera.scale), SpriteEffects.None, 0);
        }

        public override void Draw(Camera camera)
        {
            if (!InView(camera)) { return; }

            if (resource.coloring_method == ParticleColoring.Blend)
            {
                for (int i = 0; i < count; i++)
                {
                    if (alpha[i] > 0)
                    {
                        Color k = Color.Lerp(resource.particle_color_end, resource.particle_color_start, alpha[i]);
                        this.DrawParticle(camera, i, k);
                    }
                }
            }
            else if (resource.coloring_method == ParticleColoring.Temp)
            {
                for (int i = 0; i < count; i++)
                {
                    if (alpha[i] > 0)
                    {
                        Color k = ColorManager.GetThermo(temp[i]);
                        this.DrawParticle(camera, i, k);
                    }
                }
            }

        }
    }
}








