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
            template.base_mass = 50;
            template.base_intertia = 600;
            template.hull_art_resource = "F2";
            template.heat_art_resource = "F2 heat";
            template.paint_art_resource = "F2 paint";
            template.component_thruster_size = 1.0f;

            template.shield_radius = 30f;
            template.component_shield_size = 1.0f;

            template.hitbox = new HitboxPolygon( new Vector2[] {
                new Vector2(18,-3),
                new Vector2(-12, -14),
                new Vector2(-18, -8),
                new Vector2(-18, 8),
                new Vector2(-12, 14),
                new Vector2(18, 3),
            } );

            template.AddThrusterPort(new Vector2(-15, 0), reverse,  0.6f , 1, 0, 0);
            template.AddThrusterPort(new Vector2(12, 4), right ,     0.4f, 0, -0.5f, -0.5f);
            template.AddThrusterPort(new Vector2(-8, 11), right,     0.4f, 0, -0.5f, 0.5f);
            template.AddThrusterPort(new Vector2(12, -4), left,      0.4f, 0, 0.5f, 0.5f);
            template.AddThrusterPort(new Vector2(-8, -11), left,     0.4f, 0, 0.5f, -0.5f);
            template.AddThrusterPort(new Vector2(0, 9), 0.2f,     0.4f, -0.5f, 0, 0);
            template.AddThrusterPort(new Vector2(0, -9), -0.2f,   0.4f, -0.5f, 0, 0);

            template.AddWeaponPort(new Vector2(10, 2), 1.0f, 0.0f, 0.05f);
            template.AddWeaponPort(new Vector2(10, -2), 1.0f, 0.0f, 0.05f);

            ship_templates.Add("F2", template);

            ArtManager.sprites.Add("F2 paint", new ArtSpriteResource("F2 paint", 0.5f));
            ArtManager.sprites.Add("F2 heat", new ArtSpriteResource("F2 heat", 0.5f));
            ArtManager.sprites.Add("F2", new ArtSpriteResource("F2", 0.5f));

        }
    }
}