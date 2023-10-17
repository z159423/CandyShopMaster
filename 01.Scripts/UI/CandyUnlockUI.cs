using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CandyUnlockUI : MonoBehaviour
{
    [SerializeField] Image candyImage;
    [SerializeField] Image candyImage_black;
    [SerializeField] Transform collectTarget;
    [SerializeField] Transform clameBtn;
    // [SerializeField] Transform noThanksBtnƒ;

    [SerializeField] Text percentText;

    [SerializeField] CandyInventory candyinventory;
    [SerializeField] UIAttractorCustom[] uIAttractorCustoms;

    [SerializeField] UIDynamicPanel dynamicPanel;

    // public CandyUnlockStatus tempStatus;


    public CandyUnlockStatus currentStatus;

    public static CandyUnlockUI instance;

    private void Awake()
    {
        instance = this;
    }

    public void ShowUI()
    {
        // var current = SaveManager.instance.GetCandyUnlockStatuses().Where((n) => !n.unlocked).OrderBy((n) => n.id).ToArray()[0];
        var resource = Resources.LoadAll<CandyObject>("Candy").First((n) => n.id == currentStatus.id);

        candyImage.sprite = resource.icon;
        candyImage_black.sprite = resource.icon;

        percentText.text = Mathf.FloorToInt(currentStatus.GetCurrentPercent()) + "%";

        ChangeFillRate(1f - (currentStatus.GetCurrentPercent() * 0.01f));

        // currentStatus = current;
        dynamicPanel.Expand();

        this.TaskDelay(1.5f, StartFillAnimation);

        this.TaskDelay(4.5f, () =>
        {
            if (currentStatus.unlocked)
            {
                RunManager.instance.unlockedImage.sprite = resource.icon;
                RunManager.instance.NewCandyUnlockedUI.SetActive(true);

                if (StageManager.instance.currentStageNum == 4 && RunManager.forceIdle)
                {
                    RunManager.instance.NewCandyUnlockedUI_SellCandyBtn.SetActive(true);
                }
            }
            else if (StageManager.instance.currentStageNum == 4 && RunManager.forceIdle)
            {
                RunManager.instance.sellCandyBtn.SetActive(true);
            }
            else
            {
                clameBtn.gameObject.SetActive(true);
            }
        });
    }

    public void HideUI()
    {
        dynamicPanel.Collapse();
    }

    public void StartFillAnimation()
    {
        StartCoroutine(coroutin());

        IEnumerator coroutin()
        {
            var candyList = RunManager.instance.GetLastCandyInventory();

            for (int i = 0; i < candyList.candyItems.Count(); i++)
            {
                yield return new WaitForSeconds(0.2f);
                float point = Resources.LoadAll<CandyObject>("Candy").Where((n) => n.id == candyList.candyItems[i].candy.id).First().unlockPoint;
                uIAttractorCustoms[i].Init(candyList.candyItems[i], onAttract: () => { FillOnLive(point); });
            }
        }
    }

    public void FillOnLive(float point)
    {
        currentStatus.AddPercent(point);

        // Debug.LogError(Resources.LoadAll<CandyObject>("Candy").Where((n) => n.id == id).First().unlockPoint);

        ChangeFillRate(1 - (currentStatus.GetCurrentPercent() * 0.01f));
        percentText.text = Mathf.FloorToInt(currentStatus.GetCurrentPercent()) + "%";
    }

    public void OnCompleteFill()
    {

    }

    public void ChangeFillRate(float percent)
    {
        candyImage_black.fillAmount = percent;
    }

    public void RV_UnlockCandy()
    {
        MondayOFF.AdsManager.ShowRewarded(() =>
        {
            var _currentStatus = SaveManager.instance.GetCandyUnlockStatuses().Where((n) => !n.unlocked).OrderBy((n) => n.id).First();

            _currentStatus.unlocked = true;

            var resource = Resources.LoadAll<CandyObject>("Candy").First((n) => n.id == _currentStatus.id);

            RunManager.instance.unlockedImage.sprite = resource.icon;
            RunManager.instance.NewCandyUnlockedUI.SetActive(true);

            EventManager.instance.CustomEvent(AnalyticsType.RV, "UnlockCandy_" + _currentStatus.id, true, true);
        });
    }
}
