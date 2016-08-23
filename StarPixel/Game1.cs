using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;



namespace StarPixel
{
    
    /// This is the main type for your game
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // meh. Its a semi comfortable res for developing in.
        
        int window_res_x = (GameConst.screensaver) ? 1920 : 1200;
        int window_res_y = (GameConst.screensaver) ? 1080 : 800;
        bool fullscreen = GameConst.screensaver;
        
        
        UI ui;
        Universe universe;

        double time_accelleration = 1.0;
        double time_update_counter = 0.0;
        bool paused = false;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = window_res_y;
            graphics.PreferredBackBufferWidth = window_res_x;

            if(fullscreen != graphics.IsFullScreen) { graphics.ToggleFullScreen(); }

            Content.RootDirectory = "Content";
        }

        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        protected override void Initialize()
        {
            base.Initialize();
            
            universe = new Universe();

            ui = new UI(GraphicsDevice, spriteBatch, window_res_x, window_res_y);
            ui.Start();
            ui.FocusUniverse(universe);

            universe.Start();

            
            if (universe.physicals.Count != 0)
            {
                if ((Ship)universe.physicals[0] is Ship)
                {
                    ui.FocusPhys((Ship)universe.physicals[0]);
                    //ui.GiveHumanAIHandle((IntellegenceHuman)(((Ship)universe.physicals[0]).ai));
                }
            }
            

        }


        /// LoadContent will be called once per game
        protected override void LoadContent()
        {
            AssetShipTemplates.GenerateAssets();
            AssetThrusterTemplates.GenerateAssets();
            AssetWeaponTemplates.GenerateAssets();
            AssetShieldTemplates.GenerateAssets();
            AssetArmorTemplates.GenerateAssets();

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            

            
            ColorManager.Load(Content); // it may be important to do this before artmanager.Load, in case I make art assets which need colors
            
            ArtManager.Load(Content);
            UI.Load(Content);
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

            if (GameConst.screensaver)
            {
                if (ui.inputs.pos != ui.inputs.old_pos)
                {
                    this.Exit();
                }
            }


            /*
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


            // Oh man is this ugly.
            foreach (Physical phys in universe.physicals)
            {
                if (phys is Ship)
                {
                    Ship ship = (Ship)phys;
                    if (ship.ai is IntellegenceHuman)
                    {
                        Vector2 target = camera.InverseMap(mouse_pos * camera.upsample_multiplier);
                        ((IntellegenceHuman)ship.ai).GiveTarget(target);
                    }
                }
            }

            
            // Fix for Joels machine.
            // For some reason this may trigger on the first frame
            if( !first_frame && mouse_middle == ButtonState.Pressed && Mouse.GetState().MiddleButton == ButtonState.Released)
            {
                selectedEntity = universe.OnClick(camera.InverseMap(new Vector2(Mouse.GetState().X* camera.upsample_multiplier, Mouse.GetState().Y* camera.upsample_multiplier)));

            }
            mouse_middle = Mouse.GetState().MiddleButton;
            */

            // this could be a little neater.
            time_update_counter += time_accelleration;
            while (time_update_counter > 1.0)
            {
                time_update_counter--;
                universe.Update();
            }


            ui.Update();

            base.Update(gameTime);
        }

        
        /// This is called when the game should draw itself.
        protected override void Draw(GameTime gameTime)
        {
            ui.Draw();

            //spriteBatch.Draw(cursor_fire, mouse_pos - new Vector2(7,7), Color.White);

            base.Draw(gameTime);
        }
    }
}
