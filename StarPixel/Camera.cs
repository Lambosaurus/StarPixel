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

        public Vector2 res;
        public Vector2 midpoint;

        public bool DRAW_HITBOXES = false;


        public Camera(GraphicsDevice arg_device, SpriteBatch arg_batch, int x, int y)
        {
            pos = new Vector2(0, 0);
            scale = 1.0f;


            batch = arg_batch;
            device = arg_device;
            surface = new RenderTarget2D(device, x, y);

            res = new Vector2(x, y);
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
    }
}
