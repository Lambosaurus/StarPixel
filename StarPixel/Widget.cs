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

        public Vector2 pos;
        public Vector2 size { get; protected set; }

        public RenderCamera camera { get; protected set; }
        public float uiscale { get; protected set; }

        public static Color default_background_color = new Color(32, 32, 32, 128);

        public Widget(RenderCamera arg_camera, float arg_uiscale)
        {
            camera = arg_camera;

            uiscale = arg_uiscale;
            size = camera.output_res;
        }

        public Widget(GraphicsDevice device, SpriteBatch batch, Vector2 default_size, float arg_uiscale, int upscale = GameConst.upsample)
        {
            int width = (int)Math.Ceiling(default_size.X * arg_uiscale);
            int height = (int)Math.Ceiling(default_size.Y * arg_uiscale);

            uiscale = arg_uiscale;
            size = new Vector2(width, height);

            camera = new RenderCamera(device, batch, width, height, upscale);
            //camera.Traditional(uiscale);
            
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
        public Universe universe;
        public Entity focus_entity;
        public bool draw_stat_rings;

        UI ui;

        public List<UIMarker> markers = null;

        public Vector2 drag_origin;
        public Vector2 drag_last;
        public bool drag_selection = false;
        
        public bool following = false;

        
        public WidgetCamera( UI arg_ui, RenderCamera arg_camera) : base(arg_camera, 1.0f)
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
            camera.Begin();

            universe.Draw(camera);

            if (markers != null)
            {
                foreach (UIMarker marker in markers)
                {
                    marker.Draw(camera);
                }
            }
            camera.End();
        }
        
        public override void Draw(SpriteBatch arg_batch)
        {
            camera.Blit(arg_batch, pos);

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


    public class WidgetShipStatus : Widget
    {
        Physical target;

        float target_scale;

        float armor_width = 8f;
        Vector2 padding = new Vector2(4,4);
        float component_minimum_radius = 3f;

        HitboxArmorMarker armor;
        WidgetElementBar shield_bar;


        const int default_width = 160;
        const int default_height = 160;
        static Color dark_grey = new Color(32, 32, 32);

        public WidgetShipStatus(GraphicsDevice arg_device, SpriteBatch arg_batch, float arg_uiscale = 1.0f) : base( arg_device, arg_batch, new Vector2(default_width, default_height), arg_uiscale)
        {
            padding += new Vector2(armor_width, armor_width);

            camera.background_color = new Color(32, 32, 32, 128);

            shield_bar = new WidgetElementBar( new Vector2(10, 150), new Vector2(140, armor_width));
            shield_bar.full_color = ColorManager.shield_color;
            shield_bar.empty_color = ColorManager.HP_DEAD;
        }

        public void Focus(Physical new_target)
        {
            target = new_target;
            Vector2 scale = (camera.internal_res - (padding*2* camera.ui_feature_scale)) / (target.sprite.resource.size * target.sprite.resource.scale);
            target_scale = Utility.Min(scale.X, scale.Y);
            camera.SetScale(target_scale);
            
            if (target.armor != null)
            {
                armor = new HitboxArmorMarker( (HitboxPolygon)target.hitbox, target.armor,
                   armor_width / 2.0f, armor_width);
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
                camera.SetPos(Vector2.Zero);
                camera.SetScale(target_scale);

                target.sprite.Draw(camera, Vector2.Zero, 0.0f);
                //if (target is Ship)
                //{
                //    ((Ship)target).paint_sprite.Draw(camera, Vector2.Zero, 0.0f);
                //}

                if (armor != null)
                {
                    armor.Draw(camera, armor_width);
                }
                
                if (target.armor != null)
                {
                    //target.hitbox.Draw(camera, target.armor, 8.0f, Vector2.Zero, 0.0f);
                }

                if (target is Ship)
                {
                    SpriteTileSheet sheet = Symbols.component_sheet;

                    foreach (Component component in ((Ship)target).ListComponents())
                    {
                        Vector2 center = camera.Map(component.pos);
                        float scale = ((component.size * 2f) / camera.ui_feature_scale) + (component_minimum_radius);
                        ArtPrimitive.DrawCircle(center, dark_grey, scale + (2f));
                        ArtPrimitive.DrawCircle(center, ColorManager.HPColor(component.health / component.max_health), scale);

                        sheet.Draw(camera.batch, (int)component.base_template.symbol, center, 2f * scale / (camera.ui_feature_scale * sheet.tile_width), dark_grey, 0.0f);
                    }
                }

                if (target.shield != null)
                {
                    shield_bar.full_color = (target.shield.active) ? ColorManager.shield_color : ColorManager.dead_shield_color;
                    shield_bar.fill = target.shield.integrity / target.shield.max_integrity;
                }
                
            }

            //camera.Traditional(uiscale);

            if (target != null)
            { 
                shield_bar.Draw(camera);
            }

            camera.End();
        }

        public override void Draw(SpriteBatch batch)
        {
            camera.Blit(batch, pos);
        }
    }
}
