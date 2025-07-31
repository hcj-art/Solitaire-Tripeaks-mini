using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    public GameObject cardGameRootPrefab;
    public Transform canvasParent;  // 新增！让它等会指向我的gameCanvas

    private GameObject currentRoot;

    void Start()
    {
        //ResetCardGameRoot(); 我在开始按钮点击时也会重置，所以这里不需要了
    }

    public void ResetCardGameRoot()
    {
        if (currentRoot != null)
            Destroy(currentRoot);

        // 关键的变成：指定父物体
        currentRoot = Instantiate(cardGameRootPrefab, canvasParent, false);

        int resultPanelIdx = canvasParent.childCount - 1; // resultPanel在最底部（最后一个）
        currentRoot.transform.SetSiblingIndex(resultPanelIdx - 1);
    }
}
