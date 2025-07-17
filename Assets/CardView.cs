using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardView : MonoBehaviour, IPointerClickHandler
{

    public CardLogic logic;
    public GameObject front;
    public GameObject back;

    private bool isFront = false;
    private bool isAnimating = false;
    public float flipDuration = 0.25f;
    public float flyDuration = 0.5f;
    public float scaleSize = 1.2f;
    private bool hasPlayed = false;
    public bool IsPlayed() => hasPlayed;
    public RectTransform rectTransform;

    void Awake()
    {
        if (logic == null)
            logic = GetComponent<CardLogic>();
        if (logic != null && logic.view != this)
            logic.view = this;
    }
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        ShowBack();
        rectTransform.localRotation = Quaternion.identity;
    }

    //入场飞行动画（起始位置，目标位置，动画开始延迟时间：开始设置为0）
    public void FlyInFrom(Vector2 fromLocal, Vector2 toLocal, float delay = 0f)
    {
        if (isAnimating) return;
        isAnimating = true;
        rectTransform.anchoredPosition = fromLocal;//先将卡牌放在起始位置
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.localScale = Vector3.one;//（1，1，1）
        rectTransform.SetAsLastSibling(); // 确保动画遮挡正确，显示在最上层

        // 动画
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(delay); // 错开更自然，延迟飞入
        seq.Append(rectTransform.DOAnchorPos(toLocal, 0.5f).SetEase(Ease.OutQuad));//0.5s移动至该位置
        seq.Join(rectTransform.DOScale(scaleSize, 0.25f).SetLoops(2, LoopType.Yoyo)); // 同步进行放缩动画
        seq.AppendCallback(() =>
        {
            isAnimating = false;
        });
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        logic?.OnPointerClick(eventData); // 让CardLogic处理
    }

    public void FlipAndFlyTo(Vector2 targetLocalPosition, System.Action onComplete = null)
    {
        if (isAnimating || hasPlayed) return;
        isAnimating = true;
        transform.SetAsLastSibling();
        Sequence seq = DOTween.Sequence();//创建动画空队列
        seq.Append(rectTransform.DOLocalRotate(new Vector3(0, 90, 0), flipDuration));
        seq.AppendCallback(() => ShowFront());
        seq.Append(rectTransform.DOLocalRotate(Vector3.zero, flipDuration));
        seq.Join(rectTransform.DOAnchorPos(targetLocalPosition, flyDuration));
        seq.Join(rectTransform.DOScale(scaleSize, flyDuration));
        seq.Append(rectTransform.DOScale(1f, 0.2f));
        seq.AppendCallback(() =>
        {
            isAnimating = false;
            hasPlayed = true;
            onComplete?.Invoke();
        });
    }

    public void PlayShake() => rectTransform.DOShakeScale(0.2f, 0.2f);

    private void ShowBack()
    {
        isFront = false;
        front.SetActive(false);
        back.SetActive(true);
    }

    private void ShowFront()
    {
        isFront = true;
        front.SetActive(true);
        back.SetActive(false);
    }
    public bool IsFront()
    {
        return isFront;
    }

    // 新增撤销功能（SetFrontState，撤销会用到）
    public void SetFrontState(bool isFront)
    {
        this.isFront = isFront;

        if (isFront)
            ShowFront();
        else
            ShowBack();
    }
    // 用于撤销和判定恢复已出牌状态
    public void SetPlayedState(bool played)
    {
        hasPlayed = played;
    }
}