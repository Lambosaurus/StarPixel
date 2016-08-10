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
    public class Camera
    {
        public Vector2 pos;
        public float scale;

        public Vector2 velocity;

        public RenderTarget2D surface { get; private set; }

        public SpriteBatch batch { get; private set; }
        public GraphicsDevice device { get; private set; }

        public Vector2 onscreen_res { get; private set; }
        public Vector2 res { get; private set; }
        public Vector2 midpoint { get; private set; }

        public bool DRAW_HITBOXES = false;

        public int upsample_multiplier { get; private set; } = 1;
    
        public Camera(GraphicsDevice arg_device, SpriteBatch arg_batch, int x, int y, int arg_upsample_multiplier = 1)
        {
            
            upsample_multiplier = arg_upsample_multiplier;

            pos = new Vector2(0, 0);
            scale = 1.0f * upsample_multiplier;


            batch = arg_batch;
            device = arg_device;
            surface = new RenderTarget2D(device, x * upsample_multiplier, y * upsample_multiplier);

            onscreen_res = new Vector2(x, y);
            res = onscreen_res * upsample_multiplier;
            midpoint = res / 2;
        }

        // maps a global coordinate point into the onscreen coordinate
        public Vector2 Map(Vector2 point)
        {
            return midpoint + ((point - pos) * scale);
        }

        // maps a on camera point into the global coordinate frame
        public Vector2 InverseMouseMap(Vector2 point)
        {
            return (((point * upsample_multiplier) - midpoint) / scale) + pos;

        }

        public bool Contains(Vector2 point)
        {
            Vector2 onscreen = this.Map(point);

            return onscreen.X > 0 &&
                   onscreen.Y > 0 &&
                   onscreen.X < res.X &&
                   onscreen.Y < res.Y;
    
    }

        public bool ContainsCircle(Vector2 arg_center, float arg_radius)
        {
            Vector2 onscreen = this.Map(arg_center);

            float onscreen_rad = arg_radius * scale;

            return onscreen.X + onscreen_rad > 0 &&
                   onscreen.Y + onscreen_rad > 0 &&
                   onscreen.X - onscreen_rad < res.X &&
                   onscreen.Y - onscreen_rad < res.Y;
        }

        public void MoveTo(Vector2 new_pos)
        {
            velocity = new_pos - pos;
            pos = new_pos;
        }


        public void Draw(Universe universe, List<UIMarker> markers = null ) // bool draw_all_ship_stats)
        {
            device.SetRenderTarget(surface);
            device.Clear(Color.Black);

            if (universe != null)
            {
                batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                universe.Draw(this);
                

                if (markers != null)
                {
                    foreach (UIMarker mark in markers)
                    {
                        mark.Draw(this);
                    }
                }

                batch.End();
            }
        }


        public void Blit(SpriteBatch arg_batch, Vector2 arg_pos)
        {
            if (upsample_multiplier != 1)
            {
                arg_batch.Draw(surface, arg_pos, null, Color.White, 0.0f, new Vector2(0, 0), 1.0f / upsample_multiplier, SpriteEffects.None, 0);
            }
            else
            {
                arg_batch.Draw(surface, arg_pos, Color.White);
            }
        }
    }
}
