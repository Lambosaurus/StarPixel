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
    public class Ship : Physical
    {

        ArtSprite hull_sprite;

        public Ship() : base()
        {
            mass = 50;
            inertia = 500;

            angular_velocity = 0.1f;

            hull_sprite = ArtManager.NewArtSprite("ship");
        }

        public override void Update()
        {
            base.Update();

            hull_sprite.pos = pos;
            hull_sprite.angle = angle;
        }

        public override void Draw(Camera camera)
        {
            hull_sprite.Draw(camera);
        }
    }
}
