using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Wall : MonoBehaviour
{
    private Animator anim;
    public GameObject DestroyWallEffect;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision) //使用该方法时该物体需要带有rigidbody组件
    {
        if (collision.CompareTag(Tags.BombEffect))
        {
            DestroyWallAni();
            //GameObject effct = Instantiate(DestroyWallEffect, transform);
            //effct.transform.position = new Vector2(-7, 2);
            //ObjectPool.Instance.Add(ObjectType.Wall, gameObject);
            //GameObject effct = Instantiate(DestroyWallEffect, transform);
            

            //ObjectPool.Instance.Get(ObjectType.BombEffect, pos);
        }
    }

    public void DestroyWallAni()
    {
        anim.SetTrigger("DestroyWall");
    }

    public void AniFin()
    {
        ObjectPool.Instance.Add(ObjectType.Wall, gameObject);
    }
}
