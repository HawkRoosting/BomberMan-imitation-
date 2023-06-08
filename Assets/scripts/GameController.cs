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
    //��ǰ�ؿ���
    private int levelCount = 0;
    //�Ѷ�ϵ�������ڵ�����������
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
        mapController = GetComponent<MapController>();//��ȡͳһ�����¹��صĽű�MapController
        LevelCtrl();
    }

    private void Update()
    {
        LevelTimer();
        UIController.Instance.Refresh(playerCtrl.HP, levelCount, time, score);
        if (Input.GetKeyDown(KeyCode.N))
        {
            LevelCtrl(); //N�����½�����һ��
        }
        if (levelCount >= 8)
            SceneManager.LoadScene("over");
    }

    /// <summary>
    /// �ؿ�������
    /// </summary>
    public void LevelCtrl()
    {
        //��ͼ���󣺹ؿ�ÿ����3�أ�x��y������2
        int x = 6 + 2 * (levelCount / 3);
        int y = 3 + 2 * (levelCount / 3);
        //�������ֵ
        if (x > 18)
            x = 18;
        if (y > 15)
            y = 15;
        //ʱ�����
        time = levelCount * 50 + 200;
        //��������
        enemyCount = (int)(levelCount * degreeOfDifficuty) + 1;
        if (enemyCount >= 15)
            enemyCount = 15;
        mapController.InitMap(x, y, x*y, enemyCount);
        //ֻ�ڵ�һ�ؿ�ʼʱ�ų�ʼ���������ݣ����������һ��ʱҲ��ʼ����������
        if (player == null)
        {
            player = Instantiate(playerPre);
            playerCtrl = player.GetComponent<PlayerCtrl>();
            playerCtrl.Init(1, 3, 2);
        }
        //����ը��������������һ��
        playerCtrl.ResetPlayer();    
        player.transform.position = mapController.GetPlayerPos();
        //����ը����Ч���ⱬըЧ����������һ��
        GameObject[] effects = GameObject.FindGameObjectsWithTag(Tags.BombEffect);
        foreach (var item in effects)
        {
            ObjectPool.Instance.Add(ObjectType.BombEffect, item);
        }
        //ֱ��ͨ��camera��ȡCamera�½ű� 
        Camera.main.GetComponent<CameraFollow>().Init(player.transform, x, y);
        levelCount++;
        //ֹͣ��������
        GameController.Instance.PauseBackgroundMusic();
        UIController.Instance.PlayLevelFade(levelCount, score);
    }
    /// <summary>
    /// �ؿ���ʱ
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

            //����ʱ����
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
    /// ��Ϸ����
    /// </summary>
    public void GameOver()
    {
        
        //��ʾ��Ϸ����ҳ��
        UIController.Instance.ShowGameOverPanel();
    }

    /// <summary>
    /// ������һ��
    /// </summary>
    public void LoadNextLevel()
    {
        if(enemyCount <= 0)
            LevelCtrl();
    }

    //Bombֱ�ӵ���MapController��issuperwall������������ȹʽ����м��װ
    public bool IsSuperWall(Vector2 pos)
    {
        return mapController.IsSuperWall(pos);
    }

    //���ֹ���
    private AudioSource backgroundMusicAudioSource; // �������ֵ� AudioSource ���


    // ��ĳ����������ͣ��������
    public void PauseBackgroundMusic()
    {
        if (backgroundMusicAudioSource != null)
        {
            backgroundMusicAudioSource.Pause(); // ��ͣ�������ֵĲ���
        }
    }

    // ��ĳ�������»ָ���������
    public void ResumeBackgroundMusic()
    {
        if (backgroundMusicAudioSource != null)
        {
            backgroundMusicAudioSource.UnPause(); // �ָ��������ֵĲ���
        }
    }

}
