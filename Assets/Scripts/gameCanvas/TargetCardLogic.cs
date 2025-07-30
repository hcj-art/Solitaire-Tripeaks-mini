using UnityEngine;

public class TargetCardLogic : MonoBehaviour
{
    public int cardNumber;     // 牌点数
    public int instanceId; // 运行时唯一标识符
    public TargetCardView view; // 关联的显示层
    

    //区分目标区Wild和功能区Wild，作为判断标识
    public bool isWild;
    public bool isTargetWild;

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
