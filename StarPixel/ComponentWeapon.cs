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

    public class WeaponPort
    {
        public Vector2 position;

        public float angle_min;
        public float angle_max;

        public int size;

        public WeaponPort(Vector2 arg_pos, int arg_size, float firing_arc, float direction)
        {
            position = arg_pos;
            size = arg_size;

            angle_min = direction - firing_arc / 2;
            angle_max = direction + firing_arc / 2;
        }

        public WeaponPort( WeaponPort example )
        {
            size = example.size;
            position = example.position;
            angle_min = example.angle_min;
            angle_max = example.angle_max;
        }
    }



    public class WeaponTemplate
    {

    }



    public class ComponentWeapon : Component
    {

        public ComponentWeapon(Ship arg_ship) : base (arg_ship)
        {

        }
    }
}
