using ModeTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeSelectionSubpage : MonoBehaviour
{
    [SerializeField] private Text HeaderText;
    [SerializeField] private List<Text> SelectionButtonsText = new List<Text>();
    [SerializeField] private List<GameObject> SelectionButtons = new List<GameObject>();
    [SerializeField] private GameObject BackButton;

    // 当前导航路径（记录每层选择的索引）
    private List<int> _currentPath = new List<int>();
    // 当前所在的节点
    private ModeNode _currentNode;

    private void OnEnable()
    {
        // 重置到根节点
        ResetToRoot();
    }

    // 重置到根节点（初始状态）
    private void ResetToRoot()
    {
        _currentPath.Clear();
        _currentNode = ModeManifest.Modes;
        UpdateUI();
    }

    // 更新界面显示（根据当前节点刷新标题和按钮）
    private void UpdateUI()
    {
        // 更新标题（优先显示ContentName，没有则用ModeName）
        HeaderText.text = string.IsNullOrEmpty(_currentNode.ContentName)
            ? _currentNode.ModeName
            : _currentNode.ContentName;

        // 隐藏所有按钮，按需显示
        foreach (var btn in SelectionButtons)
            btn.SetActive(false);

        // 如果当前节点有子节点，显示对应数量的按钮
        if (_currentNode.Submodes != null)
        {
            for (int i = 0; i < _currentNode.Submodes.Count; i++)
            {
                if (i >= SelectionButtons.Count) break; // 防止按钮数量不足

                // 显示按钮并设置文本（优先用ModeName）
                SelectionButtons[i].SetActive(true);
                var childNode = _currentNode.Submodes[i];
                SelectionButtonsText[i].text = string.IsNullOrEmpty(childNode.ModeName)
                    ? childNode.ContentName
                    : childNode.ModeName;
            }
        }

        // 根节点不显示返回按钮
        BackButton.SetActive(_currentPath.Count > 0);
    }

    // 选择当前层级的第x个选项
    public void Select(int x)
    {
        if (_currentNode.Submodes == null || x < 0 || x >= _currentNode.Submodes.Count)
            return;

        // 进入子节点
        var selectedNode = _currentNode.Submodes[x];
        _currentPath.Add(x);
        _currentNode = selectedNode;

        // 检查是否是叶子节点（ModeInfo类型且没有子节点）
        if (selectedNode is ModeInfo && (selectedNode.Submodes == null || selectedNode.Submodes.Count == 0))
        {
            // 确认选择该模式
            OnModeConfirm(new List<int>(_currentPath));
        }
        else
        {
            // 非叶子节点，更新UI显示下一级
            UpdateUI();
        }
    }

    // 返回上一级目录
    public void Back()
    {
        if (_currentPath.Count == 0) return; // 已经在根节点，无法返回

        // 移除最后一步路径，回到上一级节点
        _currentPath.RemoveAt(_currentPath.Count - 1);

        // 重新计算当前节点（从根节点重新导航）
        _currentNode = ModeManifest.Modes;
        foreach (var index in _currentPath)
        {
            if (index < 0 || index >= _currentNode.Submodes.Count)
                break; // 路径异常，终止导航
            _currentNode = _currentNode.Submodes[index];
        }

        UpdateUI();
    }

    // 确认选择模式（到达叶子节点时调用）
    public void OnModeConfirm(List<int> modeIndexCollection)
    {
        ResetToRoot();

        gameObject.SetActive(false);
        Tool.FightController.ModeList = string.Join("", modeIndexCollection);
    }
}