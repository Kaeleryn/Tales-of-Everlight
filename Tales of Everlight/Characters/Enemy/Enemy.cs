using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tales_of_Everlight;

public abstract class Enemy
{
    public static int Health { get; set; }
    public bool Damaged { get; set; }
    public bool IsDead { get; private set; }
    private double _timeSinceLastFrame;

    private float _deathAnimationTime = 0f;
    private float _deathAnimationDuration = 1.0f; // Duration of the death animation in seconds
    public double FrameTime = 0.1;
    private Texture2D Texture { get; set; }
    private Vector2 position;
    private Vector2 velocity;

    private int Rows { get; set; }
    private int Columns { get; set; }

    private int currentFrame;
    private int totalFrames;

    private bool isFacingLeft = false;
    private bool isMoving = true;
    private bool isJumping = true;

    private const float GRAVITY = 1.2f;

    public bool IsJumping
    {
        get => isJumping;
        set => isJumping = value;
    }

    public bool IsMoving
    {
        get => isMoving;
        set => isMoving = value;
    }

    public Vector2 Position
    {
        get => position;
        set => position = value;
    }

    public Vector2 Velocity
    {
        get => velocity;
        set => velocity = value;
    }

    public Rectangle BoundingBox
    {
        get
        {
            int width = Texture.Width / Columns;
            int height = Texture.Height / Rows;
            return new Rectangle((int)position.X + 25, (int)position.Y, width - 50, height - 30);
        }
    }

