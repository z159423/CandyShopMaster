using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;


public abstract class IdleUseableBuildObject : BuildObject
{

    [SerializeField] Transform[] customerQueueLine;
    [SerializeField] List<IdleCustomer> customerList = new List<IdleCustomer>();

    public bool isReady = false;
    public int maxQueueCount = 3;

    public virtual void UpdateLine()
    {
        for (int i = 0; i < customerList.Count; i++)
        {
            customerList[i].SetDestination(customerQueueLine[i].transform.position);
        }
    }

    public void Init()
    {
        this.TaskWhile(1, 0, CheckDistBetweenCustomer);
    }

    public override void Build(bool direct = false)
    {
        if (isReady)
            return;

        base.Build(direct);

        isReady = true;

        Init();
    }


    public abstract void CheckDistBetweenCustomer();
}
