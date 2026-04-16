using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionPanel:MonoBehaviour
{
    [SerializeField] private Text SelectionLabel;
    [SerializeField] private List<Button> Selections = new();//1个
    [SerializeField] private Button ConfirmButton;

    private int selected = -1;
    private Action<int> OnConfirm;

    private void Awake()
    {
        Selections[0].onClick.AddListener(() => OnSelect(0));
        ConfirmButton.onClick.AddListener(Confirm);
    }
    private void OnSelect(int x)
    {
        selected = x;
        for (int i = 0; i < Selections.Count; i++)
        {
            Selections[i].targetGraphic.color = x == i ? Color.yellow : Color.gray;
        }
    }
    public void ShowPanel(Action<int>onConfirm,string label, string[] selections)
    {
        gameObject.SetActive(true);
        selected = -1;
        SelectionLabel.text = label;
        OnConfirm=onConfirm;
        while (Selections.Count < selections.Length)
        {
            var b=Instantiate(Selections[0].gameObject, Selections[0].transform.parent).GetComponent<Button>();
            b.onClick.RemoveAllListeners();
            int j = Selections.Count;
            b.onClick.AddListener(()=>OnSelect(j));
            Selections.Add(b);
        }
        for(int i = 0; i < Selections.Count; i++)
        {
            if (i < selections.Length)
            {
                Selections[i].gameObject.SetActive(true);
                Selections[i].transform.GetComponent<Text>().text = selections[i];
            }
            else
            {
                Selections[i].gameObject.SetActive(false);
            }
        }
        OnSelect(0);
    }
    public void HidePanel()
    {
        gameObject.SetActive(false);
    }
    private void Confirm()
    {
        OnConfirm?.Invoke(selected);
        gameObject.SetActive(false);
    }
}