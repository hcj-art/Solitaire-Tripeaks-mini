using UnityEngine;
using DG.Tweening;

public class Card6EnterView : MonoBehaviour
{
    public Transform enterStartPoint; // 拖一个“动画始发点”transform进来
    public Transform enterTargetPoint; // 拖牌目标点（就是常驻的那点）

    void Start()
    {
        // 入场前先放起点
        transform.position = enterStartPoint.position;

        // 立刻执行入场动画
        transform.DOMove(enterTargetPoint.position, 0.5f)
            .SetEase(Ease.OutBack);
    }
}
