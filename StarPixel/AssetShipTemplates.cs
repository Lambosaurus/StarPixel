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
    static class AssetShipTemplates
    {
        public static Dictionary<string, ShipTemplate> ship_templates = new Dictionary<string, ShipTemplate>();

        public static void GenerateAssets()
        {
            // naming convention is intended to be 

            // Scout
            // Gunship
            // Cargo
            // Destroyer
            // Battlecruiser
            // Heavy
            // Light
  
            // Ie, HCG is a heavy cargo gunship


            float forward = 0.0f;
            float right = MathHelper.PiOver2;
            float reverse = MathHelper.Pi;
            float left = -MathHelper.PiOver2;


            ShipTemplate template = new ShipTemplate();
            template.base_mass = 200;
            template.base_intertia = 800;
            template.hull_art_resource = "LG1";

            template.AddPort(new Vector2(-10, 0), reverse,  0.9f , 1, 0, 0);
            template.AddPort(new Vector2(8, 4), right ,     0.6f, 0, -0.5f, -0.5f);
            template.AddPort(new Vector2(-8, 7), right,     0.6f, 0, -0.5f, 0.5f);
            template.AddPort(new Vector2(8, -4), left,      0.6f, 0, 0.5f, 0.5f);
            template.AddPort(new Vector2(-8, -7), left,     0.6f, 0, 0.5f, -0.5f);
            template.AddPort(new Vector2(-6, 9), 0.2f,     0.6f, -0.5f, 0, 0);
            template.AddPort(new Vector2(-6, -9), -0.2f,   0.6f, -0.5f, 0, 0);

            ship_templates.Add("LG1", template);


            ArtManager.sprites.Add("LG1", new ArtSpriteResource("LG1", 0.5f));

        }
    }
}