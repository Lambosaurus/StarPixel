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
        public bool windowed = false;
        
        public Vector2 pos { get; protected set; }
        public Vector2 size { get; protected set; }

        public Camera camera { get; protected set; }

        protected static Color default_background_color = new Color(32, 32, 32, 128);


        protected const int padding = 10;
        protected const int window_bar_height = 30;
        protected static Color default_window_bar_color = new Color(64, 64, 64, 128);


        Vector2 bar_drag_anchor;
        bool bar_dragging = false;

        public Widget(Vector2 arg_size, Vector2 arg_pos, SpriteBatch arg_batch) : this(new Camera(arg_size, arg_batch), arg_pos )
        {
            camera.background_color = new Color(32, 32, 32, 128);
        }

        public Widget(Camera arg_camera, Vector2 arg_pos)
        {
            camera = arg_camera;
            SetPos(arg_pos);
            size = camera.output_res;
        }

        /*
        public Widget(GraphicsDevice device, SpriteBatch batch, Vector2 default_size, float arg_uiscale, int upscale = GameConst.upsample)
        {
            int width = (int)Math.Ceiling(default_size.X * arg_uiscale);
            int height = (int)Math.Ceiling(default_size.Y * arg_uiscale);
            
            size = new Vector2(width, height);

            camera = new RenderCamera(device, batch, width, height, upscale);
        }
        */

        public virtual void SetPos(Vector2 arg_pos)
        {
            pos = arg_pos;
            camera.SetPos(pos);
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
            DrawWindow();
        }

        protected void DrawWindow()
        {
            camera.DrawBackground();

            if (windowed)
            {
                ArtPrimitive.DrawBoxFilled(camera.pos, new Vector2(size.X, -window_bar_height) + camera.pos, default_window_bar_color);
            }
        }


        public bool InWindow(Vector2 point)
        {
            // if we are windowed, we include the window_bar_height in our hit scan.
            if (windowed) { return Utility.PointInWindow(point - pos + new Vector2(0, window_bar_height), size + new Vector2(0, window_bar_height)); }
            return Utility.PointInWindow(point - pos, size);
        }
        
        public virtual void ClickCallback(Vector2 point, InputState.MouseButton button)
        {
        }

        public virtual void DragStartCallback(Vector2 point, InputState.MouseButton button)
        {
            if (windowed && point.Y < pos.Y)
            {
                bar_dragging = true;
                bar_drag_anchor = pos - point;
            }
        }

        // this is distinct from the HoverCallback, because as long as the drag is held, it will
        // persist on the widget, even if the cursor leaves the widget boundary
        public virtual void DragCallback(Vector2 point)
        {
            if (bar_dragging) { SetPos( point + bar_drag_anchor); }
        }

        public virtual void DragReleaseCallback(Vector2 point)
        {
            bar_dragging = false;
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
        public Universe universe;
        public Entity focus_entity;
        public bool draw_stat_rings;

        UI ui;

        public List<UIMarker> markers = null;

        public Vector2 drag_origin;
        public Vector2 drag_last;
        public bool drag_selection = false;
        
        public bool following = false;

        
        public WidgetCamera( UI arg_ui, RenderCamera arg_camera) : base(arg_camera, new Vector2(0,0))
        {
            ui = arg_ui;
        }
        
        public override void Update(InputState inputs )
        {
            base.Update(inputs);

            
            if (focus_entity != null)
            {
                camera.SetPos(focus_entity.pos);
            }
        }

        public override void ClickCallback(Vector2 point, InputState.MouseButton button)
        {
            if (button == InputState.MouseButton.Left)
            {
                Vector2 mapped_pos = camera.InverseMouseMap(point - pos);
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

            List<Physical> selected = universe.PhysInBox(camera.InverseMouseMap(drag_origin - pos), camera.InverseMouseMap(drag_last - pos) );

            if (selected.Count != 0)
            {
                ui.FocusPhys(selected[0]);
            }

        }

        public override void Render(  )
        {
            //camera.Draw(universe, markers);
            ((RenderCamera)camera).Begin();

            universe.Draw(camera);

            if (markers != null)
            {
                foreach (UIMarker marker in markers)
                {
                    marker.Draw(camera);
                }
            }
            ((RenderCamera)camera).End();
        }
        
        public override void Draw(SpriteBatch arg_batch)
        {
            ((RenderCamera)camera).Blit(arg_batch, pos);

            if (drag_selection)
            {
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


    public class WidgetSubCamera : Widget
    {
        public Universe target_universe;
        public Physical target;

        RenderCamera subcamera;
        
        public WidgetSubCamera(Vector2 arg_size, Vector2 arg_pos, SpriteBatch arg_batch, GraphicsDevice arg_device) : base( arg_size, arg_pos, arg_batch )
        {
            windowed = true;

            int sub_x = (int)size.X - padding * 2;
            int sub_y = (int)size.Y - padding * 2;
            subcamera = new RenderCamera(arg_device, arg_batch, sub_x, sub_y, GameConst.upsample);
            subcamera.SetScale(3);
        }
        
        public override void Render()
        {
            subcamera.Begin();

            if (target != null)
            {
                subcamera.SetPos(target.pos);
            }
            if (target_universe != null)
            {
                target_universe.Draw(subcamera);
            }

            subcamera.End();
        }

        public override void Draw(SpriteBatch arg_batch)
        {
            base.Draw(arg_batch);

            subcamera.Blit(camera.batch, camera.Map( new Vector2(padding, padding) ) );
        }
    }

    public class WidgetShipStatus : Widget
    {
        Physical target;

        float target_scale;

        float armor_width = 8f;
        float component_minimum_radius = 3f;

        HitboxArmorMarker armor;
        WidgetElementBar shield_bar;
        
        static Color dark_grey = new Color(32, 32, 32);

        CenteredCamera ship_camera;

        public WidgetShipStatus(Vector2 arg_size, Vector2 arg_pos, SpriteBatch arg_batch) : base( arg_size, arg_pos, arg_batch )
        {
            windowed = true;

            Vector2 pad_vec = new Vector2(padding, padding);
            ship_camera = new CenteredCamera(camera.internal_res - 2 * pad_vec, camera.batch);
            ship_camera.SetPos(pad_vec + camera.pos);
            
            shield_bar = new WidgetElementBar( new Vector2(padding, size.Y - (padding + armor_width)), new Vector2(size.X - (2*padding), armor_width));
            shield_bar.full_color = ColorManager.shield_color;
            shield_bar.empty_color = ColorManager.HP_DEAD;
        }

        public void Focus(Physical new_target)
        {
            target = new_target;
            Vector2 scale = (ship_camera.output_res) / (target.sprite.resource.size * target.sprite.resource.scale);
            target_scale = Utility.Min(scale.X, scale.Y);
            ship_camera.SetScale(target_scale);
            
                        
            if (target.armor != null)
            {
                armor = new HitboxArmorMarker( (HitboxPolygon)target.hitbox, target.armor,
                   armor_width / (target_scale * 2.0f), armor_width / target_scale, armor_width);
            }
            else
            {
                armor = null;
            }
        }
        

        public override void Draw(SpriteBatch batch)
        {
            base.Draw(batch);

            // we set this each frame, because we cant be bothered watching drag events
            ship_camera.SetPos( camera.pos + new Vector2(padding, padding) );
            
            if (target != null)
            {
                
                target.sprite.Draw(ship_camera, Vector2.Zero, 0.0f);
                //if (target is Ship)
                //{
                //    ((Ship)target).paint_sprite.Draw(camera, Vector2.Zero, 0.0f);
                //}

                if (armor != null)
                {
                    armor.Draw(ship_camera);
                }
                
                if (target is Ship)
                {
                    SpriteTileSheet sheet = Symbols.component_sheet;

                    foreach (Component component in ((Ship)target).ListComponents())
                    {
                        Vector2 center = ship_camera.Map(component.pos);
                        float scale = ((component.size * 3) * camera.scale);
                        //float scale = ((component.size * 2f) + component_minimum_radius);
                        ArtPrimitive.DrawCircle(center, dark_grey, (scale + 2f));
                        ArtPrimitive.DrawCircle(center, ColorManager.HPColor(component.health / component.max_health), scale);

                        sheet.Draw(camera.batch, (int)component.base_template.symbol, center, 2f * scale / sheet.tile_width, dark_grey, 0.0f);
                    }
                }

                if (target.shield != null)
                {
                    shield_bar.full_color = (target.shield.active) ? ColorManager.shield_color : ColorManager.dead_shield_color;
                    shield_bar.fill = target.shield.integrity / target.shield.max_integrity;
                }

            }


            if (target != null)
            {
                shield_bar.Draw(camera);
            }

            //camera.Blit(batch, pos);
        }
    }
}





