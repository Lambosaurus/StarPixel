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
    static class AssetArmorTemplates
    {
        public static Dictionary<string, ArmorTemplate> armor_templates = new Dictionary<string, ArmorTemplate>();

        public static void GenerateAssets()
        {
            ArmorTemplate template = new ArmorTemplate();

            template.std_segment_integrity = 100f;

            armor_templates.Add("default",template);
        }
    }
}