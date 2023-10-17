using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using System.Linq;

public class CandyMachine : BuildObject
{
    public candySaveData candyItem;

    // [SerializeField] Canvas CandyCanvas;
    // [SerializeField] Text test_candyName;
    // [SerializeField] Image candyImage;
    // [SerializeField] Text test_candyCount;

    public CandyInventoryUI candyUI_A;
    public CandyInventoryUI candyUI_B;
    CandyInventoryUI currentCandyUI;

    [SerializeField] Transform candyDeco;

    public Transform[] customerQueueLine;

    public List<IdleCustomer> customerList = new List<IdleCustomer>();

    public bool isReady = false;

    public float Debug_distToCustomer;

    [HideInInspector] public TaskUtil.DelayTaskMethod delayTimer = null;
    [HideInInspector] public TaskUtil.DelayTaskMethod candyGiveDelay = null;

    //================================================================================

    [HideInInspector] public TaskUtil.WhileTaskMethod playerTakeCandyTask = null;
    public Transform itemObjectGeneratePoint;

    Tween punchScaleTween = null;

    public SkinnedMeshRenderer insideCandyMesh;


    public void Init()
    {
        candyItem = SaveManager.instance.FindCandyItem(candyItem.id);

        if (ES3.KeyExists("AB_Test"))
        {
            if (ES3.Load<string>("AB_Test").Equals("A"))
                currentCandyUI = candyUI_A;
            else if (ES3.Load<string>("AB_Test").Equals("B"))
                currentCandyUI = candyUI_B;
        }

        currentCandyUI.gameObject.SetActive(true);
        currentCandyUI.Init(candyItem.id, candyItem.count);

        // candyImage.sprite = SaveManager.instance.FindCandyObjectInReousrce(candyItem.id).icon;
        // test_candyCount.text = "X " + (candyItem.count);

        this.TaskWhile(1, 0, CheckDistBetweenCustomer);

        UpdateCandyDisplay();
    }

    public void EnqueueCustomer(IdleCustomer newCustomer)
    {
        customerList.Add(newCustomer);

        UpdateLine();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {

            this.playerTakeCandyTask = this.TaskWhile(0.2f, 0, () => ItemObjectMoveTask(other.GetComponentInChildren<IdlePlayer>()));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            if (this.playerTakeCandyTask != null)
                this.playerTakeCandyTask.Kill();
        }
    }

    public void CheckDistBetweenCustomer()
    {
        if (!isReady || candyGiveDelay != null)
            return;

        if (customerList.Count > 0)
        {
            Debug_distToCustomer = Vector3.Distance(customerList[0].transform.position, customerQueueLine[0].transform.position);

            if (Vector3.Distance(customerList[0].transform.position, customerQueueLine[0].transform.position) < 1f)
            {
                if (candyItem.count <= 0)
                {
                    if (delayTimer == null)
                        delayTimer = this.TaskDelay(2.5f, () =>
                        {
                            customerList[0].Exit();
                            delayTimer = null;
                            customerList[0].GenerateEmoji("Particles/Dispoint");
                            customerList.Remove(customerList[0]);

                            EventManager.instance.CustomEvent(AnalyticsType.IDLE, "Customer Dispoint_" + candyItem.id, true, true);

                            UpdateLine();

                            if (ES3.Load<bool>("NextStageEnable"))
                                IdleManager.instance.HighlightNextStageBtn();
                        });
                }
                else
                {
                    if (delayTimer != null)
                    {
                        delayTimer.Kill();
                        delayTimer = null;
                    }

                    GiveCandyToCustomer(customerList[0]);
                }
            }
        }
        else
            Debug_distToCustomer = 0;
    }

    public void GiveCandyToCustomer(IdleCustomer customer)
    {
        customer.SetTimer(2.5f);

        candyGiveDelay = this.TaskDelay(2.5f, () =>
        {
            IdleManager.instance.counter.EnqueueCustomer(customer);

            customer.candyInventory.candy = SaveManager.instance.FindCandyObjectInReousrce(candyItem.id);
            customer.candyInventory.count = 1;

            SaveManager.instance.TakeCandy(customer.candyInventory.candy.id, 1);

            customer.UpdateCandyJar();

            if (ES3.KeyExists("AB_Test"))
            {
                if (ES3.Load<string>("AB_Test").Equals("A"))
                    candyItem.TakeCandy(1);
            }

            customerList.Remove(customer);

            OnChangeInventory(true);

            transform.DOPunchScale(Vector3.one * 0.1f, 0.5f);

            UpdateLine();

            candyGiveDelay = null;

            EventManager.instance.CustomEvent(AnalyticsType.IDLE, "Candy Give_" + candyItem.id, true, true);

        });
    }

