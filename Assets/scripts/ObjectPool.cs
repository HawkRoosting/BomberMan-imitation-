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
    //�������д��һ�����࣬�������
    public static ObjectPool Instance;

    public List<Type_Prefab> type_Prefabs = new List<Type_Prefab>();

    /// <summary>
    /// �������ͺͶ�Ӧ�Ķ���ع�ϵ�ֵ�
    /// </summary>
    private Dictionary<ObjectType, List<GameObject>> dic = new Dictionary<ObjectType, List<GameObject>>();

    private void Awake()
    {
        Instance = this;
    }


    /// <summary>
    /// ͨ���������ͻ�ȡ��Ԥ����
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
    /// ͨ���������ʹ���Ӧ�Ķ�����л�ȡ����
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public GameObject Get(ObjectType type, Vector2 pos)
    {
        GameObject temp = null;
        //�ж��Ƿ������������ƥ��Ķ����û���򴴽� 
        if (dic.ContainsKey(type) == false)
            dic.Add(type, new List<GameObject>());
        //�ж϶��������������
        if (dic[type].Count > 0)
        {
            int index = dic[type].Count - 1; //�±�-1
            temp = dic[type][index];
            dic[type].RemoveAt(index);//�Ƴ���ȡ��������
        }
        else
        {
            GameObject pre = GetPreByType(type);
            if (pre != null)
            {
                temp = Instantiate(pre, transform);
            }
        }
        //����Ӷ����ȡ���������ʾ
        temp.SetActive(true);
        //ֱ���ڴ��޸�����λ�ò�����֮ǰ��MapController���޸�
        temp.transform.position = pos;
        temp.transform.rotation = Quaternion.identity;
        return temp;
    }

    /// <summary>
    /// �ؿ������������Դ
    /// </summary>
    /// <param name="type"></param>
    public void Add(ObjectType type, GameObject go)
    {
        //�жϸ������Ƿ���ж�Ӧ�Ķ�����Լ�������в����ڸ����壨ȷ�������ط��Ļ��ղ����ظ����գ�
        if (dic.ContainsKey(type) && dic[type].Contains(go) == false)
        {
            //��������
            dic[type].Add(go); 
        }
        go.SetActive(false);//�������غ��������� 
    }


}

