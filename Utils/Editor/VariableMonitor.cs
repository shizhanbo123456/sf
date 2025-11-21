using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class VariableMonitor : EditorWindow
{
    // 用于存储要监控的变量信息
    private Dictionary<string, System.Func<string>> monitoredVariables = new()
    {
        { 
            "模式",()=>
            {
                if(Tool.FightController==null)return "Uninitialized";
                return Tool.FightController.ModeList;
            }
        }
    };

    // 窗口位置和大小记忆
    private Vector2 scrollPosition;
    private const int windowWidth = 300;
    private const int windowHeight = 400;

    // 添加菜单选项
    [MenuItem("Window/Variable Monitor")]
    public static void ShowWindow()
    {
        GetWindow<VariableMonitor>("Variable Monitor", true, typeof(SceneView));
    }

    private void OnEnable()
    {
        // 设置窗口初始大小
        minSize = new Vector2(windowWidth, windowHeight);
    }

    private void OnGUI()
    {
        GUILayout.Label("Variable Monitor Panel", EditorStyles.boldLabel);
        EditorGUILayout.Separator();

        // 非运行模式下显示提示
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("当前处于编辑模式，变量值将在运行时更新", MessageType.Info);
            return;
        }
        else
        {
            EditorGUILayout.HelpBox("运行中 - 实时显示变量值", MessageType.Info);
        }

        EditorGUILayout.Separator();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        GUI.enabled = false;

        foreach (var variable in monitoredVariables)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(variable.Key, GUILayout.Width(120));
            EditorGUILayout.TextField(variable.Value.Invoke());
            EditorGUILayout.EndHorizontal();
        }
        GUI.enabled = true;

        EditorGUILayout.EndScrollView();
        EditorGUILayout.Separator();

        // 每帧刷新窗口
        if (Application.isPlaying)
        {
            Repaint();
        }
    }
}