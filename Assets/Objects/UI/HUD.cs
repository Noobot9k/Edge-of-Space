using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour {

    string playerStatusText;
    public List<DisplayMsg> Messages = new List<DisplayMsg> {
        new DisplayMsg("Warning: Excessive heat! Taking Damage! Move out of red zone.", Color.red),
        new DisplayMsg("Refueling... Move quickly through yellow zone to refuel faster.", Color.yellow),
        new DisplayMsg("Performing repairs...", Color.green),
    };
    float _tickToHideMsg = 0;

    public TMP_Text PlayerStatusObj;
    public TMP_Text TutorialStatusObj;
    public Image TutorialStatusBackgroundObj;
    public RectTransform MessageObj;
    public TMP_Text MessageTextObj;

    public List<Image> MessageObjsToColor;

    public static HUD singleton;

    private void Start() {
        HUD.singleton = this;
        playerStatusText = PlayerStatusObj.text;
    }

    private void Update() {
        PlayerHandler player = PlayerHandler.singleton;
        object nextUpgradeCost = "MAX";
        if (player.Upgrades.Count > player.CurrentUpgradeIndex + 1)
            nextUpgradeCost = player.Upgrades[player.CurrentUpgradeIndex + 1].MetalCost;
        object[] args = new object[6] { RoundToTenths(player.Health), player.MaxHealth, RoundToTenths(player.Fuel), player.MaxFuel, RoundToTenths(player.Metal), nextUpgradeCost };
        PlayerStatusObj.text = string.Format(playerStatusText, args);

        if(_tickToHideMsg <= Time.time) MessageObj.gameObject.SetActive(false);

        if(StarmapHandler.singleton.CurrentStarSystem.IsVictoryStation) {
            SetTutorialStatus("Go dock at the space station to be taken to safety.", Color.green);
        } else if (player.CurrentUpgradeIndex == 0) {
            SetTutorialStatus("Shoot asteroids with [Mouse 1] to collect metal. You're on a time limit.", Color.cyan);
        } else if (player.Fuel < player.MaxFuel) {
            SetTutorialStatus("Skim fuel from a nearby star by flying in it's yellow zone. Fly around in it to refuel fastest.", Color.yellow);
        } else {
            SetTutorialStatus("Press [TAB] to jump to the next star system. Consider saving at the space station before leaving.", Color.green);
        }
    }

    void SetTutorialStatus(string msg, Color color) {
        TutorialStatusBackgroundObj.color = color;
        TutorialStatusObj.text = msg;
    }
    public void DisplayMsg(int msgIndex) {
        DisplayMsg messageData = Messages[msgIndex];
        DisplayMsg(messageData.Message, messageData.WindowColor, messageData.Duration);
    }
    public void DisplayMsg(string msg, Color color, float duration) {
        foreach(Image objToColor in MessageObjsToColor) {
            objToColor.color = color;
        }

        MessageTextObj.color = color;
        MessageTextObj.text = msg;
        _tickToHideMsg = Time.time + duration;
        MessageObj.gameObject.SetActive(true);
    }

    public static float RoundToTenths(float input) {
        return Mathf.Round(input * 10) / 10;
    }
}

[System.Serializable]
public class DisplayMsg {
    public string Message;
    public Color WindowColor;
    public float Duration = .5f;

    public DisplayMsg(string msg, Color color) {
        Message = msg;
        WindowColor = color;
    }
    public DisplayMsg(string msg, Color color, float duration) {
        Message = msg;
        WindowColor = color;
        Duration = duration;
    }
}
