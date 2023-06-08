using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAi : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rig;
    /// <summary>
    /// 敌人移动方向：0上 1下 2左 3右
    /// </summary>
    private int directionId = 0;
    private Vector2 dirVector;
    private float speed = 0.05f;
    private float rayDistance = 0.5f;
    /// <summary>
    /// 判断是否可以移动，解决当四周都有wall时敌人不停动问题
    /// </summary>
    private bool canMove = true;

    private static List<EnemyAi> enemies = new List<EnemyAi>(); // 敌人列表

    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        //InitDirection(Random.Range(0, 4));//左闭右开区间
        anim = GetComponent<Animator>();

    }
    public void Init()
    {
        canMove = true;
        InitDirection(Random.Range(0, 4));
    }
    /// <summary>
    /// 敌人移动方向：0上 1下 2左 3右
    /// </summary>
    /// <param name="dir"></param>
    private void InitDirection(int dir)
    {
        directionId = dir;
        anim.SetInteger("Direction", directionId);
        switch (directionId)
        {
            case 0:
                dirVector = Vector2.up;//up将x初始化为0，y为1，通过设置speed空乘以改数字可控制移动值
                break;
            case 1:
                dirVector = Vector2.down;
                break;
            case 2:
                dirVector = Vector2.left;
                break;
            case 3:
                dirVector = Vector2.right;
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        if (canMove)//当四周有空间时才进行移动（canMove为ture时）
            rig.MovePosition((Vector2)transform.position + dirVector * speed); //通过随机dieVertor值再乘以speed控制Enemy的移动 
        else
            ChangeDir();

    }

    //    OnTriggerEnter2D ：触发器 和 OnCollisionEnter2D：碰撞器
    //①触发器是碰撞器的一个功能
    //②在想要做碰撞检测时使用碰撞器
    //③碰撞器生效的必要条件，碰撞的双方A,B都必须有Collider（且必须有一个勾选了isTrigger），其中有移动的一方要带有rigidbody。
    //④当想要做碰撞检测却又不想产生碰撞效果时，就可以用isTrigger(unity中勾选)，在这个状态下触发检测生效，碰撞检测失效。
    //is Trigger：如果勾选了该属性，那么该物体就是一个虚体，有形而无实，不受力的作用，其它对象可以穿过它，但是如果这时满足碰撞事件产生条件，那么该物体就会产生触发事件。
    private void OnTriggerEnter2D(Collider2D collision)//当且仅当在检测到碰撞物体（collision）时调用该方法
    {
        //当检测到BombEffect时也进行销毁
        if (collision.CompareTag(Tags.BombEffect) && gameObject.activeSelf)
        {
            EnemyDieAni();
            GameController.Instance.enemyCount--;
            //加分
            GameController.Instance.score += 100;
            //Destroy(gameObject);
            //ObjectPool.Instance.Add(ObjectType.Enemy, gameObject); 
        }
        //解决Enemy之间也发触发碰撞的问题：给墙体加上标签，当碰到的物体带有该标签时才进行Changedir
        //if (collision.CompareTag(Tags.SuperWall) || collision.CompareTag(Tags.Wall))
        //    ChangeDir();
        if (canMove)
            if (collision.CompareTag(Tags.SuperWall) || collision.CompareTag(Tags.Wall ))
            {
                //复位
                transform.position = new Vector2(Mathf.RoundToInt(transform.position.x),
                    Mathf.RoundToInt(transform.position.y));
                ChangeDir();
            }
    }

    //绘制射线便于检测可视化
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, rayDistance, 0));
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -rayDistance, 0));
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(rayDistance, 0, 0));
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(-rayDistance, 0, 0));
    }
    /// <summary>
    /// 方向改变
    /// </summary>
    private void ChangeDir()
    {
        //复位
        transform.position = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));

        //设置集合存储Enemy可移动的方向（四个方向均检测）
        List<int> dirList = new List<int>();
        //射线发射检测：被检测到物体必须有Collider（component）
        //print(Physics2D.Raycast(transform.position, Vector2.up, 1)); //返回值bool类型
        //ps:if (Physics2D.Raycast(transform.position, Vector2.up, 1) == false)此类报错：
        //ArgumentOutOfRangeException: Index was out of range. Must be non-negative and less than the size of the collection.
        //是因为射线从物体中心发出只要检测到collider就返回其对应的值，而物体本身自带collider会被检测到所以需要使用layerMask进行忽略
        //思路：将wallPre与superwallPre的layer放到同一层，给Enemy的layerMask设置为只检测这一层（1 << 8:只检测第八层，0 << 8忽略第八层）

        if (Physics2D.Raycast(transform.position, Vector2.up, rayDistance, 1 << 6) == false)
            dirList.Add(0);
        if (Physics2D.Raycast(transform.position, Vector2.down, rayDistance, 1 << 6) == false)
            dirList.Add(1);
        if (Physics2D.Raycast(transform.position, Vector2.left, rayDistance, 1 << 6) == false)
            dirList.Add(2);
        if (Physics2D.Raycast(transform.position, Vector2.right, rayDistance, 1 << 6) == false)
            dirList.Add(3);
        if (dirList.Count > 0)
        {
            canMove = true;
            int index = Random.Range(0, dirList.Count);
            InitDirection(dirList[index]);
        }
        else canMove = false;
    }
    /// <summary>
    /// 气球死亡动画播放结束
    /// </summary>
    public void EnemyDieAniFin()
    {
        ObjectPool.Instance.Add(ObjectType.Enemy, gameObject);
    }

    public void EnemyDieAni()
    {
        anim.SetTrigger("EnemyDie");
    }
}