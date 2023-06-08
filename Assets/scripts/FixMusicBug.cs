using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixMusicBug : MonoBehaviour
{
    public void PlayBackGroundMusic()
    {
        GameController.Instance.ResumeBackgroundMusic();
    }
}
