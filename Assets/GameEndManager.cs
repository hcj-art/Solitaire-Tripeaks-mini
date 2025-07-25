using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndManager : MonoBehaviour
{
    public ResultPanel resultPanel;
    public static GameEndManager Instance { get; private set; }
    private bool gameOver = false;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    //检查游戏成功失败
    public void CheckWinOrLose()
    {
        if (gameOver) return;

        //判断目标区是否全部消除：普通牌和isTargetWild牌
        bool targetNotCleared = false;
        foreach (var kvp in TargetPileManager.Instance.allDict)
        {
            var logic = kvp.Value;
            if (logic == null || logic.view == null) continue;
            bool isTargetWild = logic.isTargetWild;
            bool isNormalTarget = (!logic.isWild && !logic.isTargetWild);
            if ((isTargetWild || isNormalTarget) && !logic.view.IsPlayed())
            {
                targetNotCleared = true;
                break;
            }
        }

        //判断主牌区（手牌区）是否还有未打出的牌
        bool cardPileAllGone = true;
        foreach (var kvp in CardManager.Instance.allDict)
        {
            var logic = kvp.Value;
            if (logic == null || logic.view == null) continue;
            if (!logic.view.IsPlayed())
            {
                cardPileAllGone = false;
                break;
            }
        }

        //胜利条件：TargetPile的全部普通牌以及targetWild消除完
        if (!targetNotCleared)
        {
            gameOver = true;
            ShowResultPanel(true);
        }
        //失败条件：手牌区全部打完且Target区仍有余牌
        else if (cardPileAllGone)
    {
        int playedTop = PlayedAreaManager.Instance.GetTopCardNumber();
        bool canMatchAny = false;

        foreach (var kvp in TargetPileManager.Instance.allDict)
        {
            var logic = kvp.Value;
            if (logic == null || logic.view == null) continue;
            bool isTargetWild = logic.isTargetWild;
            bool isNormalTarget = (!logic.isWild && !logic.isTargetWild);
            if (!(isTargetWild || isNormalTarget)) continue; // 只判目标牌
            if (logic.view.IsPlayed()) continue;
            if (!TargetPileManager.Instance.IsCardFree(logic.instanceId)) continue;
            if (TargetPileManager.Instance.IsCardNumberCanMatch(logic.cardNumber, playedTop))
            {
                canMatchAny = true;
                break;
            }
        }
        if (!canMatchAny)
        {
            gameOver = true;
            ShowResultPanel(false);
        }
    }
    }
    void ShowResultPanel(bool win)
    {
        Debug.Log("Game End - " + (win ? "Victory!" : "Defeat!"));
        // TODO: 胜利/失败弹窗逻辑
        if (resultPanel != null)
            resultPanel.ShowResultPanel(win);
        else
            Debug.LogError("ResultPanel 没挂载!");
    }
}
