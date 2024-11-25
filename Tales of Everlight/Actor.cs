using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;


namespace Tales_of_Everlight;

public abstract class Actor
{
    private Texture2D Texture { get; set; }
    private Vector2 position;
    private Vector2 velocity;

    private const float GRAVITY = 1.2f;

    public Rectangle BoundingBox
    {
        get
        {
            int width = Texture.Width / Columns;
            int height = Texture.Height / Rows;
            return new Rectangle((int)Position.X + 70, (int)Position.Y, width - 160, height);
        }
    }

    private int Rows { get; set; }
    private int Columns { get; set; }
    private int currentFrame;
    private int totalFrames;

    private bool isMoving = true;
    private bool isJumping = true;

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

    public Actor(Texture2D texture, Vector2 position, int rows, int columns)
    {
        Position = position;
        Texture = texture;
        Rows = rows;
        Columns = columns;
        currentFrame = 0;
        totalFrames = Rows * Columns;
        velocity = Vector2.Zero;
    }

    public Actor()
    {
        velocity = Vector2.Zero;
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
        // Update movement and jumping logic
        if (Keyboard.GetState().IsKeyDown(Keys.D) && !isMoving && position.X < 2440 - Texture.Width / Columns)
        {
            velocity.X = 15f;
            isMoving = true;
            currentFrame = (currentFrame + 1) % totalFrames;
        }
        else if (Keyboard.GetState().IsKeyDown(Keys.A) && !isMoving && position.X > 0)
        {
            velocity.X = -15f;
            isMoving = true;
            currentFrame = (currentFrame - 1 + totalFrames) % totalFrames;
        }
        else
        {
            velocity.X *= 0.8f;
            isMoving = false;
        }

        if (Keyboard.GetState().IsKeyDown(Keys.Space) && !isJumping)
        {
            isJumping = true;
            velocity.Y = -20;
            position.Y += velocity.Y;
        }

        if (!isJumping)
        {
            velocity.Y = 0f;
        }

        if (isJumping)
        {
            velocity.Y += GRAVITY;
        }

        if (position.Y > 800)
        {
            position.Y = 800;
            isJumping = false;
        }
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

    public void Draw(SpriteBatch spriteBatch, Vector2 location, Texture2D hitboxTexture)
    {
        int width = Texture.Width / Columns;
        int height = Texture.Height / Rows;
        int row = currentFrame / Columns;
        int column = currentFrame % Columns;


        Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
        Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, width, height);


        spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White);
        Rectangle boundingBox = BoundingBox;

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
    }
} // end of class