using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tales_of_Everlight;

public class MainMenu
{
    private List<Button> _buttons;
    private SpriteFont _font;
    private Texture2D _backgroundTexture;
    private Game _game;

    public bool IsVisible { get; set; }

    public MainMenu(Game game, Texture2D backgroundTexture, SpriteFont font)
    {
        _game = game;
        _backgroundTexture = backgroundTexture;
        _font = font;
        _buttons = new List<Button>();

        Button startButton = new Button(_font, "Start Game", new Vector2(400, 300), Color.White, Color.Yellow);
        startButton.Click += StartButton_Click;
        _buttons.Add(startButton);

        Button exitButton = new Button(_font, "Exit", new Vector2(400, 400), Color.White, Color.Yellow);
        exitButton.Click += ExitButton_Click;
        _buttons.Add(exitButton);
    }

    private void StartButton_Click()
    {
        IsVisible = false;
        ((Main)_game).PauseMenu.IsVisible = false;
    }

    private void ExitButton_Click()
    {
        _game.Exit();
    }

    public void Update(MouseState mouseState)
    {
        foreach (var button in _buttons)
        {
            button.Update(mouseState);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        spriteBatch.Draw(_backgroundTexture, Vector2.Zero, Color.White);

        foreach (var button in _buttons)
        {
            button.Draw(spriteBatch);
        }

        spriteBatch.End();
    }

    public static MainMenu LoadContent(Game game, ContentManager content)
    {
        Texture2D backgroundTexture = content.Load<Texture2D>("menuBackGround");
        SpriteFont font = content.Load<SpriteFont>("menuButtonFont");
        return new MainMenu(game, backgroundTexture, font);
    }
}