using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class TargetCardView : MonoBehaviour, IPointerClickHandler
{
    [Header("正面和背面节点")]
    public GameObject front;
    public GameObject back;
    [Header("动画参数")]
    public float rotateDuration = 0.5f;
    public float flyDuration = 1.2f;
    public float scaleSize = 1.2f;
    public float flipDuration = 0.3f;
    private bool isFront = false;
    public bool IsFront() => isFront;
    private bool isAnimating = false;
    public bool IsAnimating() => isAnimating;
    private bool hasPlayed = false;
    public bool IsPlayed() => hasPlayed;

    public RectTransform rectTransform;

    public System.Action<TargetCardView> onCardClicked; //  只走外部管理
    public TargetCardLogic logic;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        ShowBack();
        //rectTransform.localRotation = Quaternion.identity;
    }

    //入场飞行动画
    public void PlayEnterAnim(Vector2 fromLocalPos, Vector2 toLocalPos, float delay = 0f)
    {
        rectTransform.anchoredPosition = fromLocalPos;
        rectTransform.localScale = Vector3.one * 0.8f;
        rectTransform.localRotation = Quaternion.Euler(0, 0, Random.Range(-15f, 15f));
        hasPlayed = false;
        ShowBack();
        isAnimating = true;
        TargetPileManager.Instance.OnCardAnimationStart(); //<-动画开始登记

        Sequence seq = DOTween.Sequence();
        if (delay > 0f)
            seq.AppendInterval(delay);

        seq.Append(rectTransform.DOAnchorPos(toLocalPos, 0.55f).SetEase(Ease.OutQuart));
        seq.Join(rectTransform.DOScale(1.0f, 0.55f));
        seq.Join(rectTransform.DOLocalRotate(Vector3.zero, 0.38f));
        seq.OnComplete(() =>
        {
            isAnimating = false;
            TargetPileManager.Instance.OnCardAnimationEnd(); //<-动画结束登记
        });
    }


    public void FlipToFront(System.Action onComplete = null)
    {
        if (isAnimating || isFront) return;
        isAnimating = true;
        TargetPileManager.Instance.OnCardAnimationStart(); //<-动画开始登记

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOLocalRotate(new Vector3(0, 90, 0), flipDuration / 2))
            .AppendCallback(() => ShowFrontInstant())
            .Append(transform.DOLocalRotate(new Vector3(0, 0, 0), flipDuration / 2))
            .OnComplete(() =>
            {
                isAnimating = false;
                isFront = true;
                onComplete?.Invoke();
                TargetPileManager.Instance.OnCardAnimationEnd(); //<-动画结束登记
            });
    }

    public void ShowFrontInstant()
    {
        if (front != null) front.SetActive(true);
        if (back != null) back.SetActive(false);
        //transform.localRotation = Quaternion.Euler(0, 0, 0);
        isFront = true;
    }

    public void ShowBack()
    {
        if (front != null) front.SetActive(false);
        if (back != null) back.SetActive(true);
        isFront = false;
    }

    public void ShowFront()
    {
        if (front != null) front.SetActive(true);
        if (back != null) back.SetActive(false);
        isFront = true;
    }

    public void PlayShake()
    {
        rectTransform.DOShakeScale(0.2f, 0.2f);
    }

    // 只响应事件，不直接动画
    public void OnPointerClick(PointerEventData eventData)
    {
        onCardClicked?.Invoke(this);
    }

    public void RotateAndFlyTo(Vector2 targetLocalPos, System.Action onComplete = null)
    {
        if (isAnimating || hasPlayed) return;
        isAnimating = true;
        TargetPileManager.Instance.OnCardAnimationStart();//动画开始登记
        transform.SetAsLastSibling();

        Sequence seq = DOTween.Sequence();
        float curZ = rectTransform.localEulerAngles.z;//引入当前卡牌角度
        seq.Join(rectTransform.DOLocalRotate(new Vector3(0, 0, curZ + 360), rotateDuration, RotateMode.FastBeyond360));
        seq.Join(rectTransform.DOAnchorPos(targetLocalPos, flyDuration));
        seq.Join(rectTransform.DOScale(scaleSize, flyDuration));
        seq.Append(rectTransform.DOScale(1f, 0.2f));
        seq.AppendCallback(() =>
        {
            isAnimating = false;
            hasPlayed = true;
            ShowFront();
            onComplete?.Invoke();
            TargetPileManager.Instance.OnCardAnimationEnd(); //<-动画结束登记
        });
    }
    //提醒动画：可点击未被点击
    public void PlayHintAnim()
    {
        // 避免多次调用堆叠
        rectTransform.DOKill();
        Sequence seq = DOTween.Sequence();
        seq.Append(rectTransform.DOScale(1.15f, 0.2f));          // 放大一点
        seq.Append(rectTransform.DOShakeScale(0.25f, 0.24f));      // 晃动
        seq.Append(rectTransform.DOScale(1f, 0.2f));              // 恢复
    }
    //用于撤销——不带动画直接切牌面
    public void SetFrontState(bool isFront)
    {
        this.isFront = isFront;
        if (isFront)
            ShowFront();
        else
            ShowBack();
    }
    // 用于撤销
    public void SetPlayedState(bool played)
    {
        hasPlayed = played;
    }
    //撤销后飞回动画
    public void FlyBackTo(Transform targetParent, int siblingIndex, Vector2 targetAnchoredPos, System.Action onFinish = null)
    {
        if (isAnimating) return;
        isAnimating = true;
        TargetPileManager.Instance.OnCardAnimationStart();

        //父节点外面已经SetParent了，不能动了
        //rectTransform.SetParent(targetParent, false);

        Sequence seq = DOTween.Sequence();
        //缓动类型：Easing, Ease.OutQuad =>先快后慢 ｜Ease.Linear =>匀速运动 ｜ Ease.InQuad =>先慢后快 ｜ Ease.InOutQuad =>先加速后减速
        seq.Append(rectTransform.DOAnchorPos(targetAnchoredPos, flyDuration).SetEase(Ease.OutQuad));
        seq.Join(rectTransform.DOScale(1.0f, flyDuration));
        seq.OnComplete(() =>
        {
            rectTransform.SetSiblingIndex(siblingIndex);
            isAnimating = false;
            TargetPileManager.Instance.OnCardAnimationEnd();
            onFinish?.Invoke();
        });
    }
    //撤销后Target Pile自动翻回背面动画
    public void FlipToBack(System.Action onComplete = null)
    {
        if (isAnimating || !isFront) return;
        isAnimating = true;
        TargetPileManager.Instance.OnCardAnimationStart();
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOLocalRotate(new Vector3(0, 90, 0), flipDuration / 2))
            .AppendCallback(() => ShowBack())
            .Append(transform.DOLocalRotate(new Vector3(0, 0, 0), flipDuration / 2))
            .OnComplete(() =>
            {
                isAnimating = false;
                isFront = false;
                onComplete?.Invoke();
                TargetPileManager.Instance.OnCardAnimationEnd();
            });

    }

}

