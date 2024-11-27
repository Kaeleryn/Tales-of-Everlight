using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tales_of_Everlight;

public class PauseMenu
{
    private List<Button> _buttons;
    private SpriteFont _font;
    private Texture2D _backgroundTexture;
    private Game _game;

    public bool IsVisible { get; set; }

    public PauseMenu(Game game, Texture2D backgroundTexture, SpriteFont font)
    {
        _game = game;
        _backgroundTexture = backgroundTexture;
        _font = font;
        _buttons = new List<Button>();

        Button resumeButton = new Button(_font, "Resume", new Vector2(400, 300), Color.White, Color.Yellow);
        resumeButton.Click += ResumeButton_Click;
        _buttons.Add(resumeButton);

        Button exitToMainMenuButton = new Button(_font, "Exit to main menu", new Vector2(400, 400), Color.White, Color.Yellow);
        exitToMainMenuButton.Click += ExitToMainMenuButton_Click;
        _buttons.Add(exitToMainMenuButton);

        Button exitButton = new Button(_font, "Exit", new Vector2(400, 500), Color.White, Color.Yellow);
        exitButton.Click += ExitButton_Click;
        _buttons.Add(exitButton);
    }

    private void ResumeButton_Click()
    {
        IsVisible = false;
    }

    private void ExitToMainMenuButton_Click()
    {
        IsVisible = false;
        ((Main)_game).MainMenu.IsVisible = true;
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

    public static PauseMenu LoadContent(Game game, ContentManager content)
    {
        Texture2D backgroundTexture = content.Load<Texture2D>("menuBackground");
        SpriteFont font = content.Load<SpriteFont>("menuButtonFont");
        return new PauseMenu(game, backgroundTexture, font);
    }
}