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
    }
    
    public class UI
    {
        public enum ControlMode { Control, Observe }

        List<Widget> widgets; // ordered by depth: First object is deepest

        GraphicsDevice device;
        SpriteBatch batch;

        public InputState inputs { get; private set; }
        
        WidgetCamera camera_widget;
        WidgetShipStatus status_widget;
        
        Universe focus_universe;
        Ship focus_ship;
        IntellegenceHuman focus_ai;

        ControlMode mode = ControlMode.Observe;

        Color cursor_color_control = Color.Red;
        Color cursor_color_observer = Color.Yellow;

        public UI( GraphicsDevice arg_device, SpriteBatch arg_batch, int width, int height )
        {
            widgets = new List<Widget>();

            batch = arg_batch;
            device = arg_device;

            camera_widget = new WidgetCamera( this, new Camera(arg_device, arg_batch, width, height, 2) ); // UPSAMPLE_MULTILIER
            status_widget = new WidgetShipStatus(arg_device, arg_batch, 100, 100);
            status_widget.pos.Y = height - status_widget.size.Y;
            
            widgets.Add(camera_widget);
            widgets.Add(status_widget);
            
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
        }

        public void FocusShip(Ship arg_ship)
        {
            focus_ship = arg_ship;
            camera_widget.focus_entity = arg_ship;
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

        public void MouseCallBack( Universe universe, Vector2 position )
        {
            if (mode == ControlMode.Control)
            {
                if (universe == focus_universe && focus_ai != null)
                {
                    focus_ai.weapon_target = position;
                    focus_ai.firing = (inputs.mb.LeftButton == ButtonState.Pressed);
                }
            }
            else
            {
            }
        }

        public void Update()
        {
            inputs.Update();
            
            camera_widget.markers = null;

            if (inputs.kb.IsKeyDown(Keys.LeftShift))
            {
                if (focus_ship != null && focus_ship.ai != null)
                {
                    List<UIMarker> markers = focus_ship.ai.GetUiMarkers();
                    if (markers != null)
                    {
                        camera_widget.markers = markers;
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


            if ( inputs.KeyDownEvent(Keys.OemTilde))
            {
                mode = (mode == ControlMode.Control) ? ControlMode.Observe : ControlMode.Control;
            }

            
            bool mouse_focus_given = false;

            foreach (Widget widget in widgets)
            {
                bool give_focus = false;
                if ( !mouse_focus_given &&  Utility.PointInWindow(inputs.pos - widget.pos, widget.size) )
                {
                    give_focus = true;
                    mouse_focus_given = true;
                }
                widget.Update(inputs, give_focus);
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

            batch.Draw(cursor_target, inputs.pos - cursor_center, (mode == ControlMode.Control) ? cursor_color_control : cursor_color_observer );

            batch.End();

        }




        // this is static because the loading is done before initilisation.
        static Texture2D cursor_target;
        static Texture2D cursor_select;
        static Vector2 cursor_center = new Vector2(7, 7);

        public static void Load(ContentManager content)
        {
            cursor_target = content.Load<Texture2D>("cursor_target");
            cursor_select = content.Load<Texture2D>("cursor_select");
        }
    }
}
