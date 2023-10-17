using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CandyInventoryItem : MonoBehaviour
{
    [SerializeField] Image image;
    public Transform GetImageTrans => image.transform;
    [SerializeField] Text xText;
    [SerializeField] Text amountText;

    [SerializeField] Image m_image;


    public CandyItem candyItem;

    public void InitCandy(CandyObject candy, int count, bool cover = false)
    {
        // xText.gameObject.SetActive(true);
        // amountText.gameObject.SetActive(true);
        GetComponent<Image>().enabled = cover;
        image.gameObject.SetActive(true);

        m_image.gameObject.SetActive(false);

        image.sprite = SaveManager.instance.FindCandyObject(candy.id).icon;

        candyItem.candy = candy;
        candyItem.count = count;

        amountText.text = count.ToString();
    }

    public void AddCandy(int num)
    {
        candyItem.count += num;

        amountText.text = candyItem.count.ToString();
    }

    public void MisteryCandy()
    {
        // xText.gameObject.SetActive(false);
        // amountText.gameObject.SetActive(false);
        image.gameObject.SetActive(false);

        m_image.gameObject.SetActive(true);
    }
}
