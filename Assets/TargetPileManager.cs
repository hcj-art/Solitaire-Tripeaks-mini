using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetPileManager : MonoBehaviour
{
    public static TargetPileManager Instance { get; private set; }
    public Transform cardRoot; // 拖场上牌的父节点
    public Canvas canvas;// 拖进你根Canvas
    public RectTransform playedCardAnchor;// 拖进你目标锚点（PlayedArea下的锚空物体RectTransform）
    public Dictionary<int, TargetCardLogic> allDict = new Dictionary<int, TargetCardLogic>();// 现场所有在牌堆里的牌，key为instanceId

    //对于卡牌自动翻面，用所用动画判断结束后再进行翻面逻辑判断
    private int animatingCount = 0;
    public void OnCardAnimationStart()
    {
        animatingCount++;
        // Debug.Log("动画开始，当前animatingCount=" + animatingCount);
    }
    public void OnCardAnimationEnd()
    {
        animatingCount--;
        if (animatingCount < 0) animatingCount = 0; // 容错
        // Debug.Log("动画结束，当前animatingCount=" + animatingCount);
        if (animatingCount == 0)
        {
            TryFlipAllCanFlipCards();
        }
    }
    public void TryFlipAllCanFlipCards()
    {
        foreach (var kvp in allDict)
        {
            var logic = kvp.Value;
            if (logic == null || logic.view == null || logic.view.IsPlayed()) continue;
            if (!IsCardFree(logic.instanceId)) continue;
            if (!logic.view.IsFront() && !logic.view.IsAnimating())
            {
                logic.view.FlipToFront();
            }
        }
    }
    //上面是翻转判断逻辑实现
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // 初始化收集
        var allCards = cardRoot.GetComponentsInChildren<TargetCardLogic>(true);
        allDict.Clear();

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 canvasSize = canvasRect.sizeDelta;

        int idx = 0;
        float xScatter = 140f; // "横向发散"范围自行调整
        float yPad = 300f;     // 飞入时比屏幕还靠上多少，避免刚好在边界
        float interval = 0.07f; // 牌飞入的间隔

        foreach (var logic in allCards)
        {
            logic.view = logic.GetComponent<TargetCardView>();
            if (logic.view != null)
                logic.view.logic = logic;
            allDict[logic.instanceId] = logic;
            logic.view.onCardClicked = (view) => OnCardClicked(logic);

            // 【核心部分】算飞入起点（屏幕中心上方y，有一定x方向随机），目标点就是现在anchoredPosition
            RectTransform rt = logic.view.rectTransform;
            Vector2 toLocal = rt.anchoredPosition; // 目标位置(铺好)
            float xOffset = Random.Range(-xScatter, xScatter);
            Vector2 fromLocal = toLocal + new Vector2(xOffset, canvasSize.y / 2 + yPad);

            // 播放"飞入"动画
            logic.view.PlayEnterAnim(fromLocal, toLocal, idx * interval);
            idx++;
        }
    }

    // 重叠/遮挡判定
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

            // Hierarchy更上方且发生覆盖
            if (otherIdx > selfIdx && RectsOverlap(selfRt, otherRt))
                return false;
        }
        return true;
    }

    bool RectsOverlap(RectTransform rt1, RectTransform rt2)
    {
        Rect r1 = GetWorldRect(rt1);
        Rect r2 = GetWorldRect(rt2);
        return r1.Overlaps(r2);
    }

    //矩形遮挡检测
    Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        float minX = corners[0].x, minY = corners[0].y;
        float maxX = corners[0].x, maxY = corners[0].y;
        for (int i = 1; i < 4; ++i)
        {
            if (corners[i].x < minX) minX = corners[i].x;
            if (corners[i].y < minY) minY = corners[i].y;
            if (corners[i].x > maxX) maxX = corners[i].x;
            if (corners[i].y > maxY) maxY = corners[i].y;
        }
        // 可选的小浮点容错
        float pad = 0.5f;
        minX -= pad; minY -= pad; maxX += pad; maxY += pad;
        return new Rect(new Vector2(minX, minY), new Vector2(maxX - minX, maxY - minY));
    }
    public Transform playedCardsParent; // Inspector面板拖进目标区

    //判断匹配逻辑
    public bool IsCardNumberCanMatch(int cardidate, int played)
    {
        if (cardidate == 0 || played == 0)
            return true;
        return Mathf.Abs(cardidate - played) == 1 ||
            (cardidate == 1 && played == 13) ||
            (cardidate == 13 && played == 1);
    }

    void OnCardClicked(TargetCardLogic logic)
    {
        if (!IsCardFree(logic.instanceId)) return;

        // 1. 获取 PlayedArea 顶牌
        int playedCardNumber = PlayedAreaManager.Instance.GetTopCardNumber(); // 方法在Area管理器中
        if (!IsCardNumberCanMatch(logic.cardNumber, playedCardNumber))
        {
            // 不满足差1或A-K条件，非法点击
            Debug.Log("只能选择与出牌区数字差1（或A↔K）的牌！");
            if (logic.view != null) logic.view.PlayShake();
            return;
        }

        // 1. 计算目标anchor（playedCardAnchor）在【当前parent】坐标空间下的localPoint
        RectTransform cardParent = logic.view.rectTransform.parent as RectTransform;
        Vector3 worldTargetPos = playedCardAnchor.position;

        // 转换：目标锚点的世界位置 → 当前手牌parent坐标
        Vector2 anchorScreenPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, worldTargetPos);

        Vector2 flyTargetLocal;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            cardParent,    // 动画前的parent，极为关键
            anchorScreenPos,
            canvas.worldCamera,
            out flyTargetLocal
        );

        // 2. 动画到目标local点，动画期间parent不能动
        logic.view.RotateAndFlyTo(flyTargetLocal, () =>
        {
            // 3. 动画完成后，移动到目标parent，并anchoredPosition归零
            logic.view.rectTransform.SetParent(playedCardsParent, false);
            logic.view.rectTransform.anchoredPosition = Vector2.zero;
            PlayedAreaManager.Instance.AddCard(logic.transform);
            PlayedAreaManager.Instance.RefreshOrder();
        });
    }
}