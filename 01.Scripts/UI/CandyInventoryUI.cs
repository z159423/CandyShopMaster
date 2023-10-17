using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CandyInventoryUI : MonoBehaviour
{
    [SerializeField] Canvas parent;
    [SerializeField] Image candy_Image;
    [SerializeField] Text count_Text;

    Tween punchScaleTween = null;

    // candySaveData candyItem;

    public void Init(int id, int count)
    {
        // this.candyItem = candySave;

        candy_Image.sprite = SaveManager.instance.FindCandyObjectInReousrce(id).icon;

        count_Text.text = "X " + (count);

    }

    public void UpdateUI(int id, int count, bool updateIcon = false, bool wiggle = false)
    {
        if (updateIcon)
            candy_Image.sprite = SaveManager.instance.FindCandyObjectInReousrce(id).icon;

        if (wiggle && (punchScaleTween != null ? !punchScaleTween.IsPlaying() : true))
            punchScaleTween = parent.transform.DOPunchScale(parent.transform.localScale * 0.3f, 0.2f, 2).OnComplete(() => parent.transform.localScale = new Vector3(3.5f, 3.5f, 1f));


        count_Text.text = "X " + (count);
    }
}

