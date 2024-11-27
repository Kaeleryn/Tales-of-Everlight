using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Button
{
    private SpriteFont _font;
    private string _text;
    private Vector2 _position;
    private Color _textColor;
    private Color _hoverColor;
    private Rectangle _boundingBox;
    private bool _isHovered;

    public event Action Click;

    public Button(SpriteFont font, string text, Vector2 position, Color textColor, Color hoverColor)
    {
        _font = font;
        _text = text;
        _position = position;
        _textColor = textColor;
        _hoverColor = hoverColor;
        UpdateBoundingBox();
    }

    public void UpdateBoundingBox()
    {
        Vector2 textSize = _font.MeasureString(_text);
        _boundingBox = new Rectangle((int)_position.X, (int)_position.Y, (int)textSize.X, (int)textSize.Y);
    }

    public void Update(MouseState mouseState)
    {
        _isHovered = _boundingBox.Contains(mouseState.Position);

        if (_isHovered && mouseState.LeftButton == ButtonState.Pressed)
        {
            Click?.Invoke();
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Color color = _isHovered ? _hoverColor : _textColor;
        spriteBatch.DrawString(_font, _text, _position, color);
    }
}