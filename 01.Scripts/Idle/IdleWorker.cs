using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IdleWorker : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;

    [SerializeField] private CandyOrder currentOrder = null;
    [SerializeField]
    private CandyJar candyJar;
    [SerializeField] Animator animator;


    private CandyItem workerInventory;

    public WorkerStatus currentWorkerStatus;


    private void Start()
    {
        ChangeMoveSpeed(IdleManager.instance.workerSpeed[IdleManager.instance.workerSpeedUp.currentLevel]);
    }

    public void ChangeMoveSpeed(float speed)
    {
        agent.speed = speed;
    }

    private void Update()
    {
        WorkerStateMachine();
    }

    void WorkerStateMachine()
    {
        switch (currentWorkerStatus)
        {
            case WorkerStatus.Wait:
                WaitNextOrder();
                break;

            case WorkerStatus.MoveToCandy:

                break;

            case WorkerStatus.MoveToCustomer:

                break;
        }
    }

    void WaitNextOrder()
    {
        var line = IdleManager.instance.FindEmptyOrderLine_Worker();
        if (line != null)
        {
            MoveToCandy(line);
        }
    }

    void MoveToCandy(OrderLine line)
    {
        if (!SaveManager.instance.CheckCandyExist(line.currentCustomer.order.candy.id))
            return;

        animator.SetBool("Move", true);

        line.currentWorker = this;

        currentOrder = line.currentCustomer.order;

        var targetCandy = IdleManager.instance.FindCandyJar(line.currentCustomer.order.candy.id);

        agent.SetDestination(targetCandy.transform.position);

        currentWorkerStatus = WorkerStatus.MoveToCandy;

        this.TaskWaitUntil(() =>
        {
            MoveToCustomer(line);
            TakeCandy();
            targetCandy.BubbleWiggle();
            targetCandy.UpdateUI();
        }, () => (agent.remainingDistance < 0.1f));

        void TakeCandy()
        {
            SaveManager.instance.TakeCandy(line.currentCustomer.order.candy.id, 1);

            IdleManager.instance.OnChangeInventory();
        }
    }

    void MoveToCustomer(OrderLine line)
    {
        if (!SaveManager.instance.CheckCandyExist(line.currentCustomer.order.candy.id))
        {
            currentWorkerStatus = WorkerStatus.Wait;
            return;
        }

        workerInventory = new CandyItem() { candy = line.currentCustomer.order.candy, count = 1 };

        agent.SetDestination(line.workerLine.position);

        currentWorkerStatus = WorkerStatus.MoveToCustomer;

        candyJar.ChangeJarModel(workerInventory.candy.id);
        candyJar.gameObject.SetActive(true);

        this.TaskWaitUntil(() => CompleteDelivery(line), () => (agent.remainingDistance < 0.1f));
    }

    void CompleteDelivery(OrderLine line)
    {

        animator.SetBool("Move", false);

        // line.currentCustomer.AddCandyToOrder(workerInventory);

        line.currentWorker = null;

        candyJar.gameObject.SetActive(false);

        currentWorkerStatus = WorkerStatus.Wait;
    }

}

public enum WorkerStatus
{
    Wait = 1,
    MoveToCandy = 2,
    MoveToCustomer = 3
}
