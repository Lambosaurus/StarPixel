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
    public class ArmorTemplate
    {
        public string art_resource;


        public ComponentArmor New( Ship ship )
        {
            ComponentArmor shield = new ComponentArmor(ship, ship.template.component_armor_size, this);

            return shield;
        }
    }

    public class ComponentArmor : Component
    {
        ArmorTemplate template;

        public float segments;
        

        public ComponentArmor( Ship arg_ship, float arg_size, ArmorTemplate arg_template): base(arg_ship, arg_size)
        {
            template = arg_template;

            
        }
        
        public override void Update()
        {

        }
    }
}
