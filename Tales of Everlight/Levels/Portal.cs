using System.Drawing;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Tales_of_Everlight;

public class Portal
{
    public Texture2D PortalTexture;
    public Rectangle Rect;
    public Rectangle Srect;
    public int TotalFrames = 7;
    public int CurrentFrame = 0;
    public const int TILESIZE = 64;
    public const float FRAME_DURATION = 0.1f;
    public float FrameTimer = 0f;
    public bool IsVisible = false;

    public Portal(ContentManager content, Rectangle rect)
    {
        Rect = rect;
        PortalTexture = content.Load<Texture2D>("Portal");
    }
    
    public void Update(GameTime gameTime)
    {
        FrameTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (FrameTimer >= FRAME_DURATION)
        {
            CurrentFrame++;
            
            FrameTimer = 0f;
            
            if (CurrentFrame >= TotalFrames)
            {
                CurrentFrame = 0;
            }
            
        }
        
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if(!IsVisible) return;
        // Calculate the source rectangle based on the current animation frame
        // Assuming portal sprite sheet has frames arranged horizontally
        Srect = new Rectangle(CurrentFrame * 128, 0, 128, 128);

        // Draw the portal at its position using the correct frame
        spriteBatch.Draw(
            PortalTexture, 
            new Vector2(Rect.X, Rect.Y), 
            Srect, 
            Color.White, 
            0f, 
            Vector2.Zero, 
            1f,
            SpriteEffects.None,
            0f
        );
    }


}