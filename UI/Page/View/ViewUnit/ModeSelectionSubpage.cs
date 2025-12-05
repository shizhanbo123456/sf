using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeSelectionSubpage : MonoBehaviour
{
    [SerializeField] private Text HeaderText;
    [SerializeField] private GameObject SelectionButtonsTemplate;//»ádisable
    [SerializeField] private List<Text> SelectionButtonsLabels = new List<Text>();
    [SerializeField] private GameObject BackButton;

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
        UpdateUI();
    }
    private void UpdateUI(List<int> buffer=null)
    {
        HeaderText.text = IntToPart[_currentPath[^1]];

        if (buffer == null) buffer = CustomLevelSelector.GetNextSelectionListIndex(_currentPath);
        ActiveButtonsFor(buffer.Count);
        for(int i = 0; i < buffer.Count; i++)
        {
            SelectionButtonsLabels[i].text=IntToPart[buffer[i]];
        }
        BackButton.SetActive(_currentPath.Count > 0);
    }
    private void ActiveButtonsFor(int num)
    {
        while(SelectionButtonsLabels.Count < num)
        {
            var obj=Instantiate(SelectionButtonsTemplate,SelectionButtonsTemplate.transform.parent);
            obj.GetComponent<Button>().onClick.AddListener(() => Select(SelectionButtonsTemplate.transform.childCount - 2));//ÅÅ³ýÄ£°åµÄindex
            SelectionButtonsLabels.Add(obj.GetComponent<Text>());
        }
        for(int i = 0;i < SelectionButtonsLabels.Count; i++)
        {
            SelectionButtonsLabels[i].gameObject.SetActive(i < num);
        }
    }
    private void Select(int x)
    {
        _currentPath.Add(x);
        var next = CustomLevelSelector.GetNextSelectionListIndex(_currentPath);

        if (next==null) OnModeConfirm();
        else UpdateUI(next);
    }
    public void Back()
    {
        if (_currentPath.Count == 0) return;

        _currentPath.RemoveAt(_currentPath.Count - 1);
        UpdateUI();
    }
    public void OnModeConfirm()
    {
        ResetToRoot();

        gameObject.SetActive(false);
        Tool.FightController.SelectedMode = CustomLevelSelector.GetCustomLevelText(_currentPath);
    }
}