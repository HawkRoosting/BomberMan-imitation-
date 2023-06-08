using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //����һ��player
    private Transform player;
    private int X, Y;  

    public void Init(Transform player, int x, int y)
    {
        this.player = player;
        X = x;
        Y = y;
    }

    //lateupdate:player�ƶ���������ִ��
    private void LateUpdate()
    {
        if (player != null)
        {
            float x = Mathf.Lerp(transform.position.x, player.position.x, 0.2f);
            float y = Mathf.Lerp(transform.position.y, player.position.y, 0.2f);
            transform.position = new Vector3(x, y, transform.position.z);

            //��������淶Χ���÷�Χ����x��yֵ�仯���ı�
            // 6 3(x -1 -1)(y -l -1)  ���岻��
            //8 5 (x -2 0)(y -3 1)  �仯
            //10 7(x -4 2)(y -5 3)  �仯
            //clamp: valueֵ����min��max֮��---
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -(X - 5), X - 7),
                Mathf.Clamp(transform.position.y, -(Y - 2), Y - 4), 
                transform.position.z);

        }
    }
}
