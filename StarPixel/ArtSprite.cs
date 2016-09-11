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
    public class SpriteTileSheet
    {
        public Texture2D sheet { get; private set; }
        public string name { get; private set; }

        public int tile_width { get; private set; }
        public int tile_height { get; private set; }

        public int cols { get; private set; }
        public int rows { get; private set; }

        Vector2 tile_center;

        // leave x and y counts to 0 and it will figure out the maximum x and y that fits on the sheet.
        public SpriteTileSheet( string arg_name, int arg_tile_width, int arg_tile_height, int arg_cols = -1, int arg_rows = -1 )
        {
            name = arg_name;
            tile_width = arg_tile_width;
            tile_height = arg_tile_height;
            cols = arg_cols;
            rows = arg_rows;

            tile_center = new Vector2( tile_width, tile_height ) / 2.0f;
        }

        public void Load(ContentManager content)
        {
            sheet = content.Load<Texture2D>(name);
            
            if (cols == -1) { cols = (int)(sheet.Width / tile_width); }
            if (rows == -1) { rows = (int)(sheet.Height / tile_height); }
        }

        public void Draw( SpriteBatch batch, int item, Vector2 position, float scale, Color color, float rotation = 0.0f )
        {
            int row = item / cols;
            int col = item - (row * cols);

            Rectangle rect = new Rectangle(col * tile_width, row * tile_height, tile_width, tile_height);

            batch.Draw(sheet, position, rect, color, rotation, tile_center, scale, SpriteEffects.None, 0);
        }
    }

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
        public ArtSpriteResource resource { get; private set; }
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
            return (camera.scale * scale.Y * resource.radius > GameConst.minimum_draw_radius) && camera.ContainsCircle(pos, resource.radius);
        }

        public void Draw(Camera camera)
        {
            if (!InView(camera)) { return; }

            camera.batch.Draw(resource.sprite, camera.Map(pos), null, color, angle, resource.center, camera.scale*scale, SpriteEffects.None, 0);
        }

        public void Draw(Camera camera, Vector2 pos_override, float angle_override)
        {
            camera.batch.Draw(resource.sprite, camera.Map(pos_override), null, color, angle_override, resource.center, camera.scale * scale, SpriteEffects.None, 0);
        }
    }
}
