using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Tales_of_Everlight.Org;

public enum MusicType
{
    Level1,
    Level2,
    Menu
}

public class Music
{
    
    private MusicType _type;
    public MusicType Type
    {
        get => _type;
        set
        {
            _type = value;
            switch (_type)
            {
                case MusicType.Level1:
                    Song = SongLevel1;
                    break;
                case MusicType.Level2:
                    Song = SongLevel2;
                    break;
                case MusicType.Menu:
                    Song = SongMenu;
                    break;
            }
        }
    }
    public Song Song { get; private set; }
    public Song SongLevel1;
    public Song SongLevel2;
    public Song SongMenu;
    public float Volume = 0.25f;
    public bool IsPlaying;
    
    public Music(ContentManager content)
    {
        SongLevel1 = content.Load<Song>("_level1_sound");
        SongLevel2 = content.Load<Song>("_level2_sound");
        SongMenu = content.Load<Song>("_menu_sound");
    }

    
    
    
    public void PlayMusic()
    {
        if (!IsPlaying)
        {
            MediaPlayer.Pause();
        }
        else
        {
            MediaPlayer.Play(Song);
            MediaPlayer.Volume = Volume;
            MediaPlayer.IsRepeating = true;
        
            MediaPlayer.Play(Song);
            
        }
        
        
       
        
    }
    
}