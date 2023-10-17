using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class StandBuildObject : BuildObject
{
    public StandPoint[] standPoints;
    public Transform GetEmptyPoint() => standPoints.Where((n) => n.itemObject == null).FirstOrDefault().point;

    [SerializeField] CandyInventoryUI inventoryUI;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Vector3 materialOffset;

    [SerializeField] Transform[] customerQueueLine;
    [SerializeField] public CandyMachine currentMachine;
    [SerializeField] List<IdleCustomer> customerList = new List<IdleCustomer>();

    public int targetItemId;
    public int maxQueueCount = 3;

    public bool isReady = false;
    private bool pause = false;

    private TaskUtil.WhileTaskMethod checkPlayerItemWhileTask = null;
    private TaskUtil.WhileTaskMethod candyGiveDelay = null;

    private void Start()
    {
        if (ES3.KeyExists(Guid + "_items"))
        {
            var _items = ES3.Load<Item[]>(Guid + "_items");

            for (int i = 0; i < _items.Length; i++)
            {
                standPoints[i].itemObject = IdleManager.instance.GenerateItemObject(standPoints[i].point, targetItemId).GetComponentInChildren<ItemObject>();
                standPoints[i].itemObject.transform.localScale = Vector3.one * 0.2f;
            }

        }

        inventoryUI.Init(targetItemId, standPoints.Where((n) => n.itemObject != null).Count());
        OnChangeInventory(false);

        if (meshRenderer != null)
            meshRenderer.material.mainTextureOffset = materialOffset;
    }

    public void Init()
    {
        this.TaskWhile(0.5f, 0, CheckDistBetweenCustomer);
    }

    public override void Build(bool direct = false)
    {
        if (isReady)
            return;

        base.Build(direct);

        isReady = true;

        Init();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            checkPlayerItemWhileTask = this.TaskWhile(0.1f, 0, () => CheckPlayerItems(other.GetComponent<IdlePlayer>()));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            if (checkPlayerItemWhileTask != null)
            {
                checkPlayerItemWhileTask.Kill();
                checkPlayerItemWhileTask = null;
            }
        }
    }


    //이 전시대에 아이템 추가
    public void AddNewItem(ItemObject itemObject)
    {
        if (itemObject.GetItem.id != targetItemId)
        {
            Debug.LogError("해당 아이템의 id가 일치하지 않습니다.");
            return;
        }

        if (standPoints.Where((n) => n.itemObject == null).Count() <= 0)
        {
            Debug.LogError("남는 자리가 없습니다.");
            return;
        }

        foreach (var point in standPoints)
        {
            if (point.itemObject == null)
            {
                itemObject.Jump(point.point);

                point.itemObject = itemObject;
                break;
            }
        }

        OnChangeInventory(true);
        SaveStandData();
    }

    public void CheckDistBetweenCustomer()
    {
        if (!isReady || candyGiveDelay != null || pause)
            return;

        if (customerList.Count > 0)
            if (Vector3.Distance(customerList[0].transform.position, customerQueueLine[0].transform.position) < 1f)
                if (standPoints.Where((n) => n.itemObject != null).Count() > 0)
                    GiveCandyToCustomer(customerList[0]);
    }

    public void GiveCandyToCustomer(IdleCustomer customer)
    {
        candyGiveDelay = this.TaskWhile(0.2f, 0, () =>
        {
            if (customer.requestItemCount <= customer.currentItemCount)
            {
                customerList.Remove(customer);
                IdleManager.instance.counter.EnqueueCustomer(customer);
                UpdateLine();

                if (candyGiveDelay != null)
                {
                    candyGiveDelay.Kill();
                    candyGiveDelay = null;
                }
            }
            else
            {
                if (standPoints.Where((n) => n.itemObject != null).Count() > 0)
                {
                    ItemObject itemObject = null;

                    foreach (var point in standPoints)
                    {
                        if (point.itemObject != null)
                        {
                            itemObject = point.itemObject;
                            point.itemObject = null;
                            break;
                        }
                    }

                    if (itemObject == null)
                        return;

                    customer.AddItem(itemObject.gameObject);

                    OnChangeInventory(true);
                    EventManager.instance.CustomEvent(AnalyticsType.IDLE, "Candy Give_" + targetItemId, true, true);
                    customer.currentItemCount++;
                    customer.UpdateUI();

                    // if (punchTween != null ? !punchTween.IsPlaying() : true)
                    //     punchTween = transform.DOPunchScale(Vector3.one * 0.1f, 0.5f);
                }
            }

            SaveStandData();
        });
    }

    private void CheckPlayerItems(IdlePlayer player)
    {
        var emptyPoints = standPoints.Where((n) => n.itemObject == null).ToArray();

        if (emptyPoints.Length > 0)
            player.PopoutItem(targetItemId, emptyPoints[0].point, this);
    }

    public void SaveStandData()
    {
        ES3.Save<Item[]>(Guid + "_items", standPoints.Where((n) => n.itemObject != null).Select(point => point.itemObject.GetItem).ToArray());
    }

    public int GetItemCount() => standPoints.Where((n) => n.itemObject != null).Count();

    Tween CanvasWiggle = null;
    public void OnChangeInventory(bool wiggle = false)
    {
        inventoryUI.UpdateUI(targetItemId, GetItemCount(), true, wiggle: wiggle && ((CanvasWiggle != null) ? !CanvasWiggle.IsPlaying() : true));
    }

    public void UpdateLine()
    {
        for (int i = 0; i < customerList.Count; i++)
        {
            customerList[i].SetDestination(customerQueueLine[i].transform.position);
        }
    }

    public bool CheckHasQueue()
    {
        if (customerQueueLine.Length > customerList.Count)
            return true;
        else
            return false;
    }

    public bool IsEnableEnqueue() => maxQueueCount > customerList.Count;

    public void EnqueueCustomer(IdleCustomer newCustomer)
    {
        customerList.Add(newCustomer);
        newCustomer.itemId = targetItemId;
        newCustomer.UpdateUI();

        UpdateLine();
    }

    [ContextMenu("SwapDataFromDisplayStand")]
    public void SwapDataFromDisplayStand()
    {
        if (GetComponent<DisplayStand>() == null)
        {
            Debug.LogError("해당 컴포넌트가 없습니다.");
            return;
        }

        var displayStand = GetComponent<DisplayStand>();

        this.customerQueueLine = displayStand.GetCustomerQueueLine;
        this.customerList = displayStand.GetCustomerList;

        var array = displayStand.GetDisplayPoints.Select(point => point.Key).ToArray();

        this.standPoints = new StandPoint[array.Length];

        this.Guid = displayStand.Guid;
        this.targetItemId = displayStand.itemId;
        this.inventoryUI = displayStand.inventoryUI;

        for (int i = 0; i < array.Length; i++)
            this.standPoints[i] = new StandPoint(array[i]);
    }
}

[System.Serializable]
public class StandPoint
{
    public StandPoint(Transform point)
    {
        this.point = point;
    }

    public Transform point;
    public void SetPoint(Transform p) => point = p;
    public ItemObject itemObject;
    // public int itemId = -1;
}