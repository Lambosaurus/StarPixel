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
        
        public Widget()
        {
        }

        public virtual void Update()
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
        }

        public override void Update()
        {
            if (focus_entity != null)
            {
                camera.pos = focus_entity.pos;
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
