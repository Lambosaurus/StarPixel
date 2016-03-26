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
            // Fighter
            // Gunship
            // Cargo
            // Destroyer
            // Battlecruiser
            // Heavy
            // Light
  
            // Ie, HCG is a heavy cargo gunship


            float forward = 0.0f; // STFU WARNINGS, IT WILL GET USED. IT WILL.
            float right = MathHelper.PiOver2;
            float reverse = MathHelper.Pi;
            float left = -MathHelper.PiOver2;

            float arc_minimum = 0.05f;
            float arc_180 = MathHelper.Pi;



            // -- // F2 START

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

            template.AddThrusterPort(new Vector2(-15, 0), reverse,  0.8f , 1, 0, 0);
            template.AddThrusterPort(new Vector2(12, 4), right ,     0.4f, 0, -1, -1);
            template.AddThrusterPort(new Vector2(-8, 11), right,     0.4f, 0, -1, 1);
            template.AddThrusterPort(new Vector2(12, -4), left,      0.4f, 0, 1, 1);
            template.AddThrusterPort(new Vector2(-8, -11), left,     0.4f, 0, 1, -1);
            template.AddThrusterPort(new Vector2(0, 9), 0.2f,     0.4f, -1, 0, 0);
            template.AddThrusterPort(new Vector2(0, -9), -0.2f,   0.4f, -1, 0, 0);

            template.AddWeaponPort(new Vector2(10, 2), 1.0f, 0.0f, arc_180);
            template.AddWeaponPort(new Vector2(10, -2), 1.0f, 0.0f, arc_180);

            ship_templates.Add("F2", template);

            ArtManager.sprites.Add("F2 paint", new ArtSpriteResource("F2 paint", 0.5f));
            ArtManager.sprites.Add("F2 heat", new ArtSpriteResource("F2 heat", 0.5f));
            ArtManager.sprites.Add("F2", new ArtSpriteResource("F2", 0.5f));

            // -- // F2 END

            // -- // CG1 START

            template = new ShipTemplate();
            template.base_mass = 500;
            template.base_intertia = 6000;
            template.hull_art_resource = "CG1";
            //template.heat_art_resource = "CG1 heat";
            template.paint_art_resource = "CG1 paint";
            template.component_thruster_size = 2.25f;

            template.shield_radius = 60f;
            template.component_shield_size = 2.25f;

            template.component_armor_size = 2.25f;
            template.armor_segment_count = 5;

            template.hitbox = new HitboxPolygon( new Vector2[] {
                new Vector2(40,0),

                new Vector2(48,-8),
                new Vector2(48,-13),
                new Vector2(42,-22),
                new Vector2(26,-22),
                new Vector2(20,-18),
                new Vector2(-10,-21),
                new Vector2(-14,-27),
                new Vector2(-33,-27),
                new Vector2(-30,-21),
                new Vector2(-37,-21),
                new Vector2(-46,-8),

                new Vector2(-46,8),
                new Vector2(-37,21),
                new Vector2(-30,21),
                new Vector2(-33,27),
                new Vector2(-14,27),
                new Vector2(-10,21),
                new Vector2(20,18),
                new Vector2(26,22),
                new Vector2(42,22),
                new Vector2(48,13),
                new Vector2(48,8),
            } );

            template.AddThrusterPort(new Vector2(-46, 0), reverse,  1.1f , 1, 0, 0);
            template.AddThrusterPort(new Vector2(36, 22), right,     0.4f, 0, -1, -1);
            template.AddThrusterPort(new Vector2(30, 22), right,     0.4f, 0, -1, -1);
            template.AddThrusterPort(new Vector2(-26, 24), right,     0.6f, 0, -1, 1);
            template.AddThrusterPort(new Vector2(36, -22), left,      0.4f, 0, 1, 1);
            template.AddThrusterPort(new Vector2(30, -22), left,     0.4f, 0, 1, 1);
            template.AddThrusterPort(new Vector2(-26, -24), left,    0.6f, 0, 1, -1);
            template.AddThrusterPort(new Vector2(-12, 24), forward,     0.6f, -1, 0, 0);
            template.AddThrusterPort(new Vector2(-12, -24), forward,   0.6f, -1, 0, 0);
            
            ship_templates["CG1"] = template;

            ArtManager.sprites.Add("CG1 paint", new ArtSpriteResource("CG1 paint", 0.5f));
            ArtManager.sprites.Add("CG1", new ArtSpriteResource("CG1", 0.5f));

            // -- // CG1 END
        }
    }
}