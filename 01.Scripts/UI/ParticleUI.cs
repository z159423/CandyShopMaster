using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleUI : MonoBehaviour
{
    [SerializeField] GameObject ExpandBtn;
    [SerializeField] UnityEngine.UI.Button inventoryBtn;
    [SerializeField] UIDynamicPanel dynamicPanel;
    public UIDynamicPanel GetDynamic => dynamicPanel;

    private void Start()
    {
        if (ES3.KeyExists("ParticleUI") ? ES3.Load<bool>("ParticleUI") : false)
        {
            dynamicPanel.Collapse(true, onComplete: () => { ExpandBtn.SetActive(true); inventoryBtn.enabled = true; });

            ExpandBtn.SetActive(true);
        }
    }

    public void OnClickInventoryBtn(bool immediate = false)
    {
        inventoryBtn.enabled = false;
        dynamicPanel.Collapse(immediate, onComplete: () => { ExpandBtn.SetActive(true); ES3.Save<bool>("ParticleUI", true); });
    }

    public void OnClickExpandBtn(bool immediate = false)
    {
        ExpandBtn.SetActive(false);
        dynamicPanel.Expand(immediate: immediate, onComplete: () => { inventoryBtn.enabled = true; ES3.Save<bool>("ParticleUI", false); });
    }

}
