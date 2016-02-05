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

        public Intellegence ai;


        public Ship() : base()
        {
            mass = 50;
            inertia = 500;

            hull_sprite = ArtManager.NewArtSprite("ship");

            ai = new IntellegenceHuman();
        }

        public override void Update()
        {
            base.Update();

            if (ai != null)
            {
                IntOutputs orders = ai.Process(new IntInputs());
            }

            hull_sprite.pos = pos;
            hull_sprite.angle = angle;
        }

        public override void Draw(Camera camera)
        {
            hull_sprite.Draw(camera);
        }
    }
}
