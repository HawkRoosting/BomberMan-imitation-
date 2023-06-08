using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    //�Ѿ�ʹ�ö���ش��������ˣ�������Ҫ�������
    //public GameObject superWallPre, wallPre, , propPre, enemyPre;//��ȡunity����(��unity�ͷ��������������)
    //doorPre����ֻ��һ������Ҫʹ�ö����
    public GameObject doorPre;
    private int X, Y;//��ͼ������
    private List<Vector2> nullPointsList = new List<Vector2>(); //������㼯��
    private List<Vector2> superWallPointList = new List<Vector2>(); //����ǽ����
    //�洢�ţ�����ÿһ�ض���������
    private GameObject door;
    //�Ӷ������ȡ�����������弯�ϣ�����ʹ���������ͺͶ�Ӧ�Ķ���ع�ϵ�ֵ䣩---���ڹ������
    private Dictionary<ObjectType, List<GameObject>> poolObjectDic = new Dictionary<ObjectType, List<GameObject>>();
    //private List<GameObject> superWallList = new List<GameObject>();
    //private List<GameObject> wallList = new List<GameObject>();
    //private List<GameObject> propList = new List<GameObject>();
    //private List<GameObject> enemyList = new List<GameObject>();




    //Awake��start��������unity����ʱֱ�ӵ��õķ���
    //private void Awake()
    //{
    //    InitMap(7, 3, 50);

    //}

    //��ʼ����ͼ
    public void InitMap(int x, int y, int wallCount, int enemyCount)
    {
        //��ʼ��֮ǰ������һ�ؿ���Դ
        Recovery();
        X = x;
        Y = y;
        CreatSuperWall();
        FindNullPoints();
        Debug.Log(superWallPointList.Count); //����
        CreateWall(wallCount);
        CreateDoor();
        CreatProps();
        CreatEnemy(enemyCount);
    }
     
    private void Recovery()
    {
        //������һ�ؿ�ʱ�Լ��Ͻ������
        nullPointsList.Clear();
        superWallPointList.Clear();
        //����GameController�������岢ɾ�����壺�ؿ���������Ҫ����һ�����ɵ��������ɾ��
        //foreach (Transform  item in transform)
        //{
        //    Destroy(item.gameObject);
        //}
        //������Ҫɾ���������������
        foreach (var item in poolObjectDic)
        {
            foreach (var obj in item.Value) 
            {
                ObjectPool.Instance.Add(item.Key, obj);
            }
        }
        poolObjectDic.Clear();
    }

    //����ʵ��ǽ
    private void CreatSuperWall()   
    {
        //�ڲ�ʵ��ǽ
        //XY������Ϊ1ǡ�ð���
        for (int x = -X; x < X; x+=2) //�м��пո�
        {
            for (int y = -Y; y < Y; y+=2)
            {
                SpawnSuperWall(new Vector2(x, y));
                ////��ʼ������
                //GameObject wall = Instantiate(superWallPre, transform);  
                ////�޸�����
                //wall.transform.position=new Vector3(x, y);//y���Ĭ�ϲ�����Ϊ0 //vector3ʸ��λ��
            }

        }
        //���ʵ��ǽ(���²�)
        for (int x = -(X + 2); x <= X; x++)
        {
            SpawnSuperWall(new Vector2(x, Y));
            SpawnSuperWall(new Vector2(x, -(Y + 2)));
            ////��ʼ������
            //GameObject wall = Instantiate(superWallPre, transform);
            ////�޸�����
            //wall.transform.position = new Vector3(x, Y);//y���Ĭ�ϲ�����Ϊ0 //vector3ʸ��λ��

            //GameObject wall1 = Instantiate(superWallPre, transform);
            //wall1.transform.position = new Vector3(x, -Y - 2);
        }

        //���Ҳ�
        for (int y = -(Y + 1) ; y <= Y; y++)
        {
            SpawnSuperWall(new Vector2(-(X + 2), y));
            SpawnSuperWall(new Vector2(X, y));

            //GameObject wall = Instantiate(superWallPre, transform);
            //wall.transform.position = new Vector3(X, y);

            //GameObject wall1 = Instantiate(superWallPre, transform);
            //wall1.transform.position = new Vector3(-X - 2, y);
        }


    }
    //�����ظ�����̫�ཫ��ʼ�������Լ����겿�ַ�װһ��
    private void SpawnSuperWall(Vector2 pos) //����Ϊ����
    {

        //GameObject wall1 = Instantiate(superWallPre, transform);
        superWallPointList.Add(pos);
        GameObject superWall =  ObjectPool.Instance.Get(ObjectType.SuperWall, pos);
        //����
        if (poolObjectDic.ContainsKey(ObjectType.SuperWall) == false)//�������򴴽�
            poolObjectDic.Add(ObjectType.SuperWall, new List<GameObject>());
        poolObjectDic[ObjectType.SuperWall].Add(superWall);//���
    }

    /// <summary>
    /// �жϵ�ǰλ���Ƿ���ʵ��ǽ
    /// </summary>
    /// <returns></returns>
     public bool IsSuperWall(Vector2 pos)
    {
        if (superWallPointList.Contains(pos))
            return true;
        return false;
    }

    //�ɴݻٵ�ǽ: ��ȡʵ��ǽ��϶����position���������ȡֵ
    //��ȡ���пյĵط���λ
    private void FindNullPoints()
    {
        //Unity�з���X��Y��Χ
        for (int x = -(X+1); x <= X-1; x++)
        {
            if (-(X + 1) % 2 == x % 2)//�жϸ����Ƿ����ʵ��ǽ��-(X+1)Ϊ��ʼλ�ø�����ż�жϣ�
                for (int y = -(Y + 1); y <= Y - 1; y++)
                {
                    nullPointsList.Add(new Vector2(x, y));
                }
            else
                for (int y = -(Y + 1); y <= Y - 1; y += 2)
                {
                    nullPointsList.Add(new Vector2(x, y));
                }
        }
        //�Ƴ����Ͻ�����λ�ã���λ������Ϊ�����ʼ����λ�ü����ƶ���Χ
        nullPointsList.Remove(new Vector2(-(X + 1), Y - 1));
        nullPointsList.Remove(new Vector2(-X, Y - 1));
        nullPointsList.Remove(new Vector2(-(X + 1), Y - 2));
    }

    /// <summary>
    /// ���������ʼ����λ�÷���GameController�ű�ʹ��
    /// </summary> 
    /// <returns></returns>
    public Vector2 GetPlayerPos()
    {
        return new Vector2((-X - 1), Y - 1);
    }

    /// <summary>
    /// ���������ٵ�ǽ
    /// </summary>
    private void CreateWall(int wallCount) //�β�Ϊ�ɴݻ�ǽ����
    {
        //�㷨�Ŀ�����,���贫�����Ĳ������ڿյ��������Ϊ�յ�λ�İٷ�֮��ʮ
        if (wallCount >= nullPointsList.Count)
        {
            wallCount = (int)(nullPointsList.Count * 0.4f);
        }
            

        //����ӿյ㼯���������ȡ��λ
        for (int i = 0; i < wallCount; i++)
        {
            int index = Random.Range(0, nullPointsList.Count); //����ҿ�����
            GameObject wall = ObjectPool.Instance.Get(ObjectType.Wall, nullPointsList[index]);
            //GameObject wall = Instantiate(wallPre, transform);
            //wall.transform.position = nullPointsList[index];
            //�Ƴ��Ѿ���������Ŀյ�
            nullPointsList.RemoveAt(index);
            //��������
            if (poolObjectDic.ContainsKey(ObjectType.Wall) == false)
                //�������򴴽�
                poolObjectDic.Add(ObjectType.Wall, new List<GameObject>()); 
            poolObjectDic[ObjectType.Wall].Add(wall);//���

        }
    }

    /// <summary>
    /// ����ͨ����
    /// </summary>
    private void CreateDoor()
    {
        if (door == null)
            door = Instantiate(doorPre, transform);
        //GameObject door = Instantiate(doorPre, transform);
        //������һ�ؿ���������
        door.GetComponent<Door>().ResetDoor();
        int index = Random.Range(0, nullPointsList.Count);
        door.transform.position = nullPointsList[index];
        nullPointsList.RemoveAt(index);//�Ƴ��Ѿ�ʹ�õĿյ�λ
    }
    /// <summary>
    /// ��������
    /// </summary>
    private void CreatProps()
    {
        //���������뵱ǰ�յ������й�
        int count = Random.Range(0, 2 + (int)(nullPointsList.Count * 0.05f)); //��ʱ�յ���ٺ�����ʽ��Ϊ0������+2��֤������ֵ  
        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, nullPointsList.Count);
            GameObject prop = ObjectPool.Instance.Get(ObjectType.Prop, nullPointsList[index]);
            //GameObject prop = Instantiate(propPre, transform);
            //prop.transform.position = nullPointsList[index];
            nullPointsList.RemoveAt(index);
            //����
            if (poolObjectDic.ContainsKey(ObjectType.Prop) == false)
                //�������򴴽�
                poolObjectDic.Add(ObjectType.Prop, new List<GameObject>());
            poolObjectDic[ObjectType.Prop].Add(prop);//���
        }
    }
    /// <summary>
    /// ��������
    /// </summary>
    private void CreatEnemy(int count)
    {
        for (int i = 0; i < count; i++)
        {
            //ԭ���ɷ����������Ż��Ѿ���λ���޸ķ�װ���� ObjectPool.Instance.Get��
            //GameObject enemy = Instantiate(enemyPre, transform);
            //int index = Random.Range(0, nullPointsList.Count);
            //enemy.transform.position = nullPointsList[index];
            //nullPointsList.RemoveAt(index);

            int index = Random.Range(0, nullPointsList.Count);
            GameObject enemy =  ObjectPool.Instance.Get(ObjectType.Enemy, nullPointsList[index]);
            enemy.GetComponent<EnemyAi>().Init();
            nullPointsList.RemoveAt(index);

            if (poolObjectDic.ContainsKey(ObjectType.Enemy) == false)
                //�������򴴽�
                poolObjectDic.Add(ObjectType.Enemy, new List<GameObject>());
            poolObjectDic[ObjectType.Enemy].Add(enemy);//���
        }
        
    } 
}
