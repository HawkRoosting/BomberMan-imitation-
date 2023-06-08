using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class EndGameManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    private bool isVideoEnd = false;

    private void Start()
    {
        videoPlayer.loopPointReached += OnVideoEnd;
        videoPlayer.Play();
    }

    private void Update()
    {
        if (isVideoEnd && Input.anyKeyDown)
        {
            Application.Quit();
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        isVideoEnd = true;
        videoPlayer.Pause();
        videoPlayer.frame = (long)videoPlayer.frameCount - 1;
    }
}
