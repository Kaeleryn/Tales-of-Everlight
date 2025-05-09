using System.Collections.Generic;
using System.IO;
using System.Windows.Forms.VisualStyles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Tales_of_Everlight.Characters;

namespace Tales_of_Everlight;

public class Level2
{
     private Dictionary<Vector2, int> foreground;
    private Dictionary<Vector2, int> collisions;
    private Dictionary<Vector2, int> spikes;
    public Dictionary<Vector2, int> enemies;
    public Dictionary<Vector2, int> buffs;
    public Dictionary<Vector2, int> decor;
    public Dictionary<Vector2, int> decor2;
    public Dictionary<Vector2, int> forest;
    
    
    public readonly int Width = 295 * 64;
    public int Tilesize = 64;
    
    public bool IsActive { get; set; } = false;

    public List<Enemy> Enemies { get; set; } = new List<Enemy>();
    public List<Buff> Buffs { get; set; } = new List<Buff>();


    public Dictionary<Vector2, int> Foreground => foreground;
    public Dictionary<Vector2, int> Collisions => collisions;
    public Dictionary<Vector2, int> Spikes => spikes;


    private Texture2D textureAtlas_foreground;
    private Texture2D textureAtlas_collisions;
    private Texture2D textureAtlas_spikes;
    private Texture2D textureAtlas_decor;
    private Texture2D textureAtlas_forest;
    private Texture2D background;

   // public Texture2D BackgroundHigh { get; set; }
   // public Texture2D BackgroundLow { get; set; }


    public Level2()
    {
        foreground = new Dictionary<Vector2, int>();
        collisions = new Dictionary<Vector2, int>();
        spikes = new Dictionary<Vector2, int>();
       // enemies = new Dictionary<Vector2, int>();
       // buffs = new Dictionary<Vector2, int>();
    }


    public void Initialize(ContentManager content)
    {
        foreground = LoadMap("Content/map/level2_cave.csv");
        collisions = LoadMap("Content/map/level2_collision.csv");
        spikes = LoadMap("Content/map/level2_spikes.csv");
        decor = LoadMap("Content/map/level2_decoration.csv");
        decor2 = LoadMap("Content/map/level2_decoration2.csv");
        forest = LoadMap("Content/map/level2_forest.csv");
       enemies = LoadMap("Content/map/level2_enemy.csv");
        buffs = LoadMap("Content/map/level2_buffs.csv");


        textureAtlas_foreground = content.Load<Texture2D>("level2_foreground");
        textureAtlas_collisions = content.Load<Texture2D>("tileset_collisions");
        textureAtlas_spikes = content.Load<Texture2D>("tileset_spikes");
        textureAtlas_decor = content.Load<Texture2D>("level2_decor");
        textureAtlas_forest = content.Load<Texture2D>("tileset_ground");
        
        background = content.Load<Texture2D>("level2_background");
        


       // BackgroundHigh = content.Load<Texture2D>("Level1_background_high");
       // BackgroundLow = content.Load<Texture2D>("Level1_background_low");
    }

    public void SpawnEnemies(ContentManager content)
    {
        Enemies.Clear(); 
        
       

        foreach (var item in enemies)
        {
            if (item.Value != -1)
            {
                switch (item.Value)
                {
                    case 3:


                        Goblin goblin = new Goblin(
                            content,
                            new Rectangle((int)item.Key.X * Tilesize, (int)item.Key.Y * Tilesize, 64, 64),
                            new Rectangle(0, 0, 64, 64)
                        );
                        Enemies.Add(goblin);
                        break;

                    case 1:
                        

                        Sceleton skeleton = new Sceleton(
                            content,
                            new Rectangle((int)item.Key.X * Tilesize, (int)item.Key.Y * Tilesize, 64, 128),
                            new Rectangle(0, 0, 128, 128)
                        );
                        Enemies.Add(skeleton);
                        break;

                    case 0:


                        Ghoul ghoul = new Ghoul(
                            content,
                            new Rectangle((int)item.Key.X * Tilesize, (int)item.Key.Y * Tilesize, 64, 64),
                            new Rectangle(0,0, 64,64)
                            );
                        Enemies.Add(ghoul);
                        break;

                    case 2:
                        // Worm enemy

                        Worm worm = new Worm(
                            content,
                            new Rectangle((int)item.Key.X * Tilesize, (int)item.Key.Y * Tilesize, 64, 64),
                            new Rectangle(0, 0, 128, 128)
                        );
                        Enemies.Add(worm);
                        break;
                    
                    case 4:
                        
                        DarkKnightFB darkKnight = new DarkKnightFB(
                            content,
                            new Rectangle((int)item.Key.X * Tilesize, (int)item.Key.Y * Tilesize, 128, 192),
                            new Rectangle(0, 0, 128, 192)
                        );
                        
                        Enemies.Add(darkKnight);

                        break;
                    
                }
            }
        }
    }
    
    
    public void SpawnBuffs(ContentManager content)
    {
        
        Buffs.Clear();
        
        
        foreach (var item in buffs)
        {
            
            if (item.Value != -1)
            {
                switch (item.Value)
                {
                    case 2:
                        Buff healthBuff = new Buff(
                            BuffType.Heal,
                            new Vector2(item.Key.X * Tilesize, item.Key.Y * Tilesize),
                            20,
                            content
                        );
                        Buffs.Add(healthBuff);
                        
                        break;

                    case 1:
                        Buff speedBuff = new Buff(
                            BuffType.IncreaseSpeed,
                            new Vector2(item.Key.X * Tilesize, item.Key.Y * Tilesize),
                            1.5f,
                            content
                        );
                        Buffs.Add(speedBuff);
                        
                        break;

                    case 0:
                        Buff damageBuff = new Buff(
                            BuffType.IncreaseDamage,
                            new Vector2(item.Key.X * Tilesize, item.Key.Y * Tilesize),
                            20,
                            content
                        );
                        Buffs.Add(damageBuff);
                        
                        break;
                    
                }
            }
        }
    }


    public void Draw(SpriteBatch spriteBatch)
    {
        
            float parallaxFactor = 0.03f; // Very subtle horizontal movement
            float offsetX = Main.MainHero.Rect.X * parallaxFactor;
            
            // Calculate blend factor based on player's Y position
            
            for (int i = 0; i < Width; i += background.Width)
            {
                // Position for background
                Vector2 position = new Vector2(
                    i - offsetX % background.Width,
                    Main.MainHero.Rect.Y - background.Height / 2
                );
                
                spriteBatch.Draw(background, position, Color.White);
               
            }


            foreach (var item in foreground)
            {
                int display_tilesize = 64;
                int num_tiles_per_row = 32;
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
            
            foreach (var item in decor2)
            {
                int display_tilesize = 64;
                int num_tiles_per_row = 19;
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
                
                spriteBatch.Draw(textureAtlas_decor, drect, src, Color.White);

            }
            
            
            foreach (var item in decor)
            {
                int display_tilesize = 64;
                int num_tiles_per_row = 19;
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
                
                spriteBatch.Draw(textureAtlas_decor, drect, src, Color.White);

            }
            
            
            
            
            foreach (var item in forest)
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
                
                spriteBatch.Draw(textureAtlas_forest, drect, src, Color.White);

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