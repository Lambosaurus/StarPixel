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


        public virtual void ClickCallback(Vector2 point, InputState.MouseButton button)
        {
        }

        public virtual void DragStartCallback(Vector2 point, InputState.MouseButton button)
        {
        }

        // this is distinct from the HoverCallback, because as long as the drag is held, it will
        // persist on the widget, even if the cursor leaves the widget boundary
        public virtual void DragCallback(Vector2 point)
        {
        }

        public virtual void DragReleaseCallback(Vector2 point)
        {
        }

        // returns the cursor sprite
        public virtual void HoverCallback(Vector2 point)
        {
        }

        public virtual void ScrollCallback( bool forwards )
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

        public Vector2 drag_origin;
        public Vector2 drag_last;
        public bool drag_selection = false;
        
        public bool following = false;

        
        public WidgetCamera( UI arg_ui, Camera arg_camera) : base(arg_camera.res / arg_camera.upsample)
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
        }

        public override void ClickCallback(Vector2 point, InputState.MouseButton button)
        {
            if (button == InputState.MouseButton.Left)
            {
                Vector2 mapped_pos = camera.InverseMap(point - pos);
                if (ui.mode == UI.ControlMode.Observe)
                {
                    Physical new_phys = universe.PhysAtPoint(mapped_pos);
                    if (new_phys != null)
                    {
                        ui.FocusPhys(new_phys);
                    }
                }
            }
        }

        public override void DragStartCallback(Vector2 point, InputState.MouseButton button)
        {
            if (button == InputState.MouseButton.Left)
            {
                drag_selection = true;

                drag_origin = point;
                drag_last = point;
            }
        }

        public override void DragCallback(Vector2 point)
        {
            drag_last = point;
            /*
            Vector2 delta = drag_last - point;
            drag_last = point;

            camera.MoveTo(camera.pos + (delta * camera.scale));
            */
        }

        public override void DragReleaseCallback(Vector2 point)
        {
            drag_selection = false;

            drag_last = point;

            Utility.RectSort(ref drag_origin, ref drag_last);

            List<Physical> selected = universe.PhysInBox(camera.InverseMap(drag_origin - pos), camera.InverseMap(drag_last - pos) );

            if (selected.Count != 0)
            {
                ui.FocusPhys(selected[0]);
            }

        }

        public override void Render(  )
        {
            camera.Draw(universe, markers);
        }
        
        public override void Draw(SpriteBatch arg_batch)
        {
            camera.Blit(arg_batch, pos);

            if (drag_selection)
            {
                ArtPrimitive.Setup(arg_batch, 1);
                ArtPrimitive.DrawBoxOutline(drag_origin, drag_last, ui.cursor_color, 1.0f);
                ArtPrimitive.DrawBoxFilled(drag_origin, drag_last, ui.cursor_color * 0.2f);
            }
        }

        public override void ScrollCallback(bool forwards)
        {
            if (forwards)
            {
                camera.SetScale( camera.scale * GameConst.scroll_zoom );
            }
            else
            {
                camera.SetScale(camera.scale / GameConst.scroll_zoom);
            }

        }
    }


    public class WidgetShipStatus : Widget
    {
        Camera camera;

        Physical target;

        Vector2 padding = new Vector2(8,8);

        HitboxArmorMarker armor;

        public WidgetShipStatus(GraphicsDevice arg_device, SpriteBatch arg_batch, int width, int height) : base(new Vector2(width, height))
        {
            camera = new Camera(arg_device, arg_batch, width, height, GameConst.upsample);
            camera.background_color = new Color(32, 32, 32); ;
        }

        public void Focus(Physical new_target)
        {
            target = new_target;
            Vector2 scale = (camera.res - (padding*2)) / (target.sprite.resource.size * target.sprite.resource.scale);
            camera.SetScale( Utility.Min(scale.X, scale.Y) );

            if (target.armor != null)
            {
                armor = new HitboxArmorMarker( (HitboxPolygon)target.hitbox, target.armor, camera.pixel_constant * 4f);
            }
            else
            {
                armor = null;
            }
        }

        public override void Render()
        {
            camera.Begin();

            if (target != null)
            {
                target.sprite.Draw(camera, Vector2.Zero, 0.0f);
                //if (target is Ship)
                //{
                //    ((Ship)target).paint_sprite.Draw(camera, Vector2.Zero, 0.0f);
                //}

                if (armor != null)
                {
                    armor.Draw(camera, 8f);
                }
                
                if (target.armor != null)
                {
                    //target.hitbox.Draw(camera, target.armor, 8.0f, Vector2.Zero, 0.0f);
                }
            }

            camera.End();
        }

        public override void Draw(SpriteBatch batch)
        {
            camera.Blit(batch, pos);
        }
    }
}
