using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardLogic : MonoBehaviour, IPointerClickHandler
{
    public int cardNumber;
    [Tooltip("必须与Json中的id保持一致")]
    public int instanceId; // Inspector唯一编号
    public CardView view;

    // 用属性，动态判断“自由牌”
    public bool IsFree => CardManager.Instance.IsCardFree(instanceId);

    void Awake()
    {
        if (view == null)
            view = GetComponent<CardView>();
        if (view != null && view.logic != this)
            view.logic = this;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (view == null)
        {
            Debug.LogError($"[{gameObject.name}] CardLogic.view为null!");
            return;
        }
        if (IsFree && !view.IsPlayed())
            CardManager.Instance.CardClicked(this);
        else
            view.PlayShake();
    }
}
