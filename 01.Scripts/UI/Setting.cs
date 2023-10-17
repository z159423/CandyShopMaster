using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting : MonoBehaviour
{

    public void OpenSettingUI()
    {
        var ui = Managers.UI.ShowPopupUI<UI_Popup>("UI_PopupSetting", transform.parent);

        var rect = ui.GetComponent<RectTransform>();

        rect.anchorMin = new Vector3(0, 0);
        rect.anchorMax = new Vector3(1, 1);

        rect.offsetMin = new Vector2(0, 0);
        rect.offsetMax = new Vector2(0, 0);

        rect.localScale = Vector3.one;

        EventManager.instance.CustomEvent(AnalyticsType.UI, "OpenSetting", true, true);


        // MondayOFF.EventTracker.LogCustomEvent(
        // "UI",
        // new Dictionary<string, string> { { "UI_TYPE", "OpenSetting" } }
        // );
    }
}
