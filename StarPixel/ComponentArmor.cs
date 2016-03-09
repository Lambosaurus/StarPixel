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

        public float std_segment_integrity;

        public ComponentArmor New( Ship ship )
        {
            ComponentArmor armor = new ComponentArmor(ship, ship.template.component_armor_size, this);
            

            return armor;
        }
    }

    public class ComponentArmor : Component
    {
        ArmorTemplate template;

        public int segment_count;

        public float[] integrity;
        public float max_integrity;

        public float start_angle;
        public float per_segment_angle;

        public ComponentArmor( Ship arg_ship, float arg_size, ArmorTemplate arg_template): base(arg_ship, arg_size)
        {
            template = arg_template;

            segment_count = ship.template.armor_segment_count;

            max_integrity = template.std_segment_integrity * size;
            integrity = new float[segment_count];

            for (int i = 0; i < segment_count; i++)
            {
                integrity[i] = max_integrity;
            }

            per_segment_angle = MathHelper.TwoPi / segment_count;
            start_angle = (ship.template.armor_seam_on_rear == (segment_count%2 == 0)) ? 0.0f : -per_segment_angle / 2;

        }
        
        public int GetSegment(float incoming_angle)
        {
            float aoa = Utility.WrapAngle(incoming_angle - start_angle - ship.angle);
            return (int)((aoa) / per_segment_angle);
        }

        public Damage AdsorbDamage(Damage dmg, Vector2 arg_pos)
        {
            int segment = this.GetSegment(Utility.Angle(arg_pos - ship.pos));

            if (integrity[segment] > 0)
            {
                integrity[segment] -= 4;
            }
            else
            {
                return dmg;
            }

            return null;

        }

        public override void Update()
        {

        }
    }
}



















