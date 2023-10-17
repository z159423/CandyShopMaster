using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IdleUpgradeSlot : MonoBehaviour
{
    [SerializeField] IdleUpgradeType upgrade;

    [SerializeField] Image btnImage;
    [SerializeField] Text cost;

    public bool isPossibleUpgrade => SaveManager.instance.CheckPossibleUpgrade(IdleManager.instance.GetCurrentUpgradeCost(upgrade));
    public int GetUpgradeCost => IdleManager.instance.GetCurrentUpgradeCost(upgrade);


    private void Start()
    {
        SaveManager.instance.onMoneyChangeEvent.AddListener(CheckingBtn);
        Init();
        CheckingBtn();
    }

    public void Init()
    {
        SetUpgradeCostText();
    }

    public void OnClickUpgradeBtn()
    {
        switch (upgrade)
        {
            case IdleUpgradeType.HireWorker:
                IdleManager.instance.Upgrade_HireWorker();
                break;

            case IdleUpgradeType.WorkerSpeedUp:
                IdleManager.instance.Upgrade_WorkerSpeedUp();
                break;

            case IdleUpgradeType.Promotion:
                IdleManager.instance.Upgrade_Promotion();
                break;

            case IdleUpgradeType.Income:
                IdleManager.instance.Upgrade_Income();
                break;

            case IdleUpgradeType.PlayerSpeedUp:
                IdleManager.instance.Upgrade_PlayerSpeedUp();
                break;

            case IdleUpgradeType.PlayerCapacityUp:
                IdleManager.instance.Upgrade_PlayerCapacityUp();
                break;

            case IdleUpgradeType.WorkerCapacityUp:
                IdleManager.instance.Upgrade_WorkerCapacityUp();
                break;

            default:
                Debug.LogError("정의가 없습니다. 추가해 주십시요");
                break;
        }

        SetUpgradeCostText();
        CheckingBtn();
        // IdleManager.instance.UpgradeUISort();
    }

    void CheckingBtn()
    {
        if (SaveManager.instance.CheckPossibleUpgrade(IdleManager.instance.GetCurrentUpgradeCost(upgrade)))
        {
            btnImage.color = IdleManager.instance.activeBtnColor;
            cost.color = IdleManager.instance.activeCostColor;
            btnImage.GetComponent<Button>().enabled = true;
        }
        else
        {
            btnImage.color = IdleManager.instance.deactiveBtnColor;
            cost.color = IdleManager.instance.deactiveCostColor;
            btnImage.GetComponent<Button>().enabled = false;
        }
    }

    void SetUpgradeCostText()
    {
        var value = IdleManager.instance.GetUpgradeValue(upgrade);

        if (value.currentLevel >= value.maxLevel)
            gameObject.SetActive(false);
        else
            cost.text = IdleManager.instance.GetUpgradeCost(upgrade).ToString();

    }
}
