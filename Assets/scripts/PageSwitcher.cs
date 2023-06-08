using UnityEngine;
using UnityEngine.UI;

public class PageSwitcher : MonoBehaviour
{
    public GameObject currentPage; // 当前页面的GameObject
    public GameObject nextPage; // 下一个页面的GameObject

    private bool isPageActive = true; // 页面的激活状态

    private void Start()
    {
        // 确保下一个页面开始时处于隐藏状态
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
        currentPage.SetActive(false); // 隐藏当前页面
        nextPage.SetActive(true); // 激活下一个页面
        isPageActive = false; // 禁用页面切换
    }
}
