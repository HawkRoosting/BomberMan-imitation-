using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 

public enum ObjectType
{
    SuperWall,
    Wall,
    Prop,
    Enemy,
    Bomb,
    BombEffect,
    DestroyWallEffect
}

[System.Serializable]
public class Type_Prefab
{
    public ObjectType type;
    public GameObject prefab;
}

public class ObjectPool : MonoBehaviour
{
    //将对象池写成一个单类，方便调用
    public static ObjectPool Instance;

    public List<Type_Prefab> type_Prefabs = new List<Type_Prefab>();

    /// <summary>
    /// 物体类型和对应的对象池关系字典
    /// </summary>
    private Dictionary<ObjectType, List<GameObject>> dic = new Dictionary<ObjectType, List<GameObject>>();

    private void Awake()
    {
        Instance = this;
    }


    /// <summary>
    /// 通过物体类型获取该预制体
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private GameObject GetPreByType(ObjectType type)
    {
        foreach(var item in type_Prefabs)
        {
            if (item.type == type)
                return item.prefab;
        }
        return null;
    }

 

    /// <summary>
    /// 通过物体类型从相应的对象池中获取物体
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public GameObject Get(ObjectType type, Vector2 pos)
    {
        GameObject temp = null;
        //判断是否有与该类型想匹配的对象池没有则创建 
        if (dic.ContainsKey(type) == false)
            dic.Add(type, new List<GameObject>());
        //判断对象池中有无物体
        if (dic[type].Count > 0)
        {
            int index = dic[type].Count - 1; //下标-1
            temp = dic[type][index];
            dic[type].RemoveAt(index);//移除被取出的物体
        }
        else
        {
            GameObject pre = GetPreByType(type);
            if (pre != null)
            {
                temp = Instantiate(pre, transform);
            }
        }
        //物体从对象池取出后进行显示
        temp.SetActive(true);
        //直接在此修改物体位置不用在之前的MapController里修改
        temp.transform.position = pos;
        temp.transform.rotation = Quaternion.identity;
        return temp;
    }

    /// <summary>
    /// 关卡结束后回收资源
    /// </summary>
    /// <param name="type"></param>
    public void Add(ObjectType type, GameObject go)
    {
        //判断该类型是否具有对应的对象池以及对象池中不存在该物体（确保其他地方的回收不会重复回收）
        if (dic.ContainsKey(type) && dic[type].Contains(go) == false)
        {
            //放入对象池
            dic[type].Add(go); 
        }
        go.SetActive(false);//存入对象池后隐藏物体 
    }


}

