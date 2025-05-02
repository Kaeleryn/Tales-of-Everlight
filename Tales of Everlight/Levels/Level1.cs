using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Tales_of_Everlight;

public class Level1
{

    private Dictionary<Vector2, int> foreground;
    private Dictionary<Vector2, int> collisions;
    public readonly int Width = 10624;
        
    

    public Dictionary<Vector2, int> Foreground => foreground;
    public Dictionary<Vector2, int> Collisions => collisions;
    
    
    private Texture2D textureAtlas_foreground;
    private Texture2D textureAtlas_collisions;

    public Texture2D Background { get; set; }
    
    
    
    
    
    
    
    public Level1()
    {
        foreground = new Dictionary<Vector2, int>();
        collisions = new Dictionary<Vector2, int>();
    }
    
    
    
    
    public void Initialize(ContentManager content)
    {
        foreground = LoadMap("Content/map/level1_foreground.csv");
        collisions = LoadMap("Content/map/level1_collisions.csv");

        textureAtlas_foreground = content.Load<Texture2D>("tileset_ground");
        textureAtlas_collisions = content.Load<Texture2D>("tileset_collisions");
        // Background1L = content.Load<Texture2D>("background_layer_1");
        // Background2L = content.Load<Texture2D>("background_layer_2");
        // Background3L = content.Load<Texture2D>("background_layer_3");
        
        Background = content.Load<Texture2D>("background_sky");
        
    }
    

    public void Draw(SpriteBatch spriteBatch)
    {
        // spriteBatch.Draw(Background1L, new Vector2(0, 0), Color.White);
        // spriteBatch.Draw(Background2L, new Vector2(0, 0), Color.White);
        // spriteBatch.Draw(Background3L, new Vector2(0, 0), Color.White); 992
         // spriteBatch.Draw(Background, new Vector2(0, -260), Color.White);
        for(int i = 0; i < 10880; i+=992)
        {
            spriteBatch.Draw(Background, new Vector2(i, -560), Color.White);
        }
        
        
        foreach (var item in foreground)
        {
            int display_tilesize = 64;
            int num_tiles_per_row = 16;
            int pixel_tilesize = 64;

            Rectangle drect = new(
                (int)item.Key.X * display_tilesize,
                (int)item.Key.Y * display_tilesize,
                display_tilesize,
                display_tilesize
            );


            int x = item.Value % num_tiles_per_row;
            int y = item.Value / num_tiles_per_row;

            Rectangle src = new(
                x * pixel_tilesize,
                y * pixel_tilesize,
                pixel_tilesize,
                pixel_tilesize
            );

            spriteBatch.Draw(textureAtlas_foreground, drect, src, Color.White);
        }

        foreach (var item in collisions)
        {
            int display_tilesize = 64;
            int num_tiles_per_row = 3;
            int pixel_tilesize = 64;

            Rectangle drect = new(
                (int)item.Key.X * display_tilesize,
                (int)item.Key.Y * display_tilesize,
                display_tilesize,
                display_tilesize
            );


            int x = item.Value % num_tiles_per_row;
            int y = item.Value / num_tiles_per_row;

            Rectangle src = new(
                x * pixel_tilesize,
                y * pixel_tilesize,
                pixel_tilesize,
                pixel_tilesize
            );

            // _spriteBatch.Draw(textureAtlas_collisions, drect, src, Color.White);
        }
        
    }
    
    
    
    
    public Dictionary<Vector2, int> LoadMap(string filepath)
    {
        Dictionary<Vector2, int> result = new();

        StreamReader reader = new(filepath);
        int y = 0;
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            string[] items = line.Split(',');

            for (int x = 0; x < items.Length; x++)
            {
                if (int.TryParse(items[x], out int value))
                {
                    if (value > -1)
                    {
                        result[new Vector2(x, y)] = value;
                    }
                }
            }

            y++;
        }

        return result;
    } // яка  зчитує карту з файлу
}