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

    public class ArtSpriteResource
    {
        public Texture2D sprite;
        public string name;

        public Vector2 size;
        public Vector2 center;

        public Vector2 scale;

        public float radius;

        public ArtSpriteResource(string arg_name, float arg_scale = 1.0f)
        {
            name = arg_name;
            scale = new Vector2( arg_scale );
        }


        public void Load(ContentManager content)
        {
            sprite = content.Load<Texture2D>(name);
            size = new Vector2(sprite.Bounds.Width, sprite.Bounds.Height);
            center = size / 2;

            radius = center.Length();
        }

        public ArtSprite New()
        {
            return new ArtSprite(this);
        }
    }

    public class ArtSprite
    {
        ArtSpriteResource resource;
        public Color color = Color.White;
        private Vector2 pos;
        private float angle;

        public Vector2 scale;

        public ArtSprite(ArtSpriteResource arg_resource)
        {
            resource = arg_resource;
            scale = resource.scale;
        }

        public void Update( Vector2 arg_pos, float arg_angle )
        {
            pos = arg_pos;
            angle = arg_angle;
        }

        public bool InView(Camera camera)
        {
            Vector2 onscreen = camera.Map(pos);
            float cull_radius = resource.radius * camera.scale;

            return onscreen.X + cull_radius > 0 &&
                   onscreen.Y + cull_radius > 0 &&
                   onscreen.X - cull_radius < camera.res.X &&
                   onscreen.Y - cull_radius < camera.res.Y;
        }

        public void Draw(Camera camera)
        {
            if (!InView(camera)) { return; }

            camera.batch.Draw(resource.sprite, camera.Map(pos), null, color, angle, resource.center, camera.scale*scale, SpriteEffects.None, 0);

        }
    }
}
