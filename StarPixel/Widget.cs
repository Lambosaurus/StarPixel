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
        
        public Widget()
        {
        }

        public virtual void Update(InputState current, InputState old, bool mouse_focus)
        {

        }
        

        // a chance to do sprite batch stuff before the draw
        public virtual void PreDraw()
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

        public WidgetCamera(Camera arg_camera)
        {
            camera = arg_camera;
            size = camera.res / camera.upsample_multiplier;
        }
        
        public override void Update( InputState current, InputState old, bool mouse_focus )
        {
            if (focus_entity != null)
            {
                camera.MoveTo(focus_entity.pos);
            }

            if (mouse_focus)
            {
                if (focus_entity == null && current.mb.RightButton == ButtonState.Pressed)
                {
                    Vector2 mouse_delta = current.pos - old.pos;
                    camera.MoveTo(camera.pos - (mouse_delta * camera.upsample_multiplier / camera.scale));
                }

                if (current.mb.ScrollWheelValue > old.mb.ScrollWheelValue)
                {
                    camera.scale *= 1.1f;
                }
                else if (current.mb.ScrollWheelValue < old.mb.ScrollWheelValue)
                {
                    camera.scale /= 1.1f;
                }
            }
        }

        public override void PreDraw()
        {
            camera.Draw(universe, draw_stat_rings);
        }
        
        public override void Draw(SpriteBatch arg_batch)
        {
            camera.Blit(arg_batch, pos);
        }
    }


    public class WidgetShipStatus
    {

    }

}
