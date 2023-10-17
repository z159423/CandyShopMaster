using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUISlot : MonoBehaviour
{
    public Item item;

    [SerializeField] Image candyImage;
    [SerializeField] Text candyCount;

    public void Init(Item item)
    {
        this.item = item;

        UpdateUI();
    }

    public void AddItem(int count)
    {
        this.item.count += count;

        UpdateUI();
    }

    void UpdateUI()
    {
        candyImage.sprite = SaveManager.instance.FindCandyObjectInReousrce(item.id).icon;
        candyCount.text = "X " + (item.count);
    }

    public void Clear()
    {
        item = null;
        Managers.Pool.Push(GetComponent<Poolable>());
    }
}
