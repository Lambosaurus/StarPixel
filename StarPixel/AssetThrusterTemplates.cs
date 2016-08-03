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
    static class AssetThrusterTemplates
    {
        public static Dictionary<string, ThrusterTemplate> thruster_templates = new Dictionary<string, ThrusterTemplate>();

        public static void GenerateAssets()
        {
            ThrusterTemplate template = new ThrusterTemplate();

            template.main_thrust = 1.0f;
            template.reverse_thrust = 0.3f;
            template.side_thrust = 0.3f;
            template.torque = 0.3f;
            template.particle_effects = "default_thruster";
            template.sparkle_effects = "default_sparkles";

            thruster_templates.Add("default", template);


            ArtVentResource vent = new ArtVentResource("particle");
            vent.temperature = 2000;
            vent.size_start = new Vector2(1.0f, 1.0f);
            vent.size_end = new Vector2(4.0f, 1.0f);
            vent.particle_life = 5f;
            vent.temp_halflife = 3f;
            vent.velocity_scatter = 0.1f;
            vent.velocity_ejection = 1f;
            vent.generation_frequency = 0.8f;
            vent.particle_count = 20;
            ArtManager.vents.Add("default_sparkles", vent);

            vent = new ArtVentResource("particle");
            vent.temperature = 2000;
            vent.size_start = new Vector2(1.0f, 1.0f);
            vent.size_end = new Vector2(4.0f, 1.0f);
            vent.particle_life = 0.4f;
            vent.temp_halflife = 0.25f;
            vent.velocity_scatter = 0.15f;
            vent.velocity_ejection = 1f;
            ArtManager.vents.Add("default_thruster", vent);





            template = new ThrusterTemplate();

            template.main_thrust = 1.2f;
            template.reverse_thrust = 0.4f;
            template.side_thrust = 0.4f;
            template.torque = 0.4f;
            template.particle_effects = "better_thruster";
            template.sparkle_effects = "default_sparkles";

            thruster_templates.Add("better", template);


            vent = new ArtVentResource("particle");
            vent.temperature = 5500;
            vent.size_start = new Vector2(1.0f, 1.0f);
            vent.size_end = new Vector2(4.0f, 1.0f);
            vent.particle_life = 0.55f;
            vent.temp_halflife = 0.3f;
            vent.velocity_scatter = 0.2f;
            vent.velocity_ejection = 1f;
            ArtManager.vents.Add("better_thruster", vent);



            template = new ThrusterTemplate();

            template.main_thrust = 0.6f;
            template.reverse_thrust = 0.15f;
            template.side_thrust = 0.15f;
            template.torque = 0.15f;
            template.particle_effects = "worse_thruster";
            template.sparkle_effects = "default_sparkles";

            thruster_templates.Add("worse", template);

            vent = new ArtVentResource("particle");
            vent.temperature = 1500;
            vent.size_start = new Vector2(1.0f, 1.0f);
            vent.size_end = new Vector2(4.0f, 1.0f);
            vent.particle_life = 0.45f;
            vent.temp_halflife = 0.2f;
            vent.velocity_scatter = 0.5f;
            vent.velocity_ejection = 1f;
            ArtManager.vents.Add("worse_thruster", vent);

        }
    }
}