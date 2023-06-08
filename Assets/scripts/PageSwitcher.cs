using UnityEngine;
using UnityEngine.UI;

public class PageSwitcher : MonoBehaviour
{
    public GameObject currentPage; // ��ǰҳ���GameObject
    public GameObject nextPage; // ��һ��ҳ���GameObject

    private bool isPageActive = true; // ҳ��ļ���״̬

    private void Start()
    {
        // ȷ����һ��ҳ�濪ʼʱ��������״̬
        nextPage.SetActive(false);
    }

    private void Update()
    {
        if (isPageActive && Input.anyKeyDown)
        {
            SwitchPage();
        }
    }

    private void SwitchPage()
    {
        currentPage.SetActive(false); // ���ص�ǰҳ��
        nextPage.SetActive(true); // ������һ��ҳ��
        isPageActive = false; // ����ҳ���л�
    }
}
