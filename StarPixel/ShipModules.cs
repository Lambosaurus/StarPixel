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
    class Thrusters
    {
        public float main_thrust;
        public float manouvering_thrust;

        public float max_power;
        public float input_power;

        public float health;
        public float max_health;


        Vector2 force;
        float torque;


        public Thrusters()
        {
        }

        public void Update()
        {

        }
    }
}
