using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeSelectionSubpage : MonoBehaviour
{
    [SerializeField] private Text HeaderText;
    [SerializeField] private GameObject SelectionButtonsTemplate;//»ádisable
    [SerializeField] private GameObject BackButton;
    private List<Text> SelectionButtonsLabels = new List<Text>();
    private List<int>nextSelectionList = new List<int>();

    private int nextButtonTrigIndex;

    private List<int> _currentPath = new List<int>();
    private static Dictionary<int, string> IntToPart => CustomLevelSelector.IntToPart;

    private void OnEnable()
    {
        ResetToRoot();
        SelectionButtonsTemplate.SetActive(false);
    }
    private void ResetToRoot()
    {
        _currentPath.Clear();
        nextSelectionList = CustomLevelSelector.GetNextMatchedInfoIndex(_currentPath);
        UpdateUI();
    }
    private void UpdateUI()
    {
        HeaderText.text = _currentPath.Count>0?IntToPart[_currentPath[^1]]:string.Empty;

        ActiveButtonsFor(nextSelectionList.Count);
        for(int i = 0; i < nextSelectionList.Count; i++)
        {
            SelectionButtonsLabels[i].text=IntToPart[CustomLevelSelector.LevelInfo[nextSelectionList[i]].path[_currentPath.Count]];
        }
        BackButton.SetActive(_currentPath.Count > 0);
    }
    private void ActiveButtonsFor(int num)
    {
        while(SelectionButtonsLabels.Count < num)
        {
            var obj=Instantiate(SelectionButtonsTemplate,SelectionButtonsTemplate.transform.parent);
            int i = nextButtonTrigIndex++;
            obj.GetComponent<Button>().onClick.AddListener(() => Select(i));//ÅÅ³ýÄ£°åµÄindex
            SelectionButtonsLabels.Add(obj.GetComponent<Text>());
        }
        for(int i = 0;i < SelectionButtonsLabels.Count; i++)
        {
            SelectionButtonsLabels[i].gameObject.SetActive(i < num);
        }
    }
    private void Select(int x)
    {
        _currentPath.Add(CustomLevelSelector.LevelInfo[nextSelectionList[x]].path[_currentPath.Count]);
        nextSelectionList = CustomLevelSelector.GetNextMatchedInfoIndex(_currentPath);

        if (nextSelectionList==null) OnModeConfirm();
        else UpdateUI();
    }
    public void Back()
    {
        if (_currentPath.Count == 0) return;
        _currentPath.RemoveAt(_currentPath.Count - 1);

        nextSelectionList = CustomLevelSelector.GetNextMatchedInfoIndex(_currentPath);
        UpdateUI();
    }
    public void OnModeConfirm()
    {
        Tool.FightController.SelectedMode = CustomLevelSelector.GetCustomLevelText(_currentPath);
        ResetToRoot();
        gameObject.SetActive(false);
    }
}