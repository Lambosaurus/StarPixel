﻿using System;
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



    public class Entity
    {
        public Vector2 pos;
        public Vector2 velocity;
        public float angle;

        public bool destroyed;

        public event EventHandler Clicked;

        public Entity()
        {
            pos = new Vector2(0, 0);
            velocity = new Vector2(0, 0);
            angle = 0.0f;

            destroyed = false;
        }

        public void OnClick()
        {
            angle+=1.0f;
            //Clicked(this, EventArgs.Empty);

        }


        public virtual void Update()
        {
            pos += velocity;
        }

        public virtual void Draw(Camera camera)
        {
        }

        public void Destory(Camera camera)
        {
            destroyed = true;
        }
    }

    public class Physical : Entity
    {
        public float mass;
        public float inertia;
        public float angular_velocity;

        public Physical() : base()
        {
            mass = 10;
            inertia = 50; // things feel right with the current thruster torque model when inertia is about 5x mass.

            angular_velocity = 0.0f;
        }

        public override void Update()
        {
            angle += angular_velocity;
            angle = Utility.WrapAngle(angle);

            base.Update();
        }
        
        public void Push( Vector2 force, float torque )
        {
            velocity += force / mass;
            angular_velocity += torque / inertia;
        }
    }
}
