using Microsoft.Xna.Framework;

namespace Tales_of_Everlight;

public class Camera
{
    
    public Vector2 Position { get; set; }
    public float Zoom { get; private set; }
    public Rectangle Viewport { get; private set; }

    public Camera(Rectangle viewport)
    {
        Viewport = viewport;
        Position = new Vector2(viewport.Width / 2, viewport.Height / 2);
        Zoom = 1.0f;
    }

    public void Move(Vector2 amount)
    {
        Position += amount;
    }

    public Matrix GetTransformation()
    {
        return Matrix.CreateTranslation(new Vector3(-Position, 0)) *
               Matrix.CreateScale(Zoom) *
               Matrix.CreateTranslation(new Vector3(Viewport.Width * 0.5f, Viewport.Height * 0.5f, 0));
    }
    
} // Camera