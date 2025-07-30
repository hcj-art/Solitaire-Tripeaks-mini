using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayedAreaManager : MonoBehaviour
{
    void Start()
    {
        // 清空旧--
        playedCardList.Clear();
        // 收集当前所有子对象
        foreach (Transform child in transform)
        {
            playedCardList.Add(child);
        }
        RefreshOrder();
    }
    public static PlayedAreaManager Instance { get; private set; }
    void Awake() { Instance = this; }
    private List<Transform> playedCardList = new List<Transform>();

    public void AddCard(Transform card)
    {
        // 先确保父物体是PlayedArea
        card.SetParent(transform, false);
        playedCardList.Add(card);
        RefreshOrder(); // 确保每加一个刷新一次
    }

    public void RemoveCard(Transform card)
    {
        playedCardList.Remove(card);
        RefreshOrder();
    }

    //统一刷新层级，谁先加入谁在下，最后加的在最上
    public void RefreshOrder()
    {
        for (int i = 0; i < playedCardList.Count; i++)
        {
            playedCardList[i].SetSiblingIndex(i);
        }
    }
    public int GetTopCardNumber()
    {
        if (playedCardList.Count == 0)
        return -1;
        var topCard = playedCardList[playedCardList.Count - 1];
        var cardLogic = topCard.GetComponent<CardLogic>();
        if(cardLogic != null) return cardLogic.cardNumber;
        var targetLogic = topCard.GetComponent<TargetCardLogic>();
        if(targetLogic != null) return targetLogic.cardNumber;
        return -1;
    }
}
