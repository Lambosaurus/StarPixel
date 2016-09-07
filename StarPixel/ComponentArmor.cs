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
        public float electro_block = 0.2f;


        public float segment_integrity = 60f;

        public Resistance armor_resistance = Resistance.Zero;
        
        public HullArmor New( Ship ship )
        {
            HullArmor armor = new HullArmor(ship, ship.template.component_armor_size, this);
            

            return armor;
        }
    }

    public class HullArmor
    {
        public static Resistance ARMOR_BASE_RESISTANCE = new Resistance(0.15f, 0, 0, 0.7f);

        ArmorTemplate template;

        public int segment_count { get; protected set; }

        public float[] integrity { get; protected set; }
        public float max_integrity { get; protected set; }

        public float start_angle { get; protected set; }
        public float per_segment_angle { get; protected set; }

        public Resistance armor_resistance { get; protected set; }

        public Ship ship { get; protected set; }
        public float size { get; protected set; }

        float elipticity;

        public HullArmor( Ship arg_ship, float arg_size, ArmorTemplate arg_template)
        {
            ship = arg_ship;

            elipticity = ship.hitbox.size.X / ship.hitbox.size.Y;

            template = arg_template;
            size = arg_size;

            segment_count = ship.template.armor_segment_count;

            max_integrity = template.segment_integrity * size;
            integrity = new float[segment_count];

            for (int i = 0; i < segment_count; i++)
            {
                integrity[i] = max_integrity;
            }

            per_segment_angle = MathHelper.TwoPi / segment_count;
            start_angle = (ship.template.armor_seam_on_rear == (segment_count%2 == 0)) ? 0.0f : -per_segment_angle / 2;

            armor_resistance = template.armor_resistance * ARMOR_BASE_RESISTANCE;
            
        }
        
        public int GetSegment(float incoming_angle)
        {
            float aoa = Utility.WrapAngle(incoming_angle - start_angle - ship.angle);
            int segment = (int)((aoa) / per_segment_angle);
            if (segment >= segment_count) { return 0; } // you'd think this would never happen, but WrapAngle can return +2pi due to rounding errors.
            return segment;
        }

        public Damage BlockDamage(Damage dmg, Vector2 arg_pos)
        {
            int segment = this.GetSegment(Utility.Angle(arg_pos - ship.pos));

            if (integrity[segment] > 0)
            {
                float dmg_dealt = armor_resistance.EvaluateDamage(dmg);
                
                if (integrity[segment] < dmg_dealt)
                {
                    float segment_hp = integrity[segment];
                    integrity[segment] = 0.0f;
                    return armor_resistance.RemainingDamage(segment_hp, dmg);
                }
                else
                {
                    integrity[segment] -= dmg_dealt;
                    return null;
                }
            }

            return dmg;

        }
        
    }
}



















