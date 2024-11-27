using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tales_of_Everlight
{
    public class SplashScreen
    {
        private Texture2D _splashTexture;
        private double _displayTime;
        private double _elapsedTime;

        public SplashScreen(Texture2D splashTexture, double displayTime)
        {
            _splashTexture = splashTexture;
            _displayTime = displayTime;
            _elapsedTime = 0;
        }

        public bool IsFinished => _elapsedTime >= _displayTime;

        public void Update(GameTime gameTime)
        {
            _elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(_splashTexture, new Rectangle(0, 0, 1920, 1080), Color.White);
            spriteBatch.End();
        }
    }
}