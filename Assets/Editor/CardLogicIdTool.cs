using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CardLogicIdTool : MonoBehaviour
{
    [MenuItem("工具/卡牌/为场景所有卡牌分配唯一ID")]
    public static void AssignInstanceIds()
    {
        var cards = GameObject.FindObjectsOfType<CardLogic>();

        int id = 1;
        foreach (var logic in cards)
        {
            SerializedObject so = new SerializedObject(logic);
            so.FindProperty("instanceId").intValue = id++;
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(logic);
        }

        Debug.Log($"已为{cards.Length}个CardLogic分配唯一ID！");
    }
}
