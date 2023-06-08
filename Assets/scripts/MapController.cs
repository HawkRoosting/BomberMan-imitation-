using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    //已经使用对象池处理物体了，不在需要这个引用
    //public GameObject superWallPre, wallPre, , propPre, enemyPre;//获取unity物体(在unity客服端拖入物体挂载)
    //doorPre数量只有一个不需要使用对象池
    public GameObject doorPre;
    private int X, Y;//地图中坐标
    private List<Vector2> nullPointsList = new List<Vector2>(); //空坐标点集合
    private List<Vector2> superWallPointList = new List<Vector2>(); //超级墙集合
    //存储门，不用每一关都进行生成
    private GameObject door;
    //从对象池中取出的所有物体集合（以下使用物体类型和对应的对象池关系字典）---用于管理回收
    private Dictionary<ObjectType, List<GameObject>> poolObjectDic = new Dictionary<ObjectType, List<GameObject>>();
    //private List<GameObject> superWallList = new List<GameObject>();
    //private List<GameObject> wallList = new List<GameObject>();
    //private List<GameObject> propList = new List<GameObject>();
    //private List<GameObject> enemyList = new List<GameObject>();




    //Awake与start方法都是unity启动时直接调用的方法
    //private void Awake()
    //{
    //    InitMap(7, 3, 50);

    //}

    //初始化地图
    public void InitMap(int x, int y, int wallCount, int enemyCount)
    {
        //初始化之前回收上一关卡资源
        Recovery();
        X = x;
        Y = y;
        CreatSuperWall();
        FindNullPoints();
        Debug.Log(superWallPointList.Count); //测试
        CreateWall(wallCount);
        CreateDoor();
        CreatProps();
        CreatEnemy(enemyCount);
    }
     
    private void Recovery()
    {
        //生成下一关卡时对集合进行清空
        nullPointsList.Clear();
        superWallPointList.Clear();
        //遍历GameController下子物体并删除物体：关卡的生成需要对上一关生成的物体进行删除
        //foreach (Transform  item in transform)
        //{
        //    Destroy(item.gameObject);
        //}
        //以上需要删除的物体存入对象池
        foreach (var item in poolObjectDic)
        {
            foreach (var obj in item.Value) 
            {
                ObjectPool.Instance.Add(item.Key, obj);
            }
        }
        poolObjectDic.Clear();
    }

    //生成实体墙
    private void CreatSuperWall()   
    {
        //内层实体墙
        //XY方向间隔为1恰好挨着
        for (int x = -X; x < X; x+=2) //中间有空格
        {
            for (int y = -Y; y < Y; y+=2)
            {
                SpawnSuperWall(new Vector2(x, y));
                ////初始化物体
                //GameObject wall = Instantiate(superWallPre, transform);  
                ////修改坐标
                //wall.transform.position=new Vector3(x, y);//y如果默认不传则为0 //vector3矢量位置
            }

        }
        //外层实体墙(上下层)
        for (int x = -(X + 2); x <= X; x++)
        {
            SpawnSuperWall(new Vector2(x, Y));
            SpawnSuperWall(new Vector2(x, -(Y + 2)));
            ////初始化物体
            //GameObject wall = Instantiate(superWallPre, transform);
            ////修改坐标
            //wall.transform.position = new Vector3(x, Y);//y如果默认不传则为0 //vector3矢量位置

            //GameObject wall1 = Instantiate(superWallPre, transform);
            //wall1.transform.position = new Vector3(x, -Y - 2);
        }

        //左右层
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
    //代码重复部分太多将初始化物体以及坐标部分封装一下
    private void SpawnSuperWall(Vector2 pos) //参数为坐标
    {

        //GameObject wall1 = Instantiate(superWallPre, transform);
        superWallPointList.Add(pos);
        GameObject superWall =  ObjectPool.Instance.Get(ObjectType.SuperWall, pos);
        //回收
        if (poolObjectDic.ContainsKey(ObjectType.SuperWall) == false)//不存在则创建
            poolObjectDic.Add(ObjectType.SuperWall, new List<GameObject>());
        poolObjectDic[ObjectType.SuperWall].Add(superWall);//添加
    }

    /// <summary>
    /// 判断当前位置是否是实体墙
    /// </summary>
    /// <returns></returns>
     public bool IsSuperWall(Vector2 pos)
    {
        if (superWallPointList.Contains(pos))
            return true;
        return false;
    }

    //可摧毁的墙: 提取实体墙空隙部分position，从中随机取值
    //提取所有空的地方点位
    private void FindNullPoints()
    {
        //Unity中分析X与Y范围
        for (int x = -(X+1); x <= X-1; x++)
        {
            if (-(X + 1) % 2 == x % 2)//判断该行是否存在实体墙（-(X+1)为初始位置根据奇偶判断）
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
        //移除左上角三个位置，该位置设置为人物初始生成位置及可移动范围
        nullPointsList.Remove(new Vector2(-(X + 1), Y - 1));
        nullPointsList.Remove(new Vector2(-X, Y - 1));
        nullPointsList.Remove(new Vector2(-(X + 1), Y - 2));
    }

    /// <summary>
    /// 返回人物初始生成位置方便GameController脚本使用
    /// </summary> 
    /// <returns></returns>
    public Vector2 GetPlayerPos()
    {
        return new Vector2((-X - 1), Y - 1);
    }

    /// <summary>
    /// 创建可销毁的墙
    /// </summary>
    private void CreateWall(int wallCount) //形参为可摧毁墙数量
    {
        //算法的可行性,假设传过来的参数大于空点数，则改为空点位的百分之四十
        if (wallCount >= nullPointsList.Count)
        {
            wallCount = (int)(nullPointsList.Count * 0.4f);
        }
            

        //随机从空点集合中随机获取点位
        for (int i = 0; i < wallCount; i++)
        {
            int index = Random.Range(0, nullPointsList.Count); //左闭右开区间
            GameObject wall = ObjectPool.Instance.Get(ObjectType.Wall, nullPointsList[index]);
            //GameObject wall = Instantiate(wallPre, transform);
            //wall.transform.position = nullPointsList[index];
            //移除已经创建物体的空点
            nullPointsList.RemoveAt(index);
            //回收物体
            if (poolObjectDic.ContainsKey(ObjectType.Wall) == false)
                //不存在则创建
                poolObjectDic.Add(ObjectType.Wall, new List<GameObject>()); 
            poolObjectDic[ObjectType.Wall].Add(wall);//添加

        }
    }

    /// <summary>
    /// 创建通关门
    /// </summary>
    private void CreateDoor()
    {
        if (door == null)
            door = Instantiate(doorPre, transform);
        //GameObject door = Instantiate(doorPre, transform);
        //重置上一关卡的门特征
        door.GetComponent<Door>().ResetDoor();
        int index = Random.Range(0, nullPointsList.Count);
        door.transform.position = nullPointsList[index];
        nullPointsList.RemoveAt(index);//移除已经使用的空点位
    }
    /// <summary>
    /// 创建道具
    /// </summary>
    private void CreatProps()
    {
        //道具数量与当前空点数量有关
        int count = Random.Range(0, 2 + (int)(nullPointsList.Count * 0.05f)); //有时空点过少后面表达式会为0，所以+2保证基本有值  
        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, nullPointsList.Count);
            GameObject prop = ObjectPool.Instance.Get(ObjectType.Prop, nullPointsList[index]);
            //GameObject prop = Instantiate(propPre, transform);
            //prop.transform.position = nullPointsList[index];
            nullPointsList.RemoveAt(index);
            //回收
            if (poolObjectDic.ContainsKey(ObjectType.Prop) == false)
                //不存在则创建
                poolObjectDic.Add(ObjectType.Prop, new List<GameObject>());
            poolObjectDic[ObjectType.Prop].Add(prop);//添加
        }
    }
    /// <summary>
    /// 创建敌人
    /// </summary>
    private void CreatEnemy(int count)
    {
        for (int i = 0; i < count; i++)
        {
            //原生成方法，以下优化已经将位置修改封装进了 ObjectPool.Instance.Get中
            //GameObject enemy = Instantiate(enemyPre, transform);
            //int index = Random.Range(0, nullPointsList.Count);
            //enemy.transform.position = nullPointsList[index];
            //nullPointsList.RemoveAt(index);

            int index = Random.Range(0, nullPointsList.Count);
            GameObject enemy =  ObjectPool.Instance.Get(ObjectType.Enemy, nullPointsList[index]);
            enemy.GetComponent<EnemyAi>().Init();
            nullPointsList.RemoveAt(index);

            if (poolObjectDic.ContainsKey(ObjectType.Enemy) == false)
                //不存在则创建
                poolObjectDic.Add(ObjectType.Enemy, new List<GameObject>());
            poolObjectDic[ObjectType.Enemy].Add(enemy);//添加
        }
        
    } 
}
