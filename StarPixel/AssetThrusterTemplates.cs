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
            template.reverse_thrust = 0.5f;
            template.side_thrust = 0.5f;
            template.torque = 0.25f;
            template.particle_effects = "default_thruster";
            template.sparkle_effects = "default_sparkles";

            thruster_templates.Add("default", template);


            ArtVentResource vent = new ArtVentResource("particle");
            vent.std_ejection_temperature = 2000;
            vent.std_particle_count = 30;
            vent.std_particle_length = 1.0f;
            vent.std_particle_width = 1.0f;
            vent.std_particle_stretch_length = 4f;
            vent.std_particle_life = 5f;
            vent.std_temperature_scatter = 0;
            vent.std_temp_halflife = 3f;
            vent.std_velocity_scatter = 0.1f;
            vent.std_ejection_velocity = 1f;
            ArtManager.vents.Add("default_sparkles", vent);

            vent = new ArtVentResource("particle");
            vent.std_ejection_temperature = 2000;
            vent.std_particle_count = 50;
            vent.std_particle_length = 1.0f;
            vent.std_particle_stretch_length = 4f;
            vent.std_particle_life = 0.6f;
            vent.std_particle_width = 0.75f;
            vent.std_temp_halflife = 0.33f;
            vent.std_velocity_scatter = 0.15f;
            vent.std_ejection_velocity = 1f;
            ArtManager.vents.Add("default_thruster", vent);





            template = new ThrusterTemplate();

            template.main_thrust = 1.2f;
            template.reverse_thrust = 0.7f;
            template.side_thrust = 0.7f;
            template.torque = 0.4f;
            template.particle_effects = "better_thruster";
            template.sparkle_effects = "default_sparkles";

            thruster_templates.Add("better", template);


            vent = new ArtVentResource("particle");
            vent.std_ejection_temperature = 5500;
            vent.std_particle_count = 50;
            vent.std_particle_stretch_length = 4f;
            vent.std_particle_stretch_width = 1f;
            vent.std_particle_life = 0.7f;
            vent.std_particle_width = 1.0f;
            vent.std_particle_length = 1.0f;
            vent.std_temp_halflife = 0.4f;
            vent.std_velocity_scatter = 0.2f;
            vent.std_ejection_velocity = 1f;
            ArtManager.vents.Add("better_thruster", vent);



            template = new ThrusterTemplate();

            template.main_thrust = 0.2f;
            template.reverse_thrust = 0.1f;
            template.side_thrust = 0.1f;
            template.torque = 0.05f;
            template.particle_effects = "worse_thruster";
            template.sparkle_effects = "default_sparkles";

            thruster_templates.Add("worse", template);

            vent = new ArtVentResource("particle");
            vent.std_ejection_temperature = 1500;
            vent.std_particle_count = 50;
            vent.std_particle_stretch_length = 4f;
            vent.std_particle_stretch_width = 1f;
            vent.std_particle_life = 0.6f;
            vent.std_particle_width = 1.0f;
            vent.std_particle_length = 1.0f;
            vent.std_temp_halflife = 0.3f;
            vent.std_velocity_scatter = 0.5f;
            vent.std_ejection_velocity = 1f;
            ArtManager.vents.Add("worse_thruster", vent);

        }
    }
}