    public void TakeDamage(int damage)
    {
        Console.WriteLine("TakeDamage method called");
        Health -= damage;
        Damaged = true;

        if (Health <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        IsDead = true;
        // Handle enemy death logic here
        Console.WriteLine("Enemy has died.");
        // For example, you might want to remove the enemy from the game or play a death animation.
    }

    protected Enemy(Texture2D texture, Vector2 position, int rows, int columns)
    {
        Health = 100;
        Texture = texture;
        Position = position;

        Rows = rows;
        Columns = columns;
        currentFrame = 0;
        totalFrames = Rows * Columns;
        velocity = Vector2.Zero;
    }

    protected Enemy()
    {
        velocity = Vector2.Zero;
    }


    private List<Rectangle> getIntersectingTilesHorizontal(Rectangle target, int TILESIZE)
    {
        List<Rectangle> intersections = new();

        int leftTile = target.Left / TILESIZE;
        int rightTile = (target.Right - 1) / TILESIZE;
        int topTile = target.Top / TILESIZE;
        int bottomTile = (target.Bottom - 1) / TILESIZE;

        for (int x = leftTile; x <= rightTile; x++)
        {
            for (int y = topTile; y <= bottomTile; y++)
            {
                intersections.Add(new Rectangle(x, y, TILESIZE, TILESIZE));
            }
        }

        return intersections;
    }

    private List<Rectangle> getIntersectingTilesVertical(Rectangle target, int TILESIZE)
    {
        List<Rectangle> intersections = new();

        int leftTile = target.Left / TILESIZE;
        int rightTile = (target.Right - 1) / TILESIZE;
        int topTile = target.Top / TILESIZE;
        int bottomTile = (target.Bottom - 1) / TILESIZE;

        for (int x = leftTile; x <= rightTile; x++)
        {
            for (int y = topTile; y <= bottomTile; y++)
            {
                intersections.Add(new Rectangle(x, y, TILESIZE, TILESIZE));
            }
        }

        return intersections;
    }


    public void CollisionHandler(Dictionary<Vector2, int> collisions, int TILESIZE)
    {
        position += velocity;


        // Vertical collision detection
        var intersections = getIntersectingTilesVertical(BoundingBox, TILESIZE);
        bool hasVerticalCollision = false;
        foreach (var rect in intersections)
        {
            if (collisions.TryGetValue(new Vector2(rect.X, rect.Y), out int _val))
            {
                Rectangle collision = new Rectangle(
                    rect.X * TILESIZE,
                    rect.Y * TILESIZE,
                    TILESIZE,
                    TILESIZE
                );

                if (velocity.Y > 0.0f)
                {
                    position.Y = collision.Top - BoundingBox.Height;
                    isJumping = false;
                }
                else if (velocity.Y < 0.0f)
                {
                    position.Y = collision.Bottom;
                }

                velocity.Y = 0;
                hasVerticalCollision = true;
            }
        }

        // Horizontal collision detection
        intersections = getIntersectingTilesHorizontal(BoundingBox, TILESIZE);
        foreach (var rect in intersections)
        {
            if (collisions.TryGetValue(new Vector2(rect.X, rect.Y), out int _val))
            {
                Rectangle collision = new Rectangle(
                    rect.X * TILESIZE,
                    rect.Y * TILESIZE,
                    TILESIZE,
                    TILESIZE
                );

                if (velocity.X > 0.0f)
                {
                    position.X = collision.Left - BoundingBox.Width - 70; // Adjust for bounding box offset
                    isMoving = false;
                }
                else if (velocity.X < 0.0f)
                {
                    position.X = collision.Right - 70; // Adjust for bounding box offset
                    isMoving = false;
                }

                velocity.X = 0;
            }
        }

        if (!hasVerticalCollision)
        {
            isJumping = true;
        }
    }

    public void MovementHandler()
    {
        if (isMoving)
        {
            position.X += velocity.X;
        }

        if (isJumping)
        {
            velocity.Y += GRAVITY;
            position.Y += velocity.Y;
        }
    }

    public void Update(GameTime gameTime)
    {
        if (IsDead)
        {
            _deathAnimationTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            UpdateFrame(gameTime);
            if (_deathAnimationTime >= _deathAnimationDuration)
            {
                Main.RemoveEnemy(this);
                Console.WriteLine("Enemy has been removed.");
            }

            return;
        }

        UpdateFrame(gameTime);
    }

    public void UpdateFrame(GameTime gametime)
    {
        _timeSinceLastFrame += gametime.ElapsedGameTime.TotalSeconds;
        if (_timeSinceLastFrame >= FrameTime)
        {
            _timeSinceLastFrame -= FrameTime;

            currentFrame++;
            if (currentFrame >= totalFrames)
            {
                currentFrame = 0;
            }
        }
    }


    public void Draw(SpriteBatch spriteBatch, Vector2 location, Texture2D hitboxTexture)
    {
        int width = Texture.Width / Columns;
        int height = Texture.Height / Rows;
        int row = currentFrame / Columns;
        int column = currentFrame % Columns;


        Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
        SpriteEffects spriteEffects = isFacingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, width, height);


        if (IsDead)
        {
            float alpha = 1.0f - (_deathAnimationTime / _deathAnimationDuration);
            Color deathColor = Color.White * alpha;
            spriteBatch.Draw(Texture, location, sourceRectangle, deathColor, 0f, Vector2.Zero, 1f, spriteEffects, 0f);
        }
        else if (Damaged)
        {
            spriteBatch.Draw(Texture, location, sourceRectangle, Color.Red, 0f, Vector2.Zero, 1f, spriteEffects, 0f);
            Damaged = false;
        }
        else
        {
            spriteBatch.Draw(Texture, location, sourceRectangle, Color.White, 0f, Vector2.Zero, 1f, spriteEffects, 0f);
        }

        Rectangle boundingBox = BoundingBox;

        if (!IsDead)
        {
            // хітбокс
            spriteBatch.Draw(hitboxTexture, new Rectangle(boundingBox.X, boundingBox.Y, boundingBox.Width, 1),
                Color.Red); // Top
            spriteBatch.Draw(hitboxTexture, new Rectangle(boundingBox.X, boundingBox.Y, 1, boundingBox.Height),
                Color.Red); // Left
            spriteBatch.Draw(hitboxTexture,
                new Rectangle(boundingBox.X, boundingBox.Y + boundingBox.Height - 1, boundingBox.Width, 1),
                Color.Red); // Bottom
            spriteBatch.Draw(hitboxTexture,
                new Rectangle(boundingBox.X + boundingBox.Width - 1, boundingBox.Y, 1, boundingBox.Height),
                Color.Red); // Right
            //хітбокс
        }
    }
}