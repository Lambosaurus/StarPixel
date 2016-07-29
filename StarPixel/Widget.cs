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
    public class Widget
    {
        public Vector2 pos;
        public Vector2 size;
        
        public Widget(Vector2 arg_size)
        {
            size = arg_size;
        }

        public virtual void Update(InputState inputs, bool mouse_focus)
        {
        }
        

        // a chance to do sprite batch stuff before the draw
        public virtual void Render()
        {

        }

        // draw content onto the screen through the given sprite batch
        public virtual void Draw(SpriteBatch arg_batch)
        {
        }
    }



    public class WidgetCamera : Widget
    {
        public Camera camera;

        public Universe universe;
        public Entity focus_entity;
        public bool draw_stat_rings;

        UI ui;

        public List<UIMarker> markers = null;

        public WidgetCamera( UI arg_ui, Camera arg_camera) : base(arg_camera.res / arg_camera.upsample_multiplier)
        {
            ui = arg_ui;
            camera = arg_camera;
        }
        
        public override void Update(InputState inputs, bool mouse_focus )
        {
            if (focus_entity != null)
            {
                camera.MoveTo(focus_entity.pos);
            }

            if (mouse_focus)
            {
                if (focus_entity == null && inputs.mb.RightButton == ButtonState.Pressed)
                {
                    Vector2 mouse_delta = inputs.pos - inputs.old_pos;
                    camera.MoveTo(camera.pos - (mouse_delta * camera.upsample_multiplier / camera.scale));
                }

                if (inputs.mb.ScrollWheelValue > inputs.old_mb.ScrollWheelValue)
                {
                    camera.scale *= 1.1f;
                }
                else if (inputs.mb.ScrollWheelValue < inputs.old_mb.ScrollWheelValue)
                {
                    camera.scale /= 1.1f;
                }

                if (universe != null)
                {
                    ui.MouseCallBack(universe, camera.InverseMouseMap(inputs.pos) );
                }
            }
        }

        public override void Render(  )
        {
            camera.Draw(universe, markers);
            
        }
        
        public override void Draw(SpriteBatch arg_batch)
        {
            camera.Blit(arg_batch, pos);
        }
    }


    public class WidgetShipStatus : Widget
    {
        RenderTarget2D surface;
        SpriteBatch batch;
        GraphicsDevice device;

        Color background_color;

        Ship focus_ship;
        bool ship_change;

        public WidgetShipStatus(GraphicsDevice arg_device, SpriteBatch arg_batch, int width, int height) : base(new Vector2(width, height))
        {
            surface = new RenderTarget2D(arg_device, width, height);
            batch = arg_batch;
            device = arg_device;

            background_color = new Color(32,32,32);
        }

        public void FocusShip(Ship ship)
        {
            focus_ship = ship;   
        }

        public override void Render()
        {
            device.SetRenderTarget(surface);
            device.Clear(background_color);

            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            if (focus_ship != null)
            {
            }

            batch.End();
        }

        public override void Draw(SpriteBatch arg_batch)
        {
            arg_batch.Draw(surface, pos, Color.White);
        }

    }

}
