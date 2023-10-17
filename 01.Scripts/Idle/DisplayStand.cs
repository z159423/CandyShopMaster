using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;

public class DisplayStand : BuildObject
{
    [SerializeField] Dictionary<Transform, ItemObject> displayPoints = new Dictionary<Transform, ItemObject>();
    public Dictionary<Transform, ItemObject> GetDisplayPoints => displayPoints;

    public KeyValuePair<Transform, ItemObject> GetEmptyPoint()
    {
        foreach (var point in displayPoints)
        {
            if (point.Value == null)
                return point;
        }

        return new KeyValuePair<Transform, ItemObject>();
    }

    public KeyValuePair<Transform, ItemObject> GetItemObject(bool removeFromList = false)
    {
        foreach (var point in displayPoints)
        {
            if (point.Value != null)
            {
                if (removeFromList)
                    displayPoints[point.Key] = null;

                return point;
            }
        }

        return new KeyValuePair<Transform, ItemObject>();
    }

    public KeyValuePair<Transform, ItemObject> FindEmptyPoint() => displayPoints.Where((n) => n.Value == null).First();
    [SerializeField] Stack<Item> items = new Stack<Item>();

    [SerializeField] Transform[] customerQueueLine;
    public Transform[] GetCustomerQueueLine => customerQueueLine;
    [SerializeField] List<IdleCustomer> customerList = new List<IdleCustomer>();
    public List<IdleCustomer> GetCustomerList => customerList;
    public int GetCustomerCount => customerList.Count;
    public bool IsEnableEnqueue() => maxQueueCount > customerList.Count;

    [SerializeField] public int itemId;

    public bool isReady = false;
    public int maxQueueCount = 3;

    private TaskUtil.WhileTaskMethod checkPlayerItemWhileTask = null;
    // private TaskUtil.WhileTaskMethod checkDistBetweenCustomer = null;


    TaskUtil.WhileTaskMethod candyGiveDelay = null;

    public float Debug_distToCustomer;

    [SerializeField]public CandyInventoryUI inventoryUI;

    // [SerializeField] Canvas CandyCanvas;
    // [SerializeField] Text test_candyName;
    // [SerializeField] Image candyImage;
    // [SerializeField] Text test_candyCount;

    private bool pause = false;

    [SerializeField] public CandyMachine currentMachine;

    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Vector3 materialOffset;

    Tween punchTween = null;


    private void Start()
    {
        if (ES3.KeyExists(Guid + "_items"))
        {
            var _items = ES3.Load<Item[]>(Guid + "_items");

            _items.ToList().ForEach((n) => items.Push(n));

            foreach (var item in _items)
            {
                foreach (var point in displayPoints)
                {
                    if (point.Value == null)
                    {
                        displayPoints[point.Key] = IdleManager.instance.GenerateItemObject(point.Key, item.id).GetComponentInChildren<ItemObject>();
                        displayPoints[point.Key].transform.localScale = Vector3.one * 0.2f;
                        break;
                    }
                }
            }
        }

        inventoryUI.Init(itemId, items.Count);

        meshRenderer.material.mainTextureOffset = materialOffset;

    }

    public void Init()
    {
        this.TaskWhile(0.5f, 0, CheckDistBetweenCustomer);
    }

    public void AddItemObject(GameObject itemObj)
    {
        var point = GetEmptyPoint();
        if (point.Key == null)
        {
            List<CandyItem> temp = new List<CandyItem>();

            var tempcandy1 = SaveManager.instance.FindCandyObjectInReousrce(itemObj.GetComponentInChildren<ItemObject>().GetItem.id);

            temp.Add(new CandyItem() { candy = tempcandy1, count = 1 });

            SaveManager.instance.AddCandy(temp, true);
            Debug.Log("자리가 없습니다 해당 아이템을 다시 창고로 이동");
            return;
        }

        foreach (var _point in displayPoints)
        {
            if (_point.Value == null)
            {
                displayPoints[_point.Key] = itemObj.GetComponentInChildren<ItemObject>();

                items.Push(displayPoints[_point.Key].GetItem);

                ES3.Save<Item[]>(Guid + "_items", items.ToArray());

                OnChangeInventory(true);
                return;
            }
        }
    }

