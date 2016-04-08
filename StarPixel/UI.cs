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
    public class UI
    {
        List<Widget> widgets;

        GraphicsDevice device;
        SpriteBatch batch;


        WidgetCamera camera_widget;


        Universe focus_universe;
        Ship focus_ship;

        public UI( GraphicsDevice arg_device, SpriteBatch arg_batch, int width, int height )
        {
            widgets = new List<Widget>();

            batch = arg_batch;
            device = arg_device;

            camera_widget = new WidgetCamera( new Camera(arg_device, arg_batch, width, height) );

            widgets.Add(camera_widget);
        }

        public void FocusUniverse(Universe arg_universe)
        {
            focus_universe = arg_universe;
            camera_widget.universe = arg_universe;
        }

        public void FocusShip(Ship arg_ship)
        {
            focus_ship = arg_ship;
            camera_widget.focus_entity = arg_ship;
        }

        public void Update()
        {
            foreach (Widget widget in widgets)
            {
                widget.Update();
            }
        }

        public void Draw()
        {
            foreach (Widget widget in widgets)
            {
                widget.PreDraw();
            }

            device.SetRenderTarget(null);
            batch.Begin();

            foreach (Widget widget in widgets)
            {
                widget.Draw(batch);
            }

            batch.End();

        }
    }
}
