using Microsoft.Xna.Framework;

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

    public Vector2 ScaleMouseCoordinates(Vector2 mousePosition, Vector2 currentResolution, Vector2 targetResolution)
    {
        float scaleX = targetResolution.X / currentResolution.X;
        float scaleY = targetResolution.Y / currentResolution.Y;
        return new Vector2(mousePosition.X * scaleX, mousePosition.Y * scaleY);
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