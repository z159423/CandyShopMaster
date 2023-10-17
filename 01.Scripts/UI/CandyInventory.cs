using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class CandyInventory : MonoBehaviour
{
    [SerializeField] List<CandyInventoryItem> itemList = new List<CandyInventoryItem>();
    public List<CandyInventoryItem> GetItemList() => itemList;

    public bool autoUpdateUI = false;

    public static CandyInventory instance;

    private CandyInventoryItem misteryCandyItem;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            List<CandyItem> tempItem = new List<CandyItem>();

            var tempcandy1 = SaveManager.instance.FindCandyObjectInReousrce(1);
            tempItem.Add(new CandyItem() { candy = tempcandy1, count = 105 });

            foreach (var item in tempItem)
            {
                var attractor = Instantiate(Resources.Load<GameObject>("UI/UIAttractor"), transform.parent);

                GetCandyInventoryEvent(item.candy.id);

                attractor.GetComponent<UIAttractorCustom>().Init(itemList.Find((n) => n.candyItem.candy.id == item.candy.id).GetImageTrans, item, GetCandyInventoryEvent(item.candy.id), () => { SyncCurrentCandyUI(); misteryCandyItem.transform.SetAsLastSibling(); misteryCandyItem.gameObject.SetActive(true); });
            }
        }
    }

    private void Start()
    {
        if (autoUpdateUI)
            SaveManager.instance.onChangeCandyInventoryEvent.AddListener(SyncCurrentCandyUI);
    }

    private void OnDisable()
    {
        SaveManager.instance.onChangeCandyInventoryEvent.RemoveListener(SyncCurrentCandyUI);
    }

    public void SyncCurrentCandyUI()
    {
        ClearUI();

        foreach (var candy in SaveManager.instance.candyInventory)
        {
            var newItem = Instantiate(Resources.Load<GameObject>("UI/CandyItem"), transform).GetComponent<CandyInventoryItem>();

            newItem.InitCandy(SaveManager.instance.FindCandyObjectInReousrce(candy.id), candy.count);
            itemList.Add(newItem);
        }

        if (misteryCandyItem == null)
            misteryCandyItem = Instantiate(Resources.Load<GameObject>("UI/CandyItem"), transform).GetComponent<CandyInventoryItem>();
        misteryCandyItem.MisteryCandy();
    }

    public void GenerateUIfromList(List<CandyItem> items)
    {
        foreach (var item in items)
        {
            var newItem = Instantiate(Resources.Load<GameObject>("UI/CandyItem"), transform).GetComponent<CandyInventoryItem>();

            newItem.InitCandy(item.candy, item.count, false);
            itemList.Add(newItem);
        }
    }

    public void CandyGetAnimation(List<CandyItem> candyItems)
    {
        if (misteryCandyItem != null)
            misteryCandyItem.gameObject.SetActive(false);

        // print(candyItems.Count);


        StartCoroutine(coroutin());

        IEnumerator coroutin()
        {
            for (int i = 0; i < candyItems.Count; i++)
            {
                yield return new WaitForSeconds(0.05f);
                GetCandyInventoryEvent(candyItems[i].candy.id);

                RunManager.instance.uIAttractorCustoms[i].Init(itemList.Find((n) => n.candyItem.candy.id == candyItems[i].candy.id).GetImageTrans, candyItems[i], GetCandyInventoryEvent(candyItems[i].candy.id), () => { SyncCurrentCandyUI(); misteryCandyItem.transform.SetAsLastSibling(); misteryCandyItem.gameObject.SetActive(true); });
            }
        }

        // foreach (var item in candyItems)
        // {
        //     // print(item.candy.id + " " + item.count);

        //     var attractor = Instantiate(Resources.Load<GameObject>("UI/UIAttractor"), transform.parent);

        //     GetCandyInventoryEvent(item.candy.id);

        //     attractor.GetComponent<UIAttractorCustom>().Init(itemList.Find((n) => n.candyItem.candy.id == item.candy.id).GetImageTrans, item, GetCandyInventoryEvent(item.candy.id), () => { SyncCurrentCandyUI(); misteryCandyItem.transform.SetAsLastSibling(); misteryCandyItem.gameObject.SetActive(true); });
        // }
    }

    public void ClearUI()
    {
        itemList.ForEach((n) => Destroy(n.gameObject));

        itemList.Clear();
    }

    UnityAction GetCandyInventoryEvent(int id)
    {
        foreach (var item in itemList)
        {
            if (item.candyItem.candy.id == id)
            {
                return () => item.AddCandy(1);
            }
        }

        var newItem = Instantiate(Resources.Load<GameObject>("UI/CandyItem"), transform).GetComponentInChildren<CandyInventoryItem>();

        newItem.InitCandy(SaveManager.instance.FindCandyObject(id), 0);
        itemList.Add(newItem);

        return () => newItem.AddCandy(5);
    }
}
