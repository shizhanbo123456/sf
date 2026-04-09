using LevelCreator;
using SF.UI.Bar;
using SF.UI.Skill;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayModePage : BasePage
{
    private static PlayModeController controller=>PlayModeController.Instance;
    public BarPanel BarPanel;
    public SkillPanel SkillPanel;
    public Scoreboard Scoreboard;
    public SelectionPanel SelectionPanel;
    public EffectPanel EffectPanel;
    [SerializeField] private List<GameObject> HostOnlyObjects = new List<GameObject>();
    [Header("Settings")]
    [SerializeField] private GameObject Settings;
    private int lastFrameTime;
    [SerializeField] private Text TimePassed;
    [SerializeField] private Text ModeName;
    [SerializeField] private Text ModeDes;
    [SerializeField]private List<Transform> SkillInfos=new List<Transform>();
    private bool SettingsOn;
    [Space]
    public Settlement settlement;

    public override void Repaint()
    {
        foreach (var i in HostOnlyObjects) i.SetActive(EnsInstance.HasAuthority);
        ModeName.text = controller.modeName;
        ModeDes.text = controller.modeDescription;
        Settings.gameObject.SetActive(SettingsOn);
    }
    private void Awake()
    {
        foreach (var i in SkillInfos) i.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (SettingsOn)
        {
            int timecount = (int)LevelCreator.CustomLevel.FightTime;
            if (lastFrameTime != timecount)
            {
                TimePassed.text = timecount / 60 + ":" + timecount % 60;
                lastFrameTime= timecount;
            }
        }
    }

    
    public List<SkillColumn> CreateSkillColumns(ushort[] ids)
    {
        var columns= SkillPanel.CreateSkillColumns(ids);
        for (int i = 0; i < ids.Length; i++)
        {
            var info = Tool.LevelCreatorManager.GetSkillInfo(ids[i]);
            SkillInfos[i].gameObject.SetActive(true);
            SkillInfos[i].GetChild(0).GetComponent<Text>().text = info.name;
            SkillInfos[i].GetChild(1).GetComponent<Image>().sprite = Tool.SpriteManager.GetSprite(info.sprite);
            SkillInfos[i].GetChild(2).GetComponent<Text>().text = info.des;
        }
        return columns;
    }
    public void DestroyAllSkillColumns()
    {
        SkillPanel.DestroyAllSkillColumns();
        for (int i = 0; i < SkillInfos.Count; i++)
        {
            SkillInfos[i].gameObject.SetActive(false);
        }
    }
    


    public void ShowSettings(bool show)
    {
        SettingsOn = show;
        Repaint();
    }
    public void BackToPrepare() => controller.BackToPrepare();
    public void ExitGame() => controller.ExitGame();
}