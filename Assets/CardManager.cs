using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }
    public Transform cardRoot; // 拖卡牌父节点
    public Canvas canvas;
    public RectTransform playedCardAnchor;
    public Transform playedCardsParent;
    

    // 所有场上活牌，key为instanceId
    public Dictionary<int, CardLogic> allDict = new Dictionary<int, CardLogic>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // 收集场景下所有CardLogic
        var allCards = cardRoot.GetComponentsInChildren<CardLogic>(true); // true=包括不激活的
        allDict.Clear();

        // 获取画布大小
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 canvasSize = canvasRect.sizeDelta;

        float xScatter = 60f;   // 横向偏移范围
        float yPad = 150f;      // 下方距离边界的补偿
        float interval = 0.08f; // 入场错开时间
        int idx = 0;

        foreach (var logic in allCards)
        {
            logic.view = logic.GetComponent<CardView>();
            if (logic.view != null)
                logic.view.logic = logic;
            allDict[logic.instanceId] = logic;
            //飞入：目标位置、起始位置确定
            RectTransform rt = logic.view.rectTransform;
            // 正常排布就是 anchoredPosition
            Vector2 targetLocalPos = rt.anchoredPosition;
            float xOff = Random.Range(-xScatter, xScatter);
            // 从下方飞入：起始位置 = 卡片的目标位置 + (水平随机偏移, 从屏幕底部再向下的固定偏移)
            Vector2 fromLocal = targetLocalPos + new Vector2(xOff, -canvasSize.y / 2 - yPad);
            //触发卡牌入场飞行动画（从fromLocal飞到targetLocalPos，根据idx设置延迟达到依次飞入的效果）
            logic.view.FlyInFrom(fromLocal, targetLocalPos, idx * interval);
            //延迟设计
            idx++;
        }
    }

    // 基于UI重叠、层级的遮挡自动判断
    public bool IsCardFree(int instanceId)
    {
        if (!allDict.TryGetValue(instanceId, out var selfCard)) return false;
        var selfRt = selfCard.GetComponent<RectTransform>();
        int selfIdx = selfRt.GetSiblingIndex();

        foreach (var other in allDict.Values)
        {
            if (other == selfCard) continue;
            if (other.view == null || other.view.IsPlayed()) continue;

            var otherRt = other.GetComponent<RectTransform>();
            int otherIdx = otherRt.GetSiblingIndex();

            // 在自己Hierarchy更上方 && 与自己重叠
            if (otherIdx > selfIdx && RectsOverlap(selfRt, otherRt))
                return false;
        }
        return true; // 没被任何挡，free!
    }

    // 判断两个UI世界Rect是否重叠
    bool RectsOverlap(RectTransform rt1, RectTransform rt2)
    {
        Rect r1 = GetWorldRect(rt1);
        Rect r2 = GetWorldRect(rt2);
        return r1.Overlaps(r2);
    }

    // RectTransform的世界坐标Rect
    Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        Vector2 min = corners[0];
        Vector2 size = corners[2] - corners[0];
        return new Rect(min, size);
    }

    public void CardClicked(CardLogic logic)
    {
        GameManager.Instance.OnCardPileClick();

        Canvas canvas = logic.view.GetComponentInParent<Canvas>();

        // 播放目标是playedCardAnchor，用它的position求全局坐标
        Vector3 worldTargetPos = playedCardAnchor.position;

        // 要获得“当前Parent坐标系下”该点的local位置
        Vector2 localTargetPos;
        RectTransform parentRect = logic.view.rectTransform.parent as RectTransform;

        // 先转屏幕坐标
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, worldTargetPos);

        // 再转到 parent 的局部空间
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            screenPoint,
            canvas.worldCamera,
            out localTargetPos
        );

        //记录Undo数据
        UndoManager.UndoData data = new UndoManager.UndoData
        {
            cardLogic = logic,
            cardView = logic.view,
            origParent = logic.view.rectTransform.parent,
            origSiblingIndex = logic.view.rectTransform.GetSiblingIndex(),
            origAnchoredPos = logic.view.rectTransform.anchoredPosition,
            origIsFront = logic.view.IsFront(),
            origIsPlayed = logic.view.IsPlayed(),
            fromZone = UndoManager.ZoneType.CardPile,
            affectedFaces = new List<UndoManager.AffectedFaceData>()
        };
        UndoManager.Instance.RecordPlay(data);

        // 播放动画到该局部点
        logic.view.FlipAndFlyTo(localTargetPos, () =>
        {
            // 动画结束再入目标堆
            PlayedAreaManager.Instance.AddCard(logic.transform); // 记得此处SetParent
            logic.view.rectTransform.anchoredPosition = Vector2.zero;
            PlayedAreaManager.Instance.RefreshOrder();

            // 【关键：胜负判定】
            GameEndManager.Instance.CheckWinOrLose();
        });
        //看看Target区域是否有可点击却未被点击的牌，发出提醒
        TargetPileManager.Instance.HintPlayableTargetCards();
    }
}