    public void PopoutItemObject(Transform parent)
    {
        Transform findKey = null;

        foreach (var point in displayPoints)
        {
            if (point.Value != null)
            {
                point.Value.Jump(parent);
                findKey = point.Key;
            }
        }

        if (findKey != null)
            displayPoints.Remove(findKey);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            checkPlayerItemWhileTask = this.TaskWhile(0.25f, 0, () => CheckPlayerItems(other.GetComponent<IdlePlayer>()));
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

    private void CheckPlayerItems(IdlePlayer player)
    {
        if (GetEmptyPoint().Key != null)
            player.PopoutItem(itemId, GetEmptyPoint().Key, this);
    }

    void GenerateItemObject()
    {

    }

    public override void Build(bool direct = false)
    {
        if (isReady)
            return;

        base.Build(direct);

        isReady = true;

        Init();
    }

    public bool CheckHasQueue()
    {
        if (customerQueueLine.Length > customerList.Count)
            return true;
        else
            return false;
    }

    public void EnqueueCustomer(IdleCustomer newCustomer)
    {
        customerList.Add(newCustomer);
        newCustomer.itemId = itemId;

        newCustomer.UpdateUI();
        UpdateLine();
    }

    public void CheckDistBetweenCustomer()
    {
        if (!isReady || candyGiveDelay != null || pause)
            return;

        if (customerList.Count > 0)
        {
            Debug_distToCustomer = Vector3.Distance(customerList[0].transform.position, customerQueueLine[0].transform.position);

            if (Vector3.Distance(customerList[0].transform.position, customerQueueLine[0].transform.position) < 1f)
            {
                if (items.Count <= 0)
                {
                    // pause = true;

                    // this.TaskDelay(2.5f, () =>
                    // {
                    //     customerList[0].Exit();
                    //     // delayTimer = null;
                    //     customerList[0].GenerateEmoji("Particles/Dispoint");
                    //     customerList.Remove(customerList[0]);

                    //     EventManager.instance.CustomEvent(AnalyticsType.IDLE, "Customer Dispoint_" + itemId, true, true);

                    //     UpdateLine();

                    //     //     if (ES3.Load<bool>("NextStageEnable") == )
                    //     //         IdleManager.instance.HighlightNextStageBtn();

                    //     pause = false;
                    // });
                }
                else
                {
                    // if (delayTimer != null)
                    // {
                    //     delayTimer.Kill();
                    //     delayTimer = null;
                    // }

                    GiveCandyToCustomer(customerList[0]);
                }
            }
        }
        else
            Debug_distToCustomer = 0;
    }

    public void GiveCandyToCustomer(IdleCustomer customer)
    {
        // customer.SetTimer(2.5f);

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
                if (items.Count > 0)
                {
                    var itemObject = GetItemObject(true);
                    customer.AddItem(itemObject.Value.gameObject);

                    items.Pop();

                    OnChangeInventory(true);
                    if (punchTween != null ? !punchTween.IsPlaying() : true)
                        punchTween = transform.DOPunchScale(Vector3.one * 0.1f, 0.5f);
                    EventManager.instance.CustomEvent(AnalyticsType.IDLE, "Candy Give_" + itemId, true, true);
                    customer.currentItemCount++;
                    customer.UpdateUI();
                }
            }

            ES3.Save<Item[]>(Guid + "_items", items.ToArray());

            // customer.candyInventory.candy = SaveManager.instance.FindCandyObjectInReousrce(itemId);
            // customer.candyInventory.count = 1;

            // SaveManager.instance.TakeCandy(customer.candyInventory.candy.id, 1);

            // customer.UpdateCandyJar();

            // candyItem.TakeCandy(1);
        });
    }

    Tween CanvasWiggle = null;
    public void OnChangeInventory(bool wiggle = false)
    {
        inventoryUI.UpdateUI(itemId, items.Count, true, wiggle: wiggle && ((CanvasWiggle != null) ? !CanvasWiggle.IsPlaying() : true));

        // if (wiggle && (CanvasWiggle == null) ? true : !CanvasWiggle.IsPlaying())
        // {
        //     print(1234);
        //     CanvasWiggle = CandyCanvas.transform.DOPunchScale(CandyCanvas.transform.localScale * 0.3f, 0.2f, 2).OnComplete(() => CanvasWiggle = null);
        // }
    }

    public void UpdateLine()
    {
        for (int i = 0; i < customerList.Count; i++)
        {
            customerList[i].SetDestination(customerQueueLine[i].transform.position);
        }
    }
}
