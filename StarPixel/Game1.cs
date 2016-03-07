using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace StarPixel
{

    /// This is the main type for your game
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        int UPSAMPLE_MULTIPLIER = 2; // may be 1 or 2. x3 will violate the 4096x4096 texture limit

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // meh. Its a semi comfortable res for developing in.
        int window_res_x = 1200;
        int window_res_y = 800;

        Camera camera;
        StatusCamera status_camera;
        int status_camera_width = 100;

        int scrollVal = 0;
        Vector2 mouse_pos = new Vector2(0, 0);
        Texture2D cursor;
        ButtonState mouse_left = Mouse.GetState().LeftButton;
        Universe universe;
        Entity selectedEntity = null;


        public double time_accelleration = 1.0;
        public double time_update_counter = 0.0;
        public bool paused = false;

        KeyboardState old_keys;

        public Game1()
        {
            UPSAMPLE_MULTIPLIER = 2; // ALRIGHTY, LETS DO THIS.

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = window_res_y;
            graphics.PreferredBackBufferWidth = window_res_x;

            Content.RootDirectory = "Content";
        }

        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        protected override void Initialize()
        {
            base.Initialize();

            old_keys = Keyboard.GetState();

            universe = new Universe();

            universe.Start();

            if (universe.physicals.Count != 0)
            {
                selectedEntity = universe.physicals[0];
            }

        }


        /// LoadContent will be called once per game
        protected override void LoadContent()
        {
            AssetShipTemplates.GenerateAssets();
            AssetThrusterTemplates.GenerateAssets();
            AssetWeaponTemplates.GenerateAssets();
            AssetShieldTemplates.GenerateAssets();

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);


            camera = new Camera(GraphicsDevice, spriteBatch, window_res_x, window_res_y, UPSAMPLE_MULTIPLIER);
            //camera.DRAW_HITBOXES = true;

            status_camera = new StatusCamera(GraphicsDevice, spriteBatch, 100);


            ColorManager.Load(Content); // it may be important to do this before artmanager.Load, in case I make art assets which need colors
            
            ArtManager.Load(Content);

            cursor = Content.Load<Texture2D>("cursor");
        }

        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        protected override void UnloadContent()
        {

            // probably not the spot for this. but where else?
            universe.End();
        }
        

        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                this.Exit();
            }


            KeyboardState new_keys = Keyboard.GetState();
            
            if ( new_keys.IsKeyDown(Keys.P) && !old_keys.IsKeyDown(Keys.P) )
            {
                paused = !paused;
                time_accelleration = (paused) ? 0.0 : 1.0;
            }

            if (new_keys.IsKeyDown(Keys.OemMinus) && !old_keys.IsKeyDown(Keys.OemMinus))
            {
                if (paused)
                {
                    paused = false;
                    time_accelleration = 1.0;
                }
                else
                {
                    time_accelleration /= 1.25;
                }
            }

            if (new_keys.IsKeyDown(Keys.OemPlus) && !old_keys.IsKeyDown(Keys.OemPlus))
            {
                if (paused)
                {
                    paused = false;
                    time_accelleration = 1.0;
                }
                else
                {
                    time_accelleration *= 1.25;
                }
            }



            //if scroll has been used, zoom in/out
            if (Mouse.GetState().ScrollWheelValue > scrollVal)
            {
                camera.scale *= 1.1f;
            }
            else if(Mouse.GetState().ScrollWheelValue < scrollVal)
            {
                camera.scale /= 1.1f;
            }
            scrollVal = Mouse.GetState().ScrollWheelValue;

            Vector2 new_pos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            //if right button is pressed, move camera
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                
                if (new_pos != mouse_pos)
                {
                    Vector2 delta = (mouse_pos - new_pos) / camera.scale;

                    camera.pos += delta* camera.upsample_multiplier;
                    
                }
            }
            mouse_pos = new_pos;

            if (mouse_left == ButtonState.Pressed && Mouse.GetState().LeftButton == ButtonState.Released)
            {
                selectedEntity = universe.OnClick(camera.InverseMap(new Vector2(Mouse.GetState().X* camera.upsample_multiplier, Mouse.GetState().Y* camera.upsample_multiplier)));
                
            }
            mouse_left = Mouse.GetState().LeftButton;


            time_update_counter += time_accelleration;
            while (time_update_counter > 1.0)
            {
                time_update_counter--;
                universe.Update();
            }

            if (selectedEntity != null)
            {
                camera.pos = selectedEntity.pos;
            }

            old_keys = new_keys;
            base.Update(gameTime);
        }


        /// This is called when the game should draw itself.
        protected override void Draw(GameTime gameTime)
        {
            camera.Draw(universe);

            if (selectedEntity is Ship)
            {
                status_camera.DrawTarget((Ship)selectedEntity);
            }


            GraphicsDevice.SetRenderTarget(null); // draw to windows now
            //GraphicsDevice.Clear(Color.CornflowerBlue); // so we can see what we forgot TODO: remove this.

            // now we write the cameras result to the screen
            spriteBatch.Begin();

            camera.Blit(spriteBatch, new Vector2(0, 0));
            status_camera.Blit(spriteBatch, camera.onscreen_res - status_camera.onscreen_res);
            spriteBatch.Draw(cursor, mouse_pos - new Vector2(7,7), Color.White);

            spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
