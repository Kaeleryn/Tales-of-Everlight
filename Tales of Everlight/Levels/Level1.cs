using System;
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
    public Dictionary<Vector2, int> enemies;
    public Dictionary<Vector2, int> buffs;
    public readonly int Width = 271 * 64;
    public int Tilesize = 64;

    public bool IsActive { get; set; } = true;

    public List<Enemy> Enemies { get; set; } = new List<Enemy>();
    public List<Buff> Buffs { get; set; } = new List<Buff>();


    public Dictionary<Vector2, int> Foreground => foreground;
    public Dictionary<Vector2, int> Collisions => collisions;
    public Dictionary<Vector2, int> Spikes => spikes;


    private Texture2D textureAtlas_foreground;
    private Texture2D textureAtlas_collisions;
    private Texture2D textureAtlas_spikes;
    private Texture2D textureAtlas_portal;

    public Texture2D BackgroundHigh { get; set; }
    public Texture2D BackgroundLow { get; set; }


    public Level1()
    {
        foreground = new Dictionary<Vector2, int>();
        collisions = new Dictionary<Vector2, int>();
        spikes = new Dictionary<Vector2, int>();
        enemies = new Dictionary<Vector2, int>();
        buffs = new Dictionary<Vector2, int>();
    }


    public void Initialize(ContentManager content)
    {
        foreground = LoadMap("Content/map/level1_foreground.csv");
        collisions = LoadMap("Content/map/level1_collisions.csv");
        spikes = LoadMap("Content/map/level1_spikes.csv");
        enemies = LoadMap("Content/map/level1_enemies.csv");
        buffs = LoadMap("Content/map/level1_buffs.csv");


        textureAtlas_foreground = content.Load<Texture2D>("tileset_ground");
        textureAtlas_collisions = content.Load<Texture2D>("tileset_collisions");
        textureAtlas_spikes = content.Load<Texture2D>("tileset_spikes");
        textureAtlas_portal = content.Load<Texture2D>("Portal");


        BackgroundHigh = content.Load<Texture2D>("Level1_background_high");
        BackgroundLow = content.Load<Texture2D>("Level1_background_low");
    }

    public void SpawnEnemies(ContentManager content)
    {
        Enemies.Clear();
        // Ghoul ghoul = new Ghoul(
        //     content,
        //     new Rectangle(1000, 500, 64, 64),
        //     new Rectangle(0, 0, 64, 64)
        // );
        DarkKnightFB DarkKnight = new DarkKnightFB(
            content,
            new Rectangle(1000, 500, 128, 192),
            new Rectangle(0, 0, 64, 64)
        );
        Enemies.Add(DarkKnight);
       // Enemies.Add(ghoul);
        PurpleWarrior purpleWarrior1 = new PurpleWarrior(
            content,
            new Rectangle(234 * Tilesize, 28 * Tilesize, 128, 128),
            new Rectangle(0, 0, 128, 128)
        );
        PurpleWarrior purpleWarrior2 = new PurpleWarrior(
            content,
            new Rectangle(229 * Tilesize, 60 * Tilesize, 128, 128),
            new Rectangle(0, 0, 128, 128)
        );
        Enemies.Add(purpleWarrior1);
        Enemies.Add(purpleWarrior2);

        foreach (var item in enemies)
        {
            if (item.Value != -1)
            {
                switch (item.Value)
                {
                    case 0:


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

                    case 2:


                        Mushroom mushroom = new Mushroom(
                            content,
                            new Rectangle((int)item.Key.X * Tilesize, (int)item.Key.Y * Tilesize, 64, 128),
                            new Rectangle(0, 0, 128, 128)
                        );
                        Enemies.Add(mushroom);
                        break;

                    case 3:
                        // Worm enemy

                        Worm worm = new Worm(
                            content,
                            new Rectangle((int)item.Key.X * Tilesize, (int)item.Key.Y * Tilesize, 64, 64),
                            new Rectangle(0, 0, 128, 128)
                        );
                        Enemies.Add(worm);
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
                    case 0:
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

                    case 2:
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
            float transitionHeight = 40 * 64; // 34 * Tilesize
            float blendRange = 10 * 64; // Transition range (10 tiles)
            float blendFactor = 0f;

            // Calculate how far the player is into the transition zone
            if (Main.MainHero.Rect.Y > transitionHeight - blendRange &&
                Main.MainHero.Rect.Y < transitionHeight + blendRange)
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
                    Main.MainHero.Rect.Y - BackgroundHigh.Height / 2
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