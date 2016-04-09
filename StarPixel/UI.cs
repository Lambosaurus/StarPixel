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

        public InputState()
        {
            mb = Mouse.GetState();
            kb = Keyboard.GetState();
            pos = new Vector2(mb.X, mb.Y);
        }
    }
    
    public class UI
    {
        List<Widget> widgets; // ordered by depth: First object is deepest

        GraphicsDevice device;
        SpriteBatch batch;


        public InputState current_inputs;
        public InputState old_inputs;

        WidgetCamera camera_widget;


        Universe focus_universe;
        Ship focus_ship;

        public UI( GraphicsDevice arg_device, SpriteBatch arg_batch, int width, int height )
        {
            widgets = new List<Widget>();

            batch = arg_batch;
            device = arg_device;

            camera_widget = new WidgetCamera( new Camera(arg_device, arg_batch, width, height) );

            widgets.Add(camera_widget);
        }

        public void Start()
        {
            old_inputs = new InputState();
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

        public void Update()
        {
            current_inputs = new InputState();

            bool mouse_focus_given = false;

            foreach (Widget widget in widgets)
            {
                bool give_focus = false;
                if ( !mouse_focus_given &&  Utility.PointInWindow(current_inputs.pos - widget.pos, widget.size) )
                {
                    give_focus = true;
                    mouse_focus_given = true;
                }
                widget.Update(current_inputs, old_inputs, give_focus);
            }


            camera_widget.draw_stat_rings = (current_inputs.kb.IsKeyDown(Keys.Tab));
            


            old_inputs = current_inputs;
        }

        public void Draw()
        {
            foreach (Widget widget in widgets)
            {
                widget.PreDraw();
            }

            device.SetRenderTarget(null);
            batch.Begin();

            foreach (Widget widget in widgets)
            {
                widget.Draw(batch);
            }

            batch.Draw(cursor_target, current_inputs.pos - cursor_center, Color.Red);

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
