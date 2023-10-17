using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CandyJar : MonoBehaviour
{
    public candySaveData candyItem;

    [SerializeField] private MeshRenderer[] candyMeshes;

    [SerializeField] Canvas CandyCanvas;
    [SerializeField] Text test_candyName;
    [SerializeField] Image candyImage;
    [SerializeField] Text test_candyCount;

    Tween bubbleTween;

    public void Init(candySaveData item)
    {
        candyItem = item;

        Material[] newMat = new Material[] { SaveManager.instance.FindCandyObjectInReousrce(candyItem.id).mat };

        foreach (var mr in candyMeshes)
        {
            mr.materials = newMat;
        }

        candyImage.sprite = SaveManager.instance.FindCandyObjectInReousrce(candyItem.id).icon;
        test_candyCount.text = "X " + (candyItem.count);
    }

    public void ChangeJarModel(int id)
    {
        var candy = SaveManager.instance.FindCandyObject(id);

        Material[] newMat = new Material[] { candy.mat };

        foreach (var mr in candyMeshes)
        {
            mr.materials = newMat;
        }
    }

    public void OnChangeOrder(bool wiggle = false)
    {
        // test_candyName.text = candyItem.candy.name;
        candyImage.sprite = SaveManager.instance.FindCandyObjectInReousrce(candyItem.id).icon;
        test_candyCount.text = "X " + (candyItem.count);

        if (wiggle)
            CandyCanvas.transform.DOPunchScale(CandyCanvas.transform.localScale * 0.3f, 0.2f, 2);
    }

    public void BubbleWiggle()
    {
        if (bubbleTween == null)
            bubbleTween = CandyCanvas.transform.DOPunchScale(CandyCanvas.transform.localScale * 0.3f, 0.2f, 2).OnComplete(() => bubbleTween = null);
    }

    public void UpdateUI()
    {
        test_candyCount.text = "X " + (candyItem.count);
    }
}
