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
        List<Widget> widgets; // ordered by depth: First object is deepest

        GraphicsDevice device;
        SpriteBatch batch;

        public InputState inputs { get; private set; }
        
        WidgetCamera camera_widget;
        WidgetShipStatus status_widget;
        
        Universe focus_universe;
        Ship focus_ship;
        IntellegenceHuman focus_ai;

        bool mode_control;

        Color cursor_color_control = Color.Red;
        Color cursor_color_observer = Color.Yellow;

        public UI( GraphicsDevice arg_device, SpriteBatch arg_batch, int width, int height )
        {
            widgets = new List<Widget>();

            batch = arg_batch;
            device = arg_device;

            camera_widget = new WidgetCamera( this, new Camera(arg_device, arg_batch, width, height, 2) );
            status_widget = new WidgetShipStatus(arg_device, arg_batch, 100, 100);
            status_widget.pos.Y = height - status_widget.size.Y;
            
            widgets.Add(camera_widget);
            widgets.Add(status_widget);

            mode_control = false;

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
            if (mode_control)
            {
                if (universe == focus_universe)
                {
                    focus_ai.weapon_target = position;
                }

                focus_ai.firing = (inputs.mb.LeftButton == ButtonState.Pressed);
            }
        }

        public void Update()
        {
            inputs.Update();
            GiveInputsToAi();

            camera_widget.markers = null;
            if (focus_ship != null && focus_ship.ai != null)
            {
                List<UIMarker> markers = focus_ship.ai.GetUiMarkers();
                if (markers != null)
                {
                    camera_widget.markers = markers;
                }
            }

            if ( inputs.KeyDownEvent(Keys.OemTilde))
            {
                mode_control = !mode_control;
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

            if (mode_control)
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

            batch.Draw(cursor_target, inputs.pos - cursor_center, (mode_control) ? cursor_color_control : cursor_color_observer );

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
