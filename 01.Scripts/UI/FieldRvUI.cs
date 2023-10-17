using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class FieldRvUI : MonoBehaviour
{
    [SerializeField] FieldRvType type;

    [SerializeField] Text moneyText;
    [SerializeField] GameObject noThanksBtn;

    public System.Action onComplete;

    int[] moneyValues = { 150, 300, 450, 600, 850, 1000 };

    public string pos;

    private void Start()
    {
        if (moneyText != null)
            moneyText.text = "+ " + moneyValues[IdleManager.instance.candyMachines.Where((n) => n.isReady).ToList().Count];

        this.TaskDelay(2f, () => noThanksBtn.SetActive(true));
    }

    public void OnClickClameBtn()
    {
        MondayOFF.AdsManager.ShowRewarded(() =>
        {
            switch (type)
            {
                case FieldRvType.SpeedUp:
                    IdleManager.instance.FieldRV_PlayerSpeedUp();
                    IdleManager.instance.BanFieldRv(type);
                    break;

                case FieldRvType.Money:
                    IdleManager.instance.FieldRV_Money(moneyValues[IdleManager.instance.candyMachines.Where((n) => n.isReady).ToList().Count]);
                    EventManager.instance.CustomEvent(AnalyticsType.RV, type.ToString() + " - " + moneyValues[IdleManager.instance.candyMachines.Where((n) => n.isReady).ToList().Count] + " - " + pos, true, true);

                    break;

            }
            EventManager.instance.CustomEvent(AnalyticsType.RV, type.ToString(), true, true);

            onComplete.Invoke();

            Destroy(gameObject);

        });
    }

    public void OnClickNoThanksBtn()
    {
        EventManager.instance.CustomEvent(AnalyticsType.UI, type + "_OnClickNoThanks", true, true);

        Destroy(gameObject);
    }
}

public enum FieldRvType
{
    SpeedUp = 0,
    Money = 1
}
