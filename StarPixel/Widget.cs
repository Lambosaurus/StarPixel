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


        public virtual void Update(InputState inputs)
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


        public bool InWindow(Vector2 point)
        {
            return Utility.PointInWindow(point - pos, size);
        }


        public virtual void ClickCallback(Vector2 pos, InputState.MouseButton button)
        {
        }

        public virtual void DragStartCallback(Vector2 pos, InputState.MouseButton button)
        {
        }

        public virtual void DragCallback(Vector2 pos)
        {
        }

        public virtual void DragReleaseCallback(Vector2 pos)
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
        
        public override void Update(InputState inputs )
        {
            base.Update(inputs);

            
            if (focus_entity != null)
            {
                camera.MoveTo(focus_entity.pos);
            }
            
            /*
            if (mouse_focus)
            {
                if (focus_entity == null && inputs.mb.RightButton == ButtonState.Pressed)
                {
                    Vector2 mouse_delta = inputs.pos - inputs.old_pos;
                    camera.MoveTo(camera.pos - (mouse_delta * camera.upsample_multiplier / camera.scale));
                }
            }
            */
        }

        public override void ClickCallback(Vector2 point, InputState.MouseButton button)
        {
            Vector2 mapped_pos = camera.InverseMouseMap(point - pos);
            if (ui.mode == UI.ControlMode.Observe )
            {
                Physical new_phys = universe.PhysAtPoint(mapped_pos);
                if (new_phys != null)
                {
                    ui.FocusPhys(new_phys);
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
