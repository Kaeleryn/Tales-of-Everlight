using System.IO;
using System.Threading.Tasks;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Myra;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;

namespace Tales_of_Everlight
{
    public class MainMenu
{
    private readonly Game _game;
    private Desktop _desktop;
    private MenuItem _startGameItem;

    public MainMenu(Game game)
    {
        _game = game;
        MyraEnvironment.Game = _game;
    }

    public Desktop InitializeMenu()
    {
        var panel = new Panel
        {
            Background = new SolidBrush("#33135CFF")
        };

        var fontSystem = new FontSystem();
        fontSystem.AddFont(File.ReadAllBytes("Content/KnightWarrior.otf"));
        var customFont = fontSystem.GetFont(80);
        var titleFont = fontSystem.GetFont(160);

        var text = new Label
        {
            Text = "Tales of Everlight",
            HorizontalAlignment = HorizontalAlignment.Center,
            TextColor = new Color(255, 211, 0),
            Font = titleFont,
        };

        panel.Widgets.Add(text);

        var mainMenu = new VerticalMenu
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            LabelColor = new Color(256, 256, 256),
            Background = new SolidBrush("#00000000"),
            SelectionHoverBackground = new SolidBrush("#652EC7FF"),
            SelectionBackground = new SolidBrush("#DE38C8FF"),
            LabelHorizontalAlignment = HorizontalAlignment.Center,
            Border = new SolidBrush("#00000000"),
            LabelFont = customFont,
        };

        panel.Widgets.Add(mainMenu);

        _startGameItem = new MenuItem();
        UpdateStartGameText(); // Set the initial text

        var exitItem = new MenuItem
        {
            Text = "Exit",
        };

        _startGameItem.Selected += (s, a) =>
        {
            ((Main)_game).CurrentGameState = GameState.Playing;
            DelayedUpdateStartGameText(10);
        };

        exitItem.Selected += (s, a) =>
        {
            _game.Exit();
        };

        mainMenu.Items.Add(_startGameItem);
        mainMenu.Items.Add(exitItem);

        _desktop = new Desktop { Root = panel };
        return _desktop;
    }
    public async void DelayedUpdateStartGameText(int delayMilliseconds)
    {
        await Task.Delay(delayMilliseconds);
        UpdateStartGameText();
    }
    public void UpdateStartGameText()
    {
        if (((Main)_game).CurrentGameState == GameState.MainMenu)
        {
            _startGameItem.Text = "Start Game";
        }
        else if (((Main)_game).CurrentGameState == GameState.Playing)
        {
            _startGameItem.Text = "Resume Game";
        }
    }
}
}