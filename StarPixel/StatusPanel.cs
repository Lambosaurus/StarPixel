using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;




namespace StarPixel
{
    class StatusPanel
    {
        Vector2 panel_size;

        RenderTarget2D surface;

        SpriteBatch batch;
        GraphicsDevice device;

        Color background_color;

        public StatusPanel(GraphicsDevice arg_device, SpriteBatch arg_batch, int width, int height)
        {
            device = arg_device;
            batch = arg_batch;

            panel_size = new Vector2(width, height);
            surface = new RenderTarget2D(device, width, height);


            background_color = new Color(32, 32, 32);
        }

        public void Draw()
        {
            device.SetRenderTarget(surface);
            device.Clear(background_color);
        }
        
        public void Blit(SpriteBatch arg_batch, Vector2 arg_pos)
        {
            arg_batch.Draw(surface, arg_pos, Color.White);
        }
    }
}
