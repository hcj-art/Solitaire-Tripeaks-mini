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

    public void FlyInFrom(Vector2 fromLocal, Vector2 toLocal, float delay = 0f)
    {
        if (isAnimating) return;
        isAnimating = true;
        rectTransform.anchoredPosition = fromLocal;
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.localScale = Vector3.one;
        rectTransform.SetAsLastSibling(); // 确保动画遮挡正确
    
        // 动画
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(delay); // 错开更自然
        seq.Append(rectTransform.DOAnchorPos(toLocal, 0.5f).SetEase(Ease.OutQuad));
        seq.Join(rectTransform.DOScale(scaleSize, 0.25f).SetLoops(2, LoopType.Yoyo)); // 先放大弹一下
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
}