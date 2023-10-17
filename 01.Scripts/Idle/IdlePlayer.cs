using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class IdlePlayer : MonoBehaviour
{
    [SerializeField] Transform[] itemStackPoints;

    public List<GameObject> itemStackList = new List<GameObject>();

    [SerializeField] GameObject playerArrow;
    [SerializeField] GameObject itemUICanvas;
    [SerializeField] Transform itemUISlotParent;

    [SerializeField] Transform startCollector;


    private Transform arrowTarget;
    private System.Action onReachedTarget = null;
    private List<ItemUISlot> itemSlots = new List<ItemUISlot>();

    public GameObject maxText;

    private void Start()
    {
        if (ES3.KeyExists("StartNav"))
        {
            if (ES3.Load<bool>("StartNav") == false)
                ActiveNaviArrow(startCollector, () => { ES3.Save<bool>("StartNav", true); });
        }
        else
        {
            ActiveNaviArrow(startCollector, () => { ES3.Save<bool>("StartNav", true); });
            ES3.Save<bool>("StartNav", false);
        }
    }

    public Transform GetPlayerEmptyPoint()
    {
        if (itemStackList.Count >= IdleManager.instance.playerCapacityValue[IdleManager.instance.playerCapacity.currentLevel])
            return null;

        if (itemStackList.Count < itemStackPoints.Length)
        {
            return itemStackPoints[itemStackList.Count];
        }
        else return null;
    }

    private void Update()
    {
        if (arrowTarget != null)
        {
            // Vector3 rotation = Quaternion.LookRotation(arrowTarget.transform.position).eulerAngles;
            // rotation.x = 0f;

            // playerArrow.transform.rotation = Quaternion.Euler(rotation);

            playerArrow.transform.LookAt(arrowTarget.transform.position + (Vector3.up * 5));
            // playerArrow.transform.rotation = offset;

            if (Vector3.Distance(transform.position, arrowTarget.transform.position) < 8f)
            {
                playerArrow.transform.DOScale(Vector3.zero, 0.5f);
                arrowTarget = null;

                if (onReachedTarget != null)
                {
                    onReachedTarget.Invoke();
                    onReachedTarget = null;
                }
            }
        }
    }

    public void AddItemStack(GameObject item)
    {
        itemStackList.Add(item);

        // UpdateItemUI();

        if (itemStackList.Count >= IdleManager.instance.playerCapacityValue[IdleManager.instance.playerCapacity.currentLevel])
            maxText.SetActive(true);
        else
            maxText.SetActive(false);
    }

    public void PopoutItem(int id, Transform parent, DisplayStand stand)
    {
        for (int i = 0; i < itemStackList.Count; i++)
        {
            if (itemStackList[i].GetComponent<ItemObject>().GetItem.id == id)
            {
                itemStackList[i].GetComponent<ItemObject>().Jump(parent);

                stand.AddItemObject(itemStackList[i]);

                itemStackList.Remove(itemStackList[i]);

                UpdateItemPos();

                // UpdateItemUI();
                return;
            }
        }
    }

    public void PopoutItem(int id, Transform parent, StandBuildObject stand)
    {
        for (int i = 0; i < itemStackList.Count; i++)
        {
            if (itemStackList[i].GetComponent<ItemObject>().GetItem.id == id)
            {
                itemStackList[i].GetComponent<ItemObject>().Jump(parent);

                stand.AddNewItem(itemStackList[i].GetComponent<ItemObject>());

                itemStackList.Remove(itemStackList[i]);

                UpdateItemPos();

                // UpdateItemUI();
                return;
            }
        }
    }

    public void PopoutAnyItem(Transform parent)
    {
        for (int i = 0; i < itemStackList.Count; i++)
        {
            EventManager.instance.CustomEvent(AnalyticsType.IDLE, "Player trash Candy_" + itemStackList[i].GetComponentInChildren<ItemObject>().GetItem.id, true, true);

            itemStackList[i].GetComponent<ItemObject>().Jump(parent, jumpPower: 1f, onComplete: () => { Managers.Pool.Push(itemStackList[i].GetComponentInChildren<Poolable>()); Debug.LogError(1234); });
            itemStackList.Remove(itemStackList[i]);

            UpdateItemPos();
        }
    }

    public void UpdateItemPos()
    {
        for (int i = 0; i < itemStackList.Count; i++)
        {
            itemStackList[i].GetComponent<ItemObject>().Move(itemStackPoints[i]);
        }

        if (itemStackList.Count >= IdleManager.instance.playerCapacityValue[IdleManager.instance.playerCapacity.currentLevel])
            maxText.SetActive(true);
        else
            maxText.SetActive(false);

    }

    public void ActiveNaviArrow(Transform target, System.Action onReachedTargetEvent = null)
    {
        arrowTarget = target;

        playerArrow.transform.DOScale(Vector3.one, 0.5f);

        if (onReachedTargetEvent != null)
            onReachedTarget = onReachedTargetEvent;
    }

    public void ActiveNaviArrow(Transform target)
    {
        arrowTarget = target;

        playerArrow.transform.DOScale(Vector3.one, 0.5f);
    }

    public void UpdateItemUI()
    {
        itemSlots.ForEach((n) => n.Clear());
        itemSlots.Clear();

        foreach (var item in itemStackList)
        {
            bool pass = false;
            itemSlots.ForEach((n) =>
            {
                if (item.GetComponentInChildren<ItemObject>().GetItem.id == n.item.id)
                {
                    n.AddItem(item.GetComponentInChildren<ItemObject>().GetItem.count);
                    return;
                }
            });

            if (!pass)
            {
                var newSlot = Managers.Pool.Pop(Resources.Load<GameObject>("UI/ItemSlot"), itemUISlotParent).GetComponentInChildren<ItemUISlot>();
                newSlot.Init(item.GetComponentInChildren<ItemObject>().GetItem);
                itemSlots.Add(newSlot);
            }
        }

        if (itemSlots.Count > 0)
            itemUICanvas.gameObject.SetActive(true);
        else
            itemUICanvas.gameObject.SetActive(false);
    }
}
