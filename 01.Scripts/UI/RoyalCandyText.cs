using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoyalCandyText : MonoBehaviour
{
    private void OnEnable()
    {
        this.TaskWaitUntil(() => SaveManager.instance.AddRoyalCandyText(GetComponent<UnityEngine.UI.Text>()), () => SaveManager.instance != null);
    }

    private void OnDisable()
    {
        SaveManager.instance.royalCandyTextList.Remove(GetComponent<UnityEngine.UI.Text>());
    }
}
