using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Tales_of_Everlight.Characters;

namespace Tales_of_Everlight;

public class Level1
{

    private Dictionary<Vector2, int> foreground;
    private Dictionary<Vector2, int> collisions;
    private Dictionary<Vector2, int> spikes;
    public readonly int Width = 271*64;
        
    

    public Dictionary<Vector2, int> Foreground => foreground;
    public Dictionary<Vector2, int> Collisions => collisions;
    public Dictionary<Vector2, int> Spikes => spikes;
    
    
    private Texture2D textureAtlas_foreground;
    private Texture2D textureAtlas_collisions;
    private Texture2D textureAtlas_spikes;

    public Texture2D BackgroundHigh { get; set; }
    public Texture2D BackgroundLow { get; set; }
    
    
    public Level1()
    {
        foreground = new Dictionary<Vector2, int>();
        collisions = new Dictionary<Vector2, int>();
    }
    
    
    
    
    public void Initialize(ContentManager content)
    {
        foreground = LoadMap("Content/map/level1_foreground.csv");
        collisions = LoadMap("Content/map/level1_collisions.csv");
        spikes = LoadMap("Content/map/level1_spikes.csv");

        textureAtlas_foreground = content.Load<Texture2D>("tileset_ground");
        textureAtlas_collisions = content.Load<Texture2D>("tileset_collisions");
        textureAtlas_spikes = content.Load<Texture2D>("tileset_spikes");
        
        
        BackgroundHigh = content.Load<Texture2D>("Level1_background_high");
        BackgroundLow = content.Load<Texture2D>("Level1_background_low");
        
    }


    public void Draw(SpriteBatch spriteBatch)
    {

        float parallaxFactor = 0.03f; // Very subtle horizontal movement
        float offsetX = Main.MainHero.Rect.X * parallaxFactor;
    
        // Calculate blend factor based on player's Y position
        float transitionHeight = 40 * 64; // 34 * Tilesize
        float blendRange = 10 * 64; // Transition range (10 tiles)
        float blendFactor = 0f;
    
        // Calculate how far the player is into the transition zone
        if (Main.MainHero.Rect.Y > transitionHeight - blendRange && Main.MainHero.Rect.Y < transitionHeight + blendRange)
        {
            blendFactor = (Main.MainHero.Rect.Y - (transitionHeight - blendRange)) / (2 * blendRange);
            blendFactor = MathHelper.Clamp(blendFactor, 0f, 1f);
        }
        else if (Main.MainHero.Rect.Y >= transitionHeight + blendRange)
        {
            blendFactor = 1f; // Fully transitioned to BackgroundLow
        }
    
        for (int i = 0; i < Width; i += BackgroundHigh.Width)
        {
            // Position for background
            Vector2 position = new Vector2(
                i - offsetX % BackgroundHigh.Width,
                Main.MainHero.Rect.Y - BackgroundHigh.Height/2
            );
        
            // Draw BackgroundHigh with decreasing opacity
            spriteBatch.Draw(
                BackgroundHigh, 
                position, 
                Color.White * (1f - blendFactor)
            );
        
            // Draw BackgroundLow with increasing opacity
            spriteBatch.Draw(
                BackgroundLow, 
                position, 
                Color.White * blendFactor
            );
        }


        foreach (var item in foreground)
        {
            int display_tilesize = 64;
            int num_tiles_per_row = 15;
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

        foreach (var item in spikes)
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