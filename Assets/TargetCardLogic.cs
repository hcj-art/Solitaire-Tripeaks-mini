using UnityEngine;

public class TargetCardLogic : MonoBehaviour
{
    public int cardNumber;     // 牌点数
    public int instanceId; // 运行时唯一标识符
    public TargetCardView view; // 关联的显示层
    public bool isSelected;    // 逻辑属性

    private void Awake()
    {
        if (view == null)
            view = GetComponent<TargetCardView>();
        if (view != null && view.logic != this)
            view.logic = this;
        instanceId = GetUniqueInstanceId();
    }

    // 可选静态唯一ID分配
    private static int globalInstanceID = 1;
    private int GetUniqueInstanceId()
    {
        return globalInstanceID++;
    }
}