    public void UpdateLine()
    {
        for (int i = 0; i < customerList.Count; i++)
        {
            customerList[i].SetDestination(customerQueueLine[i].transform.position);
        }
    }

    public void OnChangeInventory(bool wiggle = false)
    {
        if (currentCandyUI != null)
            currentCandyUI.UpdateUI(candyItem.id, candyItem.count, wiggle: wiggle);

        // if (wiggle)
        //     CandyCanvas.transform.DOPunchScale(CandyCanvas.transform.localScale * 0.3f, 0.2f, 2);

        UpdateCandyDisplay();
    }

    public override void Build(bool direct = false)
    {
        base.Build(direct);

        isReady = true;

        Init();
    }

    public void UpdateUI(bool wiggle = false)
    {
        OnChangeInventory(wiggle);
        // test_candyCount.text = "X " + (candyItem.count);
    }

    public void UpdateCandyDisplay()
    {
        candyDeco.transform.DOMoveY(Mathf.Clamp(-6f + (candyItem.count * 0.3f), -6f, 0), 0.5f);

        if (insideCandyMesh != null)
        {
            insideCandyMesh.SetBlendShapeWeight(0, Mathf.Clamp(100f - (float)candyItem.count, 0, 100));
            insideCandyMesh.SetBlendShapeWeight(1, Mathf.Clamp(100f - (float)candyItem.count, 0, 100));
        }
    }

    public bool CheckHasQueue()
    {
        if (customerQueueLine.Length > customerList.Count)
            return true;
        else
            return false;
    }

    public void ItemObjectMoveTask(IdlePlayer player)
    {
        if (candyItem.count <= 0)
            return;

        var playerPoint = player.GetPlayerEmptyPoint();
        if (playerPoint != null)
        {
            var obj = IdleManager.instance.GenerateItemObject(itemObjectGeneratePoint, candyItem.id);

            obj.GetComponentInChildren<ItemObject>().Jump(playerPoint);

            SaveManager.instance.TakeCandy(candyItem.id, 1);

            OnChangeInventory(true);

            if (punchScaleTween != null ? !punchScaleTween.IsPlaying() : false)
                punchScaleTween = transform.DOPunchScale(Vector3.one * 0.1f, 0.20f);

            player.AddItemStack(obj);

            MMVibrationManager.Haptic(HapticTypes.MediumImpact);

            EventManager.instance.CustomEvent(AnalyticsType.IDLE, "Player Take Candy_" + candyItem.id, true, true);


            if (!ES3.KeyExists("FirstPlayerCandyTake"))
            {
                IdleManager.instance.idlePlayer.ActiveNaviArrow(IdleManager.instance.standBuildList.Where((n) => n.isReady).First().transform);
                ES3.Save<bool>("FirstPlayerCandyTake", true);
            }
        }
    }

    public void GiveItemobjectToWorker(IdleWorker2 worker, System.Action onComplete = null, System.Action onFailed = null)
    {
        if (candyItem.count <= 0)
        {
            if (onFailed != null)
            {
                onFailed.Invoke();
                return;
            }
        }

        for (int i = 0; i < IdleManager.instance.workerCapacityValue[IdleManager.instance.workerCapacity.currentLevel]; i++)
        {
            if (candyItem.count <= 0)
            {
                break;
            }

            var emptyPoint = worker.GetEmptyPoint();

            if (emptyPoint.Key == null)
            {
                if (i == 0)
                    onFailed.Invoke();
                else
                    onComplete.Invoke();

                return;
            }

            var obj = IdleManager.instance.GenerateItemObject(itemObjectGeneratePoint, candyItem.id);

            obj.GetComponentInChildren<ItemObject>().Jump(emptyPoint.Key);

            worker.itemPoints[emptyPoint.Key] = obj.GetComponentInChildren<ItemObject>();

            SaveManager.instance.TakeCandy(candyItem.id, 1);

            OnChangeInventory(true);

            if (punchScaleTween != null ? !punchScaleTween.IsPlaying() : true)
                punchScaleTween = transform.DOPunchScale(Vector3.one * 0.1f, 0.20f);

            EventManager.instance.CustomEvent(AnalyticsType.IDLE, "Worker Take Candy_" + candyItem.id, true, true);


            // return obj.GetComponentInChildren<ItemObject>();
        }

        if (onComplete != null)
            onComplete.Invoke();

        return;
    }
}
