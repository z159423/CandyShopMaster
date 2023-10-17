using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerMoneyText : MonoBehaviour
{
    [SerializeField] Text moneyText;

    int currentValue = 0;

    bool isCollecting = false;

    TaskUtil.DelayTaskMethod delay = null;

    Tween scaleTween = null;

    public void ChangeFloatingText(int value)
    {
        if (isCollecting)
            delay.Kill();

        isCollecting = true;

        currentValue += value;

        moneyText.text = currentValue.ToString() + "$";

        if (scaleTween != null)
        {
            if (!scaleTween.IsPlaying())
                scaleTween = moneyText.transform.DOScale(Vector3.one, 0.3f);
        }
        else
            scaleTween = moneyText.transform.DOScale(Vector3.one, 0.3f);

        delay = this.TaskDelay(1.5f, () => Reset());
    }

    public void Reset()
    {
        currentValue = 0;

        isCollecting = false;

        moneyText.transform.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InOutCubic);
    }
}
