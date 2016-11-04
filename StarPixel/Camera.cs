using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace StarPixel
{
    // The information that art needs to draw themselves.
    public abstract class Camera
    {
        public Vector2 pos { get; protected set; }
        public float scale { get; protected set; } = 1.0f;
        public float ui_feature_scale { get; protected set; } = 1.0f;

        public Vector2 internal_res { get; protected set; }
        public Vector2 internal_midpoint { get; protected set; }
        protected int internal_scale = 1;
        public Vector2 output_res { get; protected set; }
        protected Vector2 mapping_center;
        
        public SpriteBatch batch { get; private set; }

        public Color background_color = Color.Black;

        public bool DRAW_HITBOXES = false;

        public Camera(Vector2 arg_res, SpriteBatch arg_batch)
        {
            output_res = arg_res;
            internal_res = output_res;
            internal_midpoint = internal_res / 2.0f;
            batch = arg_batch;

            BuildMap();
        }

        public Vector2 Map(Vector2 point)
        {
            return (point + mapping_center) * scale;
        }

        public bool Contains(Vector2 point)
        {
            Vector2 onscreen = this.Map(point);

            return onscreen.X > 0 &&
                   onscreen.Y > 0 &&
                   onscreen.X < internal_res.X &&
                   onscreen.Y < internal_res.Y;
        }

        public bool ContainsCircle(Vector2 arg_center, float arg_radius)
        {
            Vector2 onscreen = this.Map(arg_center);

            float onscreen_rad = arg_radius * scale;

            return onscreen.X + onscreen_rad > 0 &&
                   onscreen.Y + onscreen_rad > 0 &&
                   onscreen.X - onscreen_rad < internal_res.X &&
                   onscreen.Y - onscreen_rad < internal_res.Y;
        }

        protected abstract void BuildMap();

        // maps a on camera point into the global coordinate frame
        // This is not the functions you would expect from InverseMap, this is calculated post blit... i think. i cant remember.
        public Vector2 InverseMouseMap(Vector2 point)
        {
            return (((point * internal_scale) - internal_midpoint) / scale) + pos;
        }
        

        public void SetPos(Vector2 new_pos)
        {
            pos = new_pos;
            BuildMap();
        }

        public void SetScale(float arg_scale)
        {
            scale = arg_scale;
            BuildMap();
        }

    }


    public class RenderCamera : Camera
    {
        public RenderTarget2D surface { get; private set; }
        public GraphicsDevice device { get; private set; }
        
            
        public RenderCamera(GraphicsDevice arg_device, SpriteBatch arg_batch, int x, int y, int arg_upsample_multiplier = 1) : base(new Vector2(x,y), arg_batch)
        {
            internal_scale = arg_upsample_multiplier;
            internal_res *= internal_scale;
            internal_midpoint *= internal_scale;
            ui_feature_scale = internal_scale;

            SetScale(1.0f * internal_scale);
            
            device = arg_device;
            surface = new RenderTarget2D(device, x * internal_scale, y * internal_scale);
            
            BuildMap();
        }

        protected override void BuildMap()
        {
            mapping_center = (internal_midpoint / scale) - pos;
            //mapping_center = pos;
        }
        
        
        public void Begin()
        {
            device.SetRenderTarget(surface);
            device.Clear(background_color);

            ArtPrimitive.SetBatch(batch);
            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            //batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
        }

        public void End()
        {
            batch.End();
        }
        
        public void Blit(SpriteBatch arg_batch, Vector2 arg_pos)
        {
            if (internal_scale != 1)
            {
                arg_batch.Draw(surface, arg_pos, null, Color.White, 0.0f, new Vector2(0, 0), 1.0f / internal_scale, SpriteEffects.None, 0);
            }
            else
            {
                arg_batch.Draw(surface, arg_pos, Color.White);
            }
        }

        /*
        public void Draw(Universe universe, List<UIMarker> markers = null )
        {
            Begin();

            if (universe != null)
            {
                universe.Draw(this);
                

                if (markers != null)
                {
                    foreach (UIMarker mark in markers)
                    {
                        mark.Draw(this);
                    }
                }
            }

            End();
        }
        */
    }
}
