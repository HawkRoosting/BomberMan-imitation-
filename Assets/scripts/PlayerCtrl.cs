using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class PlayerCtrl : MonoBehaviour
{
    public GameObject bombPre;
    private Animator anim;//Animator����
    private Rigidbody2D rig;
    private SpriteRenderer spriteRenderer;
    private Color color;

    /// <summary>
    /// ����Ƿ��������״̬��������״̬���޵�
    /// </summary>
    /// 
    private bool isInjured = false;

    //��������
    public float speed = 0.1f;
    public int HP = 1;
    public int range = 0;
    private float bombBoomTime = 0;
    public int bombCount = 1;

    /// <summary>
    /// ��ŵ�ǰ������ը��--��Ҫ���ڽ��ը����������һ�ص�����
    /// </summary>
    private List<GameObject> bombList = new List<GameObject>();



    private void Awake()
    {
        anim = GetComponent<Animator>();//��ȡ�������
        rig = GetComponent<Rigidbody2D>();//��ȡ�����ƶ������
        spriteRenderer = GetComponent<SpriteRenderer>();
        color = spriteRenderer.color;

    }

    //˼·��Animator������½�������floatֵ�����ڼ����ƶ���Horizontal��Vertical��
    //�ڽű�������ʹ����Ӧ������ȡ�жϼ����Ƿ��µļ��ֵ
    //�ٽ���ȡ����ֵ���ø�Animator������floatֵ�����ڼ����ƶ���Horizontal��Vertical��
    //Animator����и���������ֵ�����õ��������ƶ�������

    private void Update()
    {
        Move();
        CreatBomb();
    }

    /// <summary>
    /// player����GameController�г�ʼ���������ڴ˷�װ�Ը�������һЩ���Խ��з�װ:��ը��Χ Ѫ�� ��ը�ӳ�ʱ��
    /// </summary>
    /// <param name="range"></param>
    /// <param name="HP"></param>
    /// <param name="bombTime"></param>

    public void Init(int range, int HP, float bombTime)
    {
        this.range = range;
        this.HP = HP;
        this.bombBoomTime = bombTime;
    }

    private void Move()
    {
        float h = Input.GetAxis("Horizontal");//���ˮƽ�����������ݣ�AΪ-1��DΪ1(���°�����ֵ�Ǹ������𽥱仯������һ˲��仯)
        float v = Input.GetAxis("Vertical");//��ֱ����
        //print("h:" + h + "v:" + v);
        anim.SetFloat("Horizontal", h);//����Animator���Լ������float����ֵ
        anim.SetFloat("Vertical", v);
        //transform.position += new Vector3(h, v) * speed;//ͨ���޸�player�����x��yʵ���ƶ��������ƶ�������Rigidbody��һ����
        rig.MovePosition(transform.position + new Vector3(h, v) * speed);
    }

    private void CreatBomb()
    {
        if (Input.GetKeyDown(KeyCode.Space) && bombCount > 0)
        {
            //����ը����Ч
            AudioController.Instance.PlayFire();
            bombCount--;
            //GameObject bomb = Instantiate(bombPre);
            ////ը��λ���뵱ǰ����λ����ͬ�������и�λ����
            //bomb.transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
            ////����ը���ű�
            GameObject bomb =  ObjectPool.Instance.Get(ObjectType.Bomb, new Vector3(Mathf.RoundToInt(transform.position.x), 
                Mathf.RoundToInt(transform.position.y)));
            // �ӳ�0.75����޸�Collider����
            StartCoroutine(DelaySetCollider(bomb, 0.5f));
            //bomb.GetComponent<Bomb>().Init(range, bombBoomTime, AniFinFun);
            //Lambda���ʽ�´��ݷ����� AniFinFun->>>  () => {���ʽ��} ---�����Ͳ���д�����Ǹ�AniFinFun������
            bomb.GetComponent<Bomb>().Init(range, bombBoomTime, () => 
            {
                bombCount++;
                //��ը֮��ɾ��ը��
                bombList.Remove(bomb);
            });
            bombList.Add(bomb);
        }
    }

    private IEnumerator DelaySetCollider(GameObject bomb, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        // �޸�ը��Collider��isTrigger����Ϊfalse
        Collider2D collider = bomb.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.isTrigger = false;
        }
    }

    ///// <summary>
    ///// ���ݵ�Bomb.Init�е�ί��
    ///// </summary>
    //private void AniFinFun()
    //{
    //    bombCount++;
    //}

    public void ResetPlayer()
    {
        foreach (var item in bombList)
        {
            //��Ϊֻ�б�ըը�������Ż����ӣ����������ڱ�ը֮ǰ����ը��Ӧ�ö�ը��������ԭ
            bombCount++;
            //�رն�Ӧը����Э��---Э�̲�����ըЧ��
            item.GetComponent<Bomb>().StopAllCoroutines();
            ObjectPool.Instance.Add(ObjectType.Bomb, item);
        }
        bombList.Clear();
        //����ָ�
        StopCoroutine("Injured");
        color.a = 1;
        spriteRenderer.color = color;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isInjured == true) return;
        if (collision.CompareTag(Tags.Enemy) || collision.CompareTag(Tags.BombEffect))
        {
            
            //Ѫ��������Ϸ����
            if (HP <= 0)
            {
                GameController.Instance.PauseBackgroundMusic();
                AudioController.Instance.PlayDie();
                PlayerDieAni(); 
                return;
            }
            HP--;
            StartCoroutine("Injured", 2f);
        }

        
    } 
    /// <summary>
    /// ��������
    /// </summary>
    public void PlayerDieAni()
    {
        //ʱ����ͣ
        Time.timeScale = 0;
        anim.SetTrigger("Die");
    }

    private void DieAniFin()
    {
        GameController.Instance.GameOver();
    }

    /// <summary>
    /// player��ײ��⵽enemyʱ��͸���Ƚ��б仯
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator Injured(float time)
    {
        isInjured = true;
        for (int i = 0; i < time; i++)
        {
            
            color.a = 0; //aΪ͸����
            spriteRenderer.color = color;
            yield return new WaitForSeconds(0.25f);
            color.a = 1;
            spriteRenderer.color = color;
            yield return new WaitForSeconds(0.25f);//�������

        }
        isInjured = false;
    }

    public void AddSpeed(float value = 0.03f)
    {
        speed += value;
        if (speed > 0.2f)
            speed = 0.2f;
    }
}
