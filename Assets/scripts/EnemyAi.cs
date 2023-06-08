using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAi : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rig;
    /// <summary>
    /// �����ƶ�����0�� 1�� 2�� 3��
    /// </summary>
    private int directionId = 0;
    private Vector2 dirVector;
    private float speed = 0.05f;
    private float rayDistance = 0.5f;
    /// <summary>
    /// �ж��Ƿ�����ƶ�����������ܶ���wallʱ���˲�ͣ������
    /// </summary>
    private bool canMove = true;

    private static List<EnemyAi> enemies = new List<EnemyAi>(); // �����б�

    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        //InitDirection(Random.Range(0, 4));//����ҿ�����
        anim = GetComponent<Animator>();

    }
    public void Init()
    {
        canMove = true;
        InitDirection(Random.Range(0, 4));
    }
    /// <summary>
    /// �����ƶ�����0�� 1�� 2�� 3��
    /// </summary>
    /// <param name="dir"></param>
    private void InitDirection(int dir)
    {
        directionId = dir;
        anim.SetInteger("Direction", directionId);
        switch (directionId)
        {
            case 0:
                dirVector = Vector2.up;//up��x��ʼ��Ϊ0��yΪ1��ͨ������speed�ճ��Ը����ֿɿ����ƶ�ֵ
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
        if (canMove)//�������пռ�ʱ�Ž����ƶ���canMoveΪtureʱ��
            rig.MovePosition((Vector2)transform.position + dirVector * speed); //ͨ�����dieVertorֵ�ٳ���speed����Enemy���ƶ� 
        else
            ChangeDir();

    }

    //    OnTriggerEnter2D �������� �� OnCollisionEnter2D����ײ��
    //�ٴ���������ײ����һ������
    //������Ҫ����ײ���ʱʹ����ײ��
    //����ײ����Ч�ı�Ҫ��������ײ��˫��A,B��������Collider���ұ�����һ����ѡ��isTrigger�����������ƶ���һ��Ҫ����rigidbody��
    //�ܵ���Ҫ����ײ���ȴ�ֲ��������ײЧ��ʱ���Ϳ�����isTrigger(unity�й�ѡ)�������״̬�´��������Ч����ײ���ʧЧ��
    //is Trigger�������ѡ�˸����ԣ���ô���������һ�����壬���ζ���ʵ�������������ã�����������Դ����������������ʱ������ײ�¼�������������ô������ͻ���������¼���
    private void OnTriggerEnter2D(Collider2D collision)//���ҽ����ڼ�⵽��ײ���壨collision��ʱ���ø÷���
    {
        //����⵽BombEffectʱҲ��������
        if (collision.CompareTag(Tags.BombEffect) && gameObject.activeSelf)
        {
            EnemyDieAni();
            GameController.Instance.enemyCount--;
            //�ӷ�
            GameController.Instance.score += 100;
            //Destroy(gameObject);
            //ObjectPool.Instance.Add(ObjectType.Enemy, gameObject); 
        }
        //���Enemy֮��Ҳ��������ײ�����⣺��ǽ����ϱ�ǩ����������������иñ�ǩʱ�Ž���Changedir
        //if (collision.CompareTag(Tags.SuperWall) || collision.CompareTag(Tags.Wall))
        //    ChangeDir();
        if (canMove)
            if (collision.CompareTag(Tags.SuperWall) || collision.CompareTag(Tags.Wall ))
            {
                //��λ
                transform.position = new Vector2(Mathf.RoundToInt(transform.position.x),
                    Mathf.RoundToInt(transform.position.y));
                ChangeDir();
            }
    }

    //�������߱��ڼ����ӻ�
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, rayDistance, 0));
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -rayDistance, 0));
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(rayDistance, 0, 0));
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(-rayDistance, 0, 0));
    }
    /// <summary>
    /// ����ı�
    /// </summary>
    private void ChangeDir()
    {
        //��λ
        transform.position = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));

        //���ü��ϴ洢Enemy���ƶ��ķ����ĸ��������⣩
        List<int> dirList = new List<int>();
        //���߷����⣺����⵽���������Collider��component��
        //print(Physics2D.Raycast(transform.position, Vector2.up, 1)); //����ֵbool����
        //ps:if (Physics2D.Raycast(transform.position, Vector2.up, 1) == false)���౨��
        //ArgumentOutOfRangeException: Index was out of range. Must be non-negative and less than the size of the collection.
        //����Ϊ���ߴ��������ķ���ֻҪ��⵽collider�ͷ������Ӧ��ֵ�������屾���Դ�collider�ᱻ��⵽������Ҫʹ��layerMask���к���
        //˼·����wallPre��superwallPre��layer�ŵ�ͬһ�㣬��Enemy��layerMask����Ϊֻ�����һ�㣨1 << 8:ֻ���ڰ˲㣬0 << 8���Եڰ˲㣩

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
    /// ���������������Ž���
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