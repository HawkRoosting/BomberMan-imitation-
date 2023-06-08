using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class PlayerCtrl : MonoBehaviour
{
    public GameObject bombPre;
    private Animator anim;//Animator主键
    private Rigidbody2D rig;
    private SpriteRenderer spriteRenderer;
    private Color color;

    /// <summary>
    /// 标记是否除以受伤状态，在受伤状态下无敌
    /// </summary>
    /// 
    private bool isInjured = false;

    //人物属性
    public float speed = 0.1f;
    public int HP = 1;
    public int range = 0;
    private float bombBoomTime = 0;
    public int bombCount = 1;

    /// <summary>
    /// 存放当前场景中炸弹--主要用于解决炸弹遗留到下一关的问题
    /// </summary>
    private List<GameObject> bombList = new List<GameObject>();



    private void Awake()
    {
        anim = GetComponent<Animator>();//获取自身组件
        rig = GetComponent<Rigidbody2D>();//获取用于移动的组件
        spriteRenderer = GetComponent<SpriteRenderer>();
        color = spriteRenderer.color;

    }

    //思路：Animator组件中新建了两个float值（用于键盘移动的Horizontal和Vertical）
    //在脚本中首先使用相应方法获取判断键盘是否按下的监测值
    //再将获取到的值设置给Animator中两个float值（用于键盘移动的Horizontal和Vertical）
    //Animator组件中根据这两个值及设置的条件控制动画播放

    private void Update()
    {
        Move();
        CreatBomb();
    }

    /// <summary>
    /// player是在GameController中初始化，所以在此封装以个方法对一些属性进行封装:爆炸范围 血量 爆炸延迟时间
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
        float h = Input.GetAxis("Horizontal");//检测水平方向输入内容：A为-1，D为1(按下按键后值是浮点数逐渐变化而不是一瞬间变化)
        float v = Input.GetAxis("Vertical");//垂直方向
        //print("h:" + h + "v:" + v);
        anim.SetFloat("Horizontal", h);//设置Animator中自己定义的float类型值
        anim.SetFloat("Vertical", v);
        //transform.position += new Vector3(h, v) * speed;//通过修改player物体的x、y实现移动（测试移动方法与Rigidbody不一样）
        rig.MovePosition(transform.position + new Vector3(h, v) * speed);
    }

    private void CreatBomb()
    {
        if (Input.GetKeyDown(KeyCode.Space) && bombCount > 0)
        {
            //放置炸弹音效
            AudioController.Instance.PlayFire();
            bombCount--;
            //GameObject bomb = Instantiate(bombPre);
            ////炸弹位置与当前人物位置相同，并进行复位规整
            //bomb.transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
            ////调用炸弹脚本
            GameObject bomb =  ObjectPool.Instance.Get(ObjectType.Bomb, new Vector3(Mathf.RoundToInt(transform.position.x), 
                Mathf.RoundToInt(transform.position.y)));
            // 延迟0.75秒后修改Collider属性
            StartCoroutine(DelaySetCollider(bomb, 0.5f));
            //bomb.GetComponent<Bomb>().Init(range, bombBoomTime, AniFinFun);
            //Lambda表达式下传递方法： AniFinFun->>>  () => {表达式；} ---这样就不用写下面那个AniFinFun方法了
            bomb.GetComponent<Bomb>().Init(range, bombBoomTime, () => 
            {
                bombCount++;
                //爆炸之后删除炸弹
                bombList.Remove(bomb);
            });
            bombList.Add(bomb);
        }
    }

    private IEnumerator DelaySetCollider(GameObject bomb, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        // 修改炸弹Collider的isTrigger属性为false
        Collider2D collider = bomb.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.isTrigger = false;
        }
    }

    ///// <summary>
    ///// 传递到Bomb.Init中的委托
    ///// </summary>
    //private void AniFinFun()
    //{
    //    bombCount++;
    //}

    public void ResetPlayer()
    {
        foreach (var item in bombList)
        {
            //因为只有爆炸炸弹数量才会增加，所以这里在爆炸之前回收炸弹应该对炸弹数量还原
            bombCount++;
            //关闭对应炸弹的协程---协程产生爆炸效果
            item.GetComponent<Bomb>().StopAllCoroutines();
            ObjectPool.Instance.Add(ObjectType.Bomb, item);
        }
        bombList.Clear();
        //人物恢复
        StopCoroutine("Injured");
        color.a = 1;
        spriteRenderer.color = color;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isInjured == true) return;
        if (collision.CompareTag(Tags.Enemy) || collision.CompareTag(Tags.BombEffect))
        {
            
            //血量不足游戏结束
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
    /// 死亡动画
    /// </summary>
    public void PlayerDieAni()
    {
        //时间暂停
        Time.timeScale = 0;
        anim.SetTrigger("Die");
    }

    private void DieAniFin()
    {
        GameController.Instance.GameOver();
    }

    /// <summary>
    /// player碰撞检测到enemy时让透明度进行变化
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator Injured(float time)
    {
        isInjured = true;
        for (int i = 0; i < time; i++)
        {
            
            color.a = 0; //a为透明度
            spriteRenderer.color = color;
            yield return new WaitForSeconds(0.25f);
            color.a = 1;
            spriteRenderer.color = color;
            yield return new WaitForSeconds(0.25f);//程序挂起

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
