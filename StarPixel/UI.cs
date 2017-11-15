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
    public class InputState
    {
        public Vector2 pos { get; private set; }
        public KeyboardState kb { get; private set; }
        public MouseState mb { get; private set; }
        public Vector2 old_pos { get; private set; }
        public KeyboardState old_kb { get; private set; }
        public MouseState old_mb { get; private set; }


        public InputState()
        {
            this.GetNewInputs();
        }

        void GetNewInputs()
        {
            mb = Mouse.GetState();
            kb = Keyboard.GetState();
            pos = new Vector2(mb.X, mb.Y);
        }

        public void Update()
        {
            old_kb = kb;
            old_mb = mb;
            old_pos = pos;

            GetNewInputs();
        }

        public bool KeyDownEvent( Keys key )
        {
            return kb.IsKeyDown(key) && !old_kb.IsKeyDown(key);
        }

        public enum MouseButton { None = -1, Left, Middle, Right };
        public enum ButtonEvent { Up = 0, Pressed = 1, Released = 2, Down = 3}; // this enum order is important.
        
        public ButtonEvent MouseButtonEvent(MouseButton button)
        {
            ButtonState b_old;
            ButtonState b_new;

            if (button == MouseButton.Left) { b_old = old_mb.LeftButton; b_new = mb.LeftButton; }
            else if (button == MouseButton.Right) { b_old = old_mb.RightButton; b_new = mb.RightButton; }
            else { b_old = old_mb.MiddleButton; b_new = mb.MiddleButton; }
            
            return (ButtonEvent)((int)b_new + (((int)b_old) * 2)); // Mmmmm.. much more delicious than an if statement...

            //return (b_new != b_old) && (evt == b_new);
        }

        public bool MouseButtonState(MouseButton button, ButtonState evt)
        {
            ButtonState b = (button == MouseButton.Left) ? mb.LeftButton : ((button == MouseButton.Right) ? mb.RightButton : mb.MiddleButton);
            return b == evt;
        }
    }
    
    public class UI
    {
        public enum ControlMode { Control, Observe }

        public enum Cursors { None, Pointer, Select }

        List<Widget> widgets; // ordered by depth: First object is deepest

        GraphicsDevice device;
        SpriteBatch batch;

        public InputState inputs { get; private set; }
        
        WidgetCamera camera_widget;
        WidgetShipStatus status_widget;
        WidgetSubCamera subcam_widget;
        
        Universe focus_universe;
        Physical focus_phys;
        IntellegenceHuman focus_ai;

        public ControlMode mode = ControlMode.Observe;


        public Color cursor_color = Color.Red;
        
        Widget drag_widget = null;
        InputState.MouseButton drag_button = InputState.MouseButton.None;
        Vector2 drag_origin;
        bool dragging = false;
       

        public UI( GraphicsDevice arg_device, SpriteBatch arg_batch, int width, int height )
        {
            widgets = new List<Widget>();

            batch = arg_batch;
            device = arg_device;

            camera_widget = new WidgetCamera( this, new RenderCamera(arg_device, arg_batch, width, height, GameConst.upsample) ); // UPSAMPLE_MULTILIER
            status_widget = new WidgetShipStatus( new Vector2( 200,200 ), new Vector2(0,height - 200), batch );
            subcam_widget = new WidgetSubCamera(new Vector2(300, 300), new Vector2(300, 300), batch, device);

            widgets.Add(camera_widget);
            widgets.Add(status_widget);
            widgets.Add(subcam_widget);
            
            inputs = new InputState();
        }

        public void Start()
        {
            inputs.Update();
        }

        public void FocusUniverse(Universe arg_universe)
        {
            focus_universe = arg_universe;
            camera_widget.universe = arg_universe;

            subcam_widget.target_universe = arg_universe;
        }

        public void FocusPhys(Physical phys)
        {
            focus_phys = phys;
            camera_widget.focus_entity = phys;
            status_widget.Focus(phys);
        }

        public void GiveHumanAIHandle(IntellegenceHuman arg_ai)
        {
            focus_ai = arg_ai;
        }


        public void GiveInputsToAi( )
        {
            if (focus_ai != null)
            {
                focus_ai.thrust.X = (inputs.kb.IsKeyDown(Keys.W) ? 1.0f : 0f) - (inputs.kb.IsKeyDown(Keys.S) ? 1.0f : 0f);
                focus_ai.thrust.Y = (inputs.kb.IsKeyDown(Keys.D) ? 1.0f : 0f) - (inputs.kb.IsKeyDown(Keys.A) ? 1.0f : 0f);
                focus_ai.torque = (inputs.kb.IsKeyDown(Keys.E) ? 1.0f : 0f) - (inputs.kb.IsKeyDown(Keys.Q) ? 1.0f : 0f);
            }
        }

        /*
        public void UniverseClick( Universe universe, Vector2 position )
        {
            if (mode == ControlMode.Control)
            {
                if (universe == focus_universe && focus_ai != null)
                {
                    focus_ai.weapon_target = position;
                    focus_ai.firing = (inputs.mb.LeftButton == ButtonState.Pressed);
                }
            }
        }
        */
        

        public Widget GetFocusedWidget()
        {
            // iterate in reverse, because the last widget is the topmost 
            for (int i = widgets.Count-1; i >= 0; i--)
            {
                if (widgets[i].InWindow(inputs.pos))
                {
                    return widgets[i];
                }
            }
            return null;
        }

        public void HackyManualKeyEvents()
        {

            if (inputs.kb.IsKeyDown(Keys.LeftShift))
            {
                if (focus_phys != null && focus_phys is Ship)
                {
                    Ship ship = (Ship)focus_phys;
                    if (ship.ai != null)
                    {
                        List<UIMarker> markers = ship.ai.GetUiMarkers();
                        if (markers != null)
                        {
                            camera_widget.markers = markers;
                        }
                    }
                }
            }

            if (inputs.kb.IsKeyDown(Keys.Tab))
            {
                if (camera_widget.markers == null) { camera_widget.markers = new List<UIMarker>(); }

                foreach (Physical phys in focus_universe.physicals)
                {
                    if (phys is Ship)
                    {
                        if (camera_widget.camera.ContainsCircle(phys.pos, phys.radius))
                        {
                            camera_widget.markers.Add(new MarkerPhysicalDefence(phys));
                        }
                    }
                }
            }

            if (inputs.KeyDownEvent(Keys.F3))
            {
                camera_widget.camera.DRAW_HITBOXES = !camera_widget.camera.DRAW_HITBOXES;
            }

            if (inputs.KeyDownEvent(Keys.OemTilde))
            {
                mode = (mode == ControlMode.Control) ? ControlMode.Observe : ControlMode.Control;
            }
        }


        public void ButtonEventManager(Widget focused, InputState.MouseButton button)
        {
            InputState.ButtonEvent evt = inputs.MouseButtonEvent(button);
            if ( evt == InputState.ButtonEvent.Pressed )
            {
                if (drag_button == InputState.MouseButton.None)
                {
                    drag_button = button;
                    drag_origin = inputs.pos;
                    drag_widget = focused;
                }
                //drag_widget.DragStartCallback(inputs.pos, button);
            }
            else if ( evt == InputState.ButtonEvent.Released )
            {
                if (drag_button == button)
                {
                    drag_button = InputState.MouseButton.None;

                    if (dragging)
                    {
                        dragging = false;
                        drag_widget.DragReleaseCallback(inputs.pos);
                    }
                }
                if (!dragging)
                {
                    focused.ClickCallback(inputs.pos, button);
                }
            }

            if (drag_button == button)
            {
                if ( !dragging && drag_origin != inputs.pos )
                {
                    dragging = true;
                    drag_widget.DragStartCallback(drag_origin, button);
                }
                if (dragging)
                {
                    drag_widget.DragCallback(inputs.pos);
                }
            }
        }


        public void Update()
        {
            inputs.Update();
            
            camera_widget.markers = null;

            Widget focused = GetFocusedWidget();


            if (focused != null)
            {
                ButtonEventManager(focused, InputState.MouseButton.Left);
                ButtonEventManager(focused, InputState.MouseButton.Right);

                if (inputs.mb.ScrollWheelValue > inputs.old_mb.ScrollWheelValue)
                {
                    focused.ScrollCallback(true);
                }
                else if (inputs.mb.ScrollWheelValue < inputs.old_mb.ScrollWheelValue)
                {
                    focused.ScrollCallback(false);
                }
            }

            HackyManualKeyEvents();

            foreach (Widget widget in widgets)
            {
                widget.Update(inputs);
            }

            

            if (mode == ControlMode.Control)
            {
                GiveInputsToAi();
            }

            camera_widget.draw_stat_rings = (inputs.kb.IsKeyDown(Keys.Tab));
        }

        public void Draw()
        {
            foreach (Widget widget in widgets)
            {
                widget.Render();
            }

            device.SetRenderTarget(null);
            batch.Begin();
            
            foreach (Widget widget in widgets)
            {
                widget.Draw(batch);
            }
            
            batch.Draw(cursor_point, inputs.pos, cursor_color);
        
            batch.End();

        }
        


        // Resources
        static Texture2D cursor_target;
        static Texture2D cursor_select;
        static Texture2D cursor_point;

        static Vector2 cursor_center = new Vector2(7, 7);

        public static void Load(ContentManager content)
        {
            cursor_target = content.Load<Texture2D>("cursor_target");
            cursor_select = content.Load<Texture2D>("cursor_select");
            cursor_point = content.Load<Texture2D>("cursor_point");
        }
    }
}
