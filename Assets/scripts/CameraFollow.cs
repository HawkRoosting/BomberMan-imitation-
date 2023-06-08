using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //复制一份player
    private Transform player;
    private int X, Y;  

    public void Init(Transform player, int x, int y)
    {
        this.player = player;
        X = x;
        Y = y;
    }

    //lateupdate:player移动结束后再执行
    private void LateUpdate()
    {
        if (player != null)
        {
            float x = Mathf.Lerp(transform.position.x, player.position.x, 0.2f);
            float y = Mathf.Lerp(transform.position.y, player.position.y, 0.2f);
            transform.position = new Vector3(x, y, transform.position.z);

            //摄像机跟随范围：该范围根据x和y值变化而改变
            // 6 3(x -1 -1)(y -l -1)  整体不变
            //8 5 (x -2 0)(y -3 1)  变化
            //10 7(x -4 2)(y -5 3)  变化
            //clamp: value值夹在min和max之间---
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -(X - 5), X - 7),
                Mathf.Clamp(transform.position.y, -(Y - 2), Y - 4), 
                transform.position.z);

        }
    }
}
