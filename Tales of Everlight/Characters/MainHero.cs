using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tales_of_Everlight;

public class MainHero : Actor
{
    public MainHero(Texture2D texture, Rectangle rect, Rectangle srect, int rows, int columns) : base(texture, rect, srect, rows, columns)
    {
    }

    public MainHero()
    {
    }
}