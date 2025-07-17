using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillManager : MonoBehaviour
{
    [Header("撤销次数设置")]
    public int undoSkillRemain = 3; // 可用次数，Inspector设置

    [Header("连UI元素")]
    public Button undoSkillBtn;         // 拖入Undo按钮
    public TextMeshProUGUI undoSkillCountText;     // 拖入显示次数的Text

    void Start()
    {
        undoSkillBtn.onClick.AddListener(OnUndoSkillButtonClicked);
        RefreshUndoSkillUI();
    }

    public void OnUndoSkillButtonClicked()
    {
        if (undoSkillRemain <= 0)
            return;
        
        // 撤销实际动作（这里调用UndoManager逻辑）
        UndoManager.Instance.UndoLastMove();

        undoSkillRemain--;
        RefreshUndoSkillUI();
    }

    // UI联动
    void RefreshUndoSkillUI()
    {
        // 数字独立显示
        undoSkillCountText.text = undoSkillRemain.ToString();
        // 按钮可用状态
        undoSkillBtn.interactable = undoSkillRemain > 0;
    }
}
