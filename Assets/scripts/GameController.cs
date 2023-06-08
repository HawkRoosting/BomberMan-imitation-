 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    private MapController mapController;
    private PlayerCtrl playerCtrl;

    private float timer = 0f;
    public int time = 200;
    public int score = 0;
    //当前关卡数
    private int levelCount = 0;
    //难度系数：用于敌人生成数量
    private float degreeOfDifficuty = 1.5f;
    private GameObject player;
    public int enemyCount;
    public GameObject playerPre;




    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        backgroundMusicAudioSource = GetComponent<AudioSource>();
        mapController = GetComponent<MapController>();//获取统一物体下挂载的脚本MapController
        LevelCtrl();
    }

    private void Update()
    {
        LevelTimer();
        UIController.Instance.Refresh(playerCtrl.HP, levelCount, time, score);
        if (Input.GetKeyDown(KeyCode.N))
        {
            LevelCtrl(); //N键按下进入下一关
        }
        if (levelCount >= 8)
            SceneManager.LoadScene("over");
    }

    /// <summary>
    /// 关卡生成器
    /// </summary>
    public void LevelCtrl()
    {
        //地图矩阵：关卡每增加3关，x于y都增加2
        int x = 6 + 2 * (levelCount / 3);
        int y = 3 + 2 * (levelCount / 3);
        //控制最大值
        if (x > 18)
            x = 18;
        if (y > 15)
            y = 15;
        //时间控制
        time = levelCount * 50 + 200;
        //敌人生成
        enemyCount = (int)(levelCount * degreeOfDifficuty) + 1;
        if (enemyCount >= 15)
            enemyCount = 15;
        mapController.InitMap(x, y, x*y, enemyCount);
        //只在第一关开始时才初始化人物数据，避免进入下一关时也初始化人物数据
        if (player == null)
        {
            player = Instantiate(playerPre);
            playerCtrl = player.GetComponent<PlayerCtrl>();
            playerCtrl.Init(1, 3, 2);
        }
        //回收炸弹避免遗留到下一关
        playerCtrl.ResetPlayer();    
        player.transform.position = mapController.GetPlayerPos();
        //回收炸弹特效避免爆炸效果遗留到下一关
        GameObject[] effects = GameObject.FindGameObjectsWithTag(Tags.BombEffect);
        foreach (var item in effects)
        {
            ObjectPool.Instance.Add(ObjectType.BombEffect, item);
        }
        //直接通过camera获取Camera下脚本 
        Camera.main.GetComponent<CameraFollow>().Init(player.transform, x, y);
        levelCount++;
        //停止背景音乐
        GameController.Instance.PauseBackgroundMusic();
        UIController.Instance.PlayLevelFade(levelCount, score);
    }
    /// <summary>
    /// 关卡计时
    /// </summary>
    private void LevelTimer()
    {
        if(time <= 0)
        {
            if (playerCtrl.HP > 0)
            {
                playerCtrl.HP--;
                time = 200;
                return; 
            }

            //倒计时结束
            PauseBackgroundMusic();
            AudioController.Instance.PlayDie();
            playerCtrl.PlayerDieAni();
            return;
        }
        timer += Time.deltaTime;
        if (timer >= 1.0f)
        {
            time--;
            timer = 0.0f;
        }
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    public void GameOver()
    {
        
        //显示游戏结束页面
        UIController.Instance.ShowGameOverPanel();
    }

    /// <summary>
    /// 加载下一关
    /// </summary>
    public void LoadNextLevel()
    {
        if(enemyCount <= 0)
            LevelCtrl();
    }

    //Bomb直接调用MapController中issuperwall函数不具区别度故进行中间封装
    public bool IsSuperWall(Vector2 pos)
    {
        return mapController.IsSuperWall(pos);
    }

    //音乐管理
    private AudioSource backgroundMusicAudioSource; // 背景音乐的 AudioSource 组件


    // 在某个条件下暂停背景音乐
    public void PauseBackgroundMusic()
    {
        if (backgroundMusicAudioSource != null)
        {
            backgroundMusicAudioSource.Pause(); // 暂停背景音乐的播放
        }
    }

    // 在某个条件下恢复背景音乐
    public void ResumeBackgroundMusic()
    {
        if (backgroundMusicAudioSource != null)
        {
            backgroundMusicAudioSource.UnPause(); // 恢复背景音乐的播放
        }
    }

}
