using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UISettingPanel : MonoBehaviour
{
    public GameObject panelRoot;
    public CanvasGroup panelCg;//渐变效果
    public Transform windowRoot;

    private void Awake()
    {
        //初始禁用
        panelRoot.SetActive(false);
    }

    //显示设置面板
    public void ShowPanel()
    {
        panelRoot.SetActive(true);
        //动画初始状态
        panelCg.alpha = 0;
        windowRoot.localScale = Vector3.one * 0.7f;
        //顺序动画：遮罩淡入 + 窗口缩放弹出
        DOTween.Kill(windowRoot);
        DOTween.Kill(panelCg);

        panelCg.DOFade(1, 0.2f);
        windowRoot.DOScale(1, 0.25f).SetEase(Ease.OutBack);

    }

    //隐藏面板
    public void HidePanel()
    {
        //缩小淡出
        windowRoot.DOScale(0.7f, 0.2f)
            .SetEase(Ease.InBack);
        panelCg.DOFade(0, 0.2f)
            .OnComplete(() =>
            {
                panelRoot.SetActive(false);
            });
    }
}
