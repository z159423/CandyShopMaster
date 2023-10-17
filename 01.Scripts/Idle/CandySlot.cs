using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CandySlot : BuildObject
{
    public Transform[] customerQueueLine;

    public List<IdleCustomer> customerList = new List<IdleCustomer>();

    [SerializeField] MoneyDrops moneyTower;

    public bool isReady = false;

    public float Debug_distToCustomer;

    public int level = 1;

    public int maxQueueCount = 3;
    public bool IsEnableEnqueue() => maxQueueCount > customerList.Count;


    TaskUtil.DelayTaskMethod candyGiveDelay = null;

    public void Init()
    {
        this.TaskWhile(1, 0, CheckDistBetweenCustomer);
    }

    public void EnqueueCustomer(IdleCustomer newCustomer)
    {
        customerList.Add(newCustomer);

        UpdateLine();
    }

    public void CheckDistBetweenCustomer()
    {
        if (!isReady || candyGiveDelay != null)
            return;

        if (customerList.Count > 0)
        {
            Debug_distToCustomer = Vector3.Distance(customerList[0].transform.position, customerQueueLine[0].transform.position);

            if (Vector3.Distance(customerList[0].transform.position, customerQueueLine[0].transform.position) < 1.5f)
            {
                customerList[0].SetTimer(5f);
                candyGiveDelay = this.TaskDelay(5f, () => { GiveCandyToCustomer(customerList[0]); });

                EventManager.instance.CustomEvent(AnalyticsType.IDLE, "Candy Slot Use" + Guid, true, true);
            }
        }
        else
            Debug_distToCustomer = 0;
    }

    public void GiveCandyToCustomer(IdleCustomer customer)
    {
        // IdleManager.instance.counter.EnqueueCustomer(customer);

        customer.candyInventory.candy = SaveManager.instance.FindCandyObjectInReousrce(Random.Range(1, 6));
        customer.candyInventory.count = 1;

        // SaveManager.instance.TakeCandy(customer.candyInventory.candy.id, 1);

        customer.Exit();

        customer.UpdateCandyJar();

        // candyItem.TakeCandy(1);

        customerList.Remove(customer);

        moneyTower.AddMoney((int)((level * 5) * IdleManager.instance.extraIncomePercent[IdleManager.instance.extraIncome.currentLevel]));

        transform.DOPunchScale(Vector3.one * 0.1f, 0.5f);

        UpdateLine();

        candyGiveDelay = null;

    }

    public void UpdateLine()
    {
        for (int i = 0; i < customerList.Count; i++)
        {
            customerList[i].SetDestination(customerQueueLine[i].transform.position);
        }
    }

    public override void Build(bool direct = false)
    {
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
}
