using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoManager : MonoBehaviour
{
    public static UndoManager Instance { get; private set; }
    private Stack<UndoData> undoStack = new Stack<UndoData>();
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    //数据结构定义：枚举=>定义一组固定命名常量
    public enum ZoneType
    {
        CardPile,
        TargetPile
    }
    //每次出牌记录
    public class UndoData
    {
        public MonoBehaviour cardLogic;//即可能装CardLogic，也可能装TargetCardlogic
        public MonoBehaviour cardView;//即可能装Cardview,也可能装TargetCardView
        public Transform origParent;//飞出去前的parent
        public int origSiblingIndex;//飞出去前在parent的层级
        public Vector2 origAnchoredPos;//飞出去前的局部位置
        public bool origIsFront;//用于恢复翻转状态
        public bool origIsPlayed;//是否玩过
        public ZoneType fromZone;//来源于哪个区域

        //被影响的其他牌的正反面记录
        public List<AffectedFaceData> affectedFaces = new List<AffectedFaceData>();
    }

    //被你打出当前牌后，自动被翻面的其他牌
    public class AffectedFaceData
    {
        public MonoBehaviour logic;
        public bool wasFront;
    }

    //调用点，每打出一张牌前调用，入栈
    public void RecordPlay(UndoData data)
    {
        undoStack.Push(data);
    }

    public bool HasUndo => undoStack.Count > 0;
    //调用点，点击撤销时调用
    public void UndoLastMove()
    {
        if (!HasUndo) return;
        var data = undoStack.Pop();

        //从出牌区移除
        PlayedAreaManager.Instance.RemoveCard(((MonoBehaviour)data.cardLogic).transform);

        //放回原区域
        Transform cardTrans = ((MonoBehaviour)data.cardLogic).transform;
        cardTrans.SetParent(data.origParent, false);

        //恢复父对象以及层级
        cardTrans.SetSiblingIndex(data.origSiblingIndex);

        //恢复局部位置
        RectTransform rt = cardTrans.GetComponent<RectTransform>();
        rt.anchoredPosition = data.origAnchoredPos;

        //恢复正反面及已出牌（只通过 view）
        if (data.cardLogic is CardLogic cl)
        {
            cl.view.SetFrontState(data.origIsFront);
            cl.view.SetPlayedState(data.origIsPlayed);
            CardManager.Instance.allDict[cl.instanceId] = cl;
        }
        else if (data.cardLogic is TargetCardLogic tcl)
        {
            tcl.view.SetFrontState(data.origIsFront);
            tcl.view.SetPlayedState(data.origIsPlayed);
            TargetPileManager.Instance.allDict[tcl.instanceId] = tcl;
        }

        //只用 SetFrontState 恢复正反面
        foreach (var aff in data.affectedFaces)
        {
            if (aff.logic is CardLogic ac)
            {
                ac.view.SetFrontState(aff.wasFront);
            }
            else if (aff.logic is TargetCardLogic atc)
            {
                atc.view.SetFrontState(aff.wasFront);
            }
        }

        //刷新字典层级
        if (data.fromZone == ZoneType.CardPile)
        {
            CardManager.Instance.allDict[((CardLogic)data.cardLogic).instanceId] = (CardLogic)data.cardLogic;
        }
        else if (data.fromZone == ZoneType.TargetPile)
        {
            //把被撤销操作的那张 TargetPile 区的牌（TargetCardLogic）放回到 TargetPileManager 的 allDict 字典里，索引是 instanceId。
            TargetPileManager.Instance.allDict[((TargetCardLogic)data.cardLogic).instanceId] = (TargetCardLogic)data.cardLogic;
            TargetPileManager.Instance.RefreshAllCardFaces();
        }
    }
}
