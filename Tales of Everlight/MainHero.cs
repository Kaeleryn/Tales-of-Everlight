using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tales_of_Everlight;

public class MainHero : Actor
{
    public MainHero(Texture2D texture, Vector2 position, int rows, int columns) : base(texture, position, rows, columns)
    {
    }

    public MainHero()
    {
    }
}