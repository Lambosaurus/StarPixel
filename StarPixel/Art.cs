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
    public static class ArtManager
    {

        public static Dictionary<string, ArtSpriteResource> sprites = new Dictionary<string, ArtSpriteResource>();
        static ArtSpriteResource sprite_default;

        public static void Load(ContentManager content)
        {

            sprite_default = new ArtSpriteResource("default_sprite");
            sprite_default.Load(content);

            foreach ( ArtSpriteResource sprite in sprites.Values)
            {
                sprite.Load(content);
            }
        }

        public static ArtSprite NewArtSprite(string key)
        {
            if ( sprites.ContainsKey(key) )
            {
                return sprites[key].New();
            }

            return sprite_default.New();
        }
    }


    class Particle
    {

    }

    public class ArtParticleResource
    {
        public string name;
        
        
    }


    public class ArtSpriteResource
    {
        public Texture2D sprite;
        public string name;

        public Vector2 size;
        public Vector2 center;

        public float radius;

        public float scale;

        public ArtSpriteResource(string arg_name, float arg_scale = 1.0f)
        {
            name = arg_name;
            scale = arg_scale; // TODO i dont even want the scale parameter.
        }
        

        public void Load( ContentManager content )
        {
            sprite = content.Load<Texture2D>(name);
            size = new Vector2(sprite.Bounds.Width, sprite.Bounds.Height);
            center = size / 2;
        }

        public ArtSprite New(  )
        {
            return new ArtSprite(this);
        }
    }

    public class ArtSprite
    {
        ArtSpriteResource resource;

        public Vector2 pos;
        public float angle;

        public ArtSprite( ArtSpriteResource arg_resource )
        {
            resource = arg_resource;
        }

        public bool InView( Camera camera )
        {
            Vector2 onscreen = camera.Map(pos);
            float cull_radius = resource.radius * camera.scale;

            return onscreen.X + cull_radius > 0 &&
                   onscreen.Y + cull_radius > 0 &&
                   onscreen.X - cull_radius < camera.res.X &&
                   onscreen.Y - cull_radius < camera.res.Y;
        }

        public void Draw( Camera camera )
        {
            if (!InView(camera)) { return; }

            camera.batch.Draw(resource.sprite, camera.Map(pos), null, Color.White, angle, resource.center, camera.scale * resource.scale, SpriteEffects.None, 0);
            
        }
    }
}





