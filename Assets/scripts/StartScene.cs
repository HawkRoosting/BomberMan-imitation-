using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class StartScene : MonoBehaviour
{
    public Button btn_Normal, btn_Exit;
    public VideoPlayer videoPlayer;
    public GameObject startPageObject;

    private bool isVideoFinished = false;

    private void Awake()
    {
        btn_Normal.onClick.AddListener(() =>
        {
            StartGame();
        });

        //退出游戏
        btn_Exit.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }

    private void Start()
    {
        videoPlayer.loopPointReached += OnVideoEnd;
        videoPlayer.Play();
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        isVideoFinished = true;
        startPageObject.SetActive(true);
    }

    private void StartGame()
    {
        if (isVideoFinished)
        {
            SceneManager.LoadScene("Game");
        }
        else
        {
            // 显示提示信息等待视频播放完毕
        }
    }
}