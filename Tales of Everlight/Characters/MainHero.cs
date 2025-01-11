using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tales_of_Everlight;

public class MainHero : Actor
{
    public MainHero(ContentManager content, Rectangle rect, Rectangle srect) : base(content, rect, srect)
    {
    }

    public MainHero()
    {
    }
}