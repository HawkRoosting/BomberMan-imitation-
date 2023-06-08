using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bomb : MonoBehaviour
{
    public GameObject boomEffect;
    private int range;
    //ը������������Ϻ��ί�У�����ը����������
    private Action aniFinAction;

    public void Init(int range, float dealyTime, Action action)//��Χ ���ӳٱ�ըʱ��,  actionΪһ��ί�У��ѷ��������������д��ݣ�
    {
        this.range = range;//��ȫ�ֱ������ֿ�����this��ʾȫ�ֱ���range��
        StartCoroutine("DealyBoom", dealyTime);  //����Э��
        aniFinAction = action;
    }

    //IEnumerator DealyBoom(float time)
    //{
    //    yield return new WaitForSeconds(time);
    //    if (aniFinAction != null)
    //    aniFinAction();
    //    AudioController.Instance.PlayBoom();
    //    ObjectPool.Instance.Get(ObjectType.BombEffect, transform.position);
    //    //Instantiate(boomEffect, transform.position, Quaternion.identity);
    //    Boom(Vector2.left);
    //    Boom(Vector2.right);
    //    Boom(Vector2.up);
    //    Boom(Vector2.down);
    //    Destroy(gameObject);

    //    ObjectPool.Instance.Add(ObjectType.Bomb, gameObject);
    //}



    /// <summary>
    /// ��Ч����
    /// </summary>
    /// <param name="dir"></param>
    //private void Boom(Vector2 dir)
    //{
    //    for (int i = 1; i <= range; i++)
    //    {

    //        Vector2 pos = (Vector2)transform.position + dir * i;
    //        if (GameController.Instance.IsSuperWall(pos)) 
    //            break;
    //        ObjectPool.Instance.Get(ObjectType.BombEffect, pos);
    //        //GameObject effect = Instantiate(boomEffect);
    //        //effect.transform.position = pos;
    //    }
    //}

    //��Ч�Ľ�---ʹ�þ�����Ⱦ��
    [Header("Explosion")]
    public Explosion explosionPrefab;
    //public LayerMask explosionLayerMask;
    public float explosionDuration = 1f;
    IEnumerator DealyBoom(float time)
    {
        Vector2 position = transform.position;
        position.y = Mathf.Round(position.y);
        position.x = Mathf.Round(position.x);

        yield return new WaitForSeconds(time);
        if (aniFinAction != null) aniFinAction();
        AudioController.Instance.PlayBoom();

        Explosion explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
        explosion.SetActiveRenderer(explosion.start);
        
        //ObjectPool.Instance.Get(ObjectType.BombEffect, transform.position);

        Boom(position, Vector2.left, range);
        Boom(position, Vector2.right, range);
        Boom(position, Vector2.down, range);
        Boom(position, Vector2.up, range);
        explosion.DestroyAfter(explosionDuration);
        Collider2D collider = gameObject.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
        ObjectPool.Instance.Add(ObjectType.Bomb, gameObject);

    }

    private void Boom(Vector2 position, Vector2 dir, int length)
    {
        if (length <= 0)
        {
            return;
        }
        position += dir;
        if (GameController.Instance.IsSuperWall(position))
        {
            return;
        }

        Explosion explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
        explosion.SetActiveRenderer(length > 1 ? explosion.middle : explosion.end);
        explosion.SetDirection(dir);
        explosion.DestroyAfter(explosionDuration);
        //�ݹ�����
        Boom(position, dir, length - 1);
    }


        //�������߱��ڼ����ӻ�
    private float rayDistance = 0.5f;
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, rayDistance, 0));
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -rayDistance, 0));
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(rayDistance, 0, 0));
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(-rayDistance, 0, 0));
    }
}
 