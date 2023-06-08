using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public static UIController Instance; 
    public Text text_HP, text_Time, text_Score;
    public GameObject gameOverPanel;
    public Animator levelFadeAnim;


    private void Awake()
    {
        Instance = this;
        Init();
    }

    private void Init()
    {
        gameOverPanel.transform.Find("btn_Again").GetComponent<Button>().onClick.AddListener(() =>
        {
            Time.timeScale = 1;
            //���ص�ǰ����scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        });
        gameOverPanel.transform.Find("btn_Main").GetComponent<Button>().onClick.AddListener(() =>
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("Start");
        });
    }
    /// <summary>
    /// Top
    /// </summary>
    /// <param name="hp"></param>
    /// <param name="level"></param>
    /// <param name="time"></param>
    /// <param name="score"></param>
    public void Refresh(int hp, int level, int time, int score)
    {
        //��ҪתΪstr������ʾ
        text_HP.text = "HP:" + hp.ToString();
        text_Time.text = "T:" + time.ToString();
        int Sc = score;
        string str = Sc.ToString("D8");
        text_Score.text = str;
    }

    public void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
    }

    /// <summary>
    /// �ؿ����ɶ���
    /// </summary>
    public void PlayLevelFade(int levelIndex, int score)
    {
        AudioController.Instance.PlayFade();
        Time.timeScale = 0;
        int Sc = score;
        string str = Sc.ToString("D8");
        levelFadeAnim.transform.Find("txt_Level").GetComponent<Text>().text = "1 - " + levelIndex.ToString();
        levelFadeAnim.transform.Find("text_Sc").GetComponent<Text>().text = "SC  " + str;
        levelFadeAnim.Play("LevelFade", 0, 0);//normalize���������Ž�������
        //StartCoroutine("Dealy");
        startDealy = true;
    }

    //��������Ϸ�ӳ���timescale=0ʱ��������ΪЭ����ʱ��Ҳ��timescaleӰ��
    //IEnumerator Dealy()
    //{
    //    yield return new WaitForSeconds(1f);
    //    Time.timeScale = 1;
    //}

    //�Ľ�ʹ��unscaled����
    private bool startDealy = false;
    private float timer;

    private void Update()
    {
        if (startDealy)
        {
            timer += Time.unscaledDeltaTime;
            if (timer >= 1f)
            {
                startDealy = false;
                Time.timeScale = 1;
                timer = 0;
            }
        }
    }

}
