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

    // everything art needs to draw itself 
    public class Camera
    {
        public Vector2 pos;
        public float scale;

        public RenderTarget2D surface;

        public SpriteBatch batch;
        public GraphicsDevice device;

        public Vector2 onscreen_res;
        public Vector2 res;
        public Vector2 midpoint;

        public bool DRAW_HITBOXES = false;

        public int upsample_multiplier = 1;

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
        public Vector2 InverseMap(Vector2 point)
        {
            return ((point - midpoint) / scale) + pos;

        }

        public bool Contains(Vector2 point)
        {
            // TODO: make work.
            return true;
        }

 

        public void Draw(Universe universe)
        {

            device.SetRenderTarget(surface);
            device.Clear(Color.Black);
            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            universe.Draw(this);

            batch.End();
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


    public class StatusCamera : Camera
    {
        public Color shield_bar_color;

        public bool autoscale = true;

        public float bar_pad = 2;
        public float shield_bar_width = 4;


        public StatusCamera(GraphicsDevice arg_device, SpriteBatch arg_batch, int size, int arg_upsample_multiplier = 2) : base (  arg_device, arg_batch, size, size, arg_upsample_multiplier)
        {
            shield_bar_color = Color.Lerp(Color.DeepSkyBlue, Color.Blue, 0.5f);
        }



        public void DrawTarget(Ship ship, float angle = -MathHelper.PiOver2)
        {
            device.SetRenderTarget(surface);
            device.Clear(Color.Transparent);
            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            if (autoscale)
            {
                scale = (midpoint.X) / ship.template.shield_radius;
            }

            ship.hull_sprite.Update(new Vector2(0, 0), angle);
            ship.hull_sprite.Draw(this);
            ship.hull_sprite.Update(ship.pos, ship.angle);

            if (ship.paint_sprite != null)
            {
                ship.paint_sprite.Update(new Vector2(0, 0), angle);
                ship.paint_sprite.Draw(this);
                ship.paint_sprite.Update(ship.pos, ship.angle);
            }

            if (ship.shield != null)
            {
                ArtLine.DrawArcU(this, midpoint, angle,
                    MathHelper.TwoPi * ship.shield.integrity / ship.shield.max_integrity,
                    midpoint.X - (shield_bar_width* upsample_multiplier / 2) - (bar_pad* upsample_multiplier),
                    shield_bar_color , shield_bar_width* upsample_multiplier);
            }

            batch.End();
        }

        
    }
}
