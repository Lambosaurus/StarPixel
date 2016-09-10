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
            float arc_90 = MathHelper.Pi / 2.0f;
            float arc_60 = MathHelper.Pi / 3.0f;
            float arc_30 = MathHelper.Pi / 6.0f;



            // -- // F2 START

            ShipTemplate template = new ShipTemplate();
            template.base_mass = 50;
            //template.base_intertia = 600;
            template.mass_avg_radius = 9; // length approx 18*2, so reccommended mass-radius is length/4
            template.thruster_avg_radius = 9;
            template.hull_art_resource = "Ship/F2/Hull";
            template.heat_art_resource = "Ship/F2/Heat";
            template.paint_art_resource = "Ship/F2/Paint";

            template.component_thruster_size = 1.0f;
            template.component_thruster_pos = new Vector2(-10, 0);


            template.shield_radius = 30f;
            template.component_shield_size = 1.0f;
            template.component_shield_pos = new Vector2(-2, -4);

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

            template.AddWeaponPort(new Vector2(10, 3), 1.0f, 0.0f, arc_180/4);
            template.AddWeaponPort(new Vector2(10, -3), 1.0f, 0.0f, arc_180/4);

            ship_templates.Add("F2", template);

            ArtManager.sprites.Add("Ship/F2/Paint", new ArtSpriteResource("Ship/F2/Paint", 0.5f));
            ArtManager.sprites.Add("Ship/F2/Heat", new ArtSpriteResource("Ship/F2/Heat", 0.5f));
            ArtManager.sprites.Add("Ship/F2/Hull", new ArtSpriteResource("Ship/F2/Hull", 0.5f));

            // -- // F2 END


            // -- // CG1 START

            template = new ShipTemplate();
            template.base_mass = 300;
            //template.base_intertia = 30000;
            template.mass_avg_radius = 28; // length approx (48+46), so reccommended mass-radius is length/3
            template.thruster_avg_radius = 28;
            template.hull_art_resource = "Ship/CG1/Hull";
            //template.heat_art_resource = "Ship/CG1/Heat";
            template.paint_art_resource = "Ship/CG1/Paint";

            template.component_thruster_size = GameConst.C(3);
            template.component_thruster_pos = new Vector2(-20, 0);

            template.shield_radius = 60f;
            template.component_shield_size = GameConst.C(3);
            template.component_shield_pos = new Vector2(4, 0);
            
            template.component_armor_size = GameConst.C(3);
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
                new Vector2(-32,-27),
                new Vector2(-46,-8),

                new Vector2(-46,8),
                new Vector2(-32,27),
                new Vector2(-14,27),
                new Vector2(-10,21),
                new Vector2(20,18),
                new Vector2(26,22),
                new Vector2(42,22),
                new Vector2(48,13),
                new Vector2(48,8),
            } );

            template.AddThrusterPort(new Vector2(-46, 0), reverse,  1.1f , 1, 0, 0);
            template.AddThrusterPort(new Vector2(36, 22), right,     0.5f, 0, -1, -1);
            template.AddThrusterPort(new Vector2(30, 22), right,     0.5f, 0, -1, -1);
            template.AddThrusterPort(new Vector2(-26, 24), right,     0.7f, 0, -1, 1);
            template.AddThrusterPort(new Vector2(36, -22), left,      0.5f, 0, 1, 1);
            template.AddThrusterPort(new Vector2(30, -22), left,     0.5f, 0, 1, 1);
            template.AddThrusterPort(new Vector2(-26, -24), left,    0.7f, 0, 1, -1);
            template.AddThrusterPort(new Vector2(-12, 24), forward,     0.7f, -1, 0, 0);
            template.AddThrusterPort(new Vector2(-12, -24), forward,   0.7f, -1, 0, 0);

            template.AddWeaponPort(new Vector2(42, 12), 1.0f, 0.0f , arc_90);
            template.AddWeaponPort(new Vector2(42, -12), 1.0f, 0.0f, arc_90);
            template.AddWeaponPort(new Vector2(32, 18), 1.0f, arc_60, arc_90);
            template.AddWeaponPort(new Vector2(32, -18), 1.0f, -arc_60, arc_90);
            template.AddWeaponPort(new Vector2(-20, 20), 1.0f, arc_90 + arc_30, arc_90);
            template.AddWeaponPort(new Vector2(-20, -20), 1.0f, -arc_90 - arc_30, arc_90);


            ship_templates["CG1"] = template;

            ArtManager.sprites.Add("Ship/CG1/Paint", new ArtSpriteResource("Ship/CG1/Paint", 0.5f));
            ArtManager.sprites.Add("Ship/CG1/Hull", new ArtSpriteResource("Ship/CG1/Hull", 0.5f));

            // -- // CG1 END
        }
    }
}