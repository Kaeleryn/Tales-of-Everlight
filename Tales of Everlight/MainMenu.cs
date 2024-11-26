using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tales_of_Everlight
{
    public class MainMenu
    {
        private Texture2D _backgroundTexture;
        private SpriteFont _font;
        private bool _isVisible;

        public MainMenu(Texture2D backgroundTexture, SpriteFont font)
        {
            _backgroundTexture = backgroundTexture;
            _font = font;
            _isVisible = true;
        }

        public bool IsVisible
        {
            get => _isVisible;
            set => _isVisible = value;
        }

        public void Update(KeyboardState currentKeyState, KeyboardState previousKeyState)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, 1920, 1080), Color.Black * 0.5f);
            spriteBatch.DrawString(_font, "MAIN MENU", new Vector2(800, 300), Color.White);
            spriteBatch.DrawString(_font, "A, D: Move the main hero", new Vector2(800, 500), Color.White);
            spriteBatch.DrawString(_font, "Space: Jump", new Vector2(800, 550), Color.White);
            spriteBatch.DrawString(_font, "Escape: Pause", new Vector2(800, 600), Color.White);
            spriteBatch.DrawString(_font, "I: Toggle HUD visibility", new Vector2(800, 650), Color.White);
            spriteBatch.DrawString(_font, "Delete: Exit the game", new Vector2(800, 700), Color.White);
            spriteBatch.End();
        }
    }
}