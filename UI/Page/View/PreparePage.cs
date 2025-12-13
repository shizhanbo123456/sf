using UnityEngine;
using UnityEngine.UI;

public class PreparePage : BasePage
{
    public static PrepareController controller=>PrepareController.Instance;

    [SerializeField] private GameObject SelectModeButton;
    [SerializeField] private GameObject ModeSelectionPanel;
    [SerializeField] private GameObject SettingsPanel;
    [Space]
    [SerializeField] private InputField RoomNameInput;
    [SerializeField]private Text RoomId;
    [SerializeField] private Text RoomName;
    [SerializeField] private Text Mode;

    public override void Repaint()
    {
        Mode.text = controller.ModeName;
        RoomNameInput.text = controller.ModeName;
        RoomNameInput.gameObject.SetActive(controller.HasAuthority);
        SelectModeButton.SetActive(controller.HasAuthority);

        SettingsPanel.SetActive(controller.SettingsPanelActive);
        ModeSelectionPanel.SetActive(controller.ModeSelectionPanelActive);

        if (EnsInstance.Corr.networkMode == EnsCorrespondent.NetworkMode.Host)
        {
            RoomId.text = Tool.GetIP();
        }
        else if (EnsInstance.Corr.networkMode == EnsCorrespondent.NetworkMode.Client)
        {
            RoomId.text = EnsInstance.PresentRoomId.ToString();
        }
    }
    private void OnEnable()
    {
        controller.Enter();
    }
    private void OnDisable()
    {
        controller.Exit();
    }
    public void EnterSettingsPanel() => controller.EnterSettingsPanel();
    public void ExitSettingsPanel()
    {
        if (controller.HasAuthority)
        {
            if (RoomNameInput.text.Length < 1 || RoomNameInput.text.Length > 8)
            {
                RoomNameInput.text = "游戏房间";
                Tool.Notice.ShowMesg("房间名长度应在1-8之间");
                controller.ExitSettingsPanel("RoomName");
            }
            else controller.ExitSettingsPanel(RoomNameInput.text);
        }
        else controller.ExitSettingsPanel(string.Empty);
    }
    public void StartFight()=>controller.StartFight();
    public void ExitRoom() => controller.ExitRoom();
}