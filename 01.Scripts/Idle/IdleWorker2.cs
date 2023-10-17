using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Linq;

public class IdleWorker2 : SerializedMonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;


    [SerializeField] public Dictionary<Transform, ItemObject> itemPoints = new Dictionary<Transform, ItemObject>();

    [SerializeField] bool Working = false;
    private bool delivery = false;

    [SerializeField] ParticleSystem leftFootStepDust;
    [SerializeField] ParticleSystem rightFootStepDust;

    candySaveData data;

    private void Start()
    {
        this.TaskWhile(1f, 1f, FindJob);
    }

    private void Update()
    {
        if (agent.velocity.magnitude > 0.1f)
        {
            animator.SetBool("Move", true);
        }
        else
            animator.SetBool("Move", false);

        if (delivery)
            animator.SetLayerWeight(1, 1);
        else
            animator.SetLayerWeight(1, 0);
    }


    public void FindJob()
    {

        if (!agent.isOnNavMesh)
        {
            agent.enabled = false;
            agent.enabled = true;
        }

        if (Working || !agent.isOnNavMesh || !IdleManager.instance.playIdle)
            return;

        // print("finding");

        // var jobs = IdleManager.instance.candyDisplayStandList.Where((n) => n.isReady && n.GetEmptyPoint().Value == null && n.currentMachine.candyItem.count > 0
        //  && n.GetEmptyPoint().Key != null).ToArray();

         var jobs = IdleManager.instance.standBuildList.Where((n) => n.isReady &&
          n.standPoints.Where((n) => n.itemObject == null).Count() > 0 && n.currentMachine.candyItem.count > 0).ToArray();

        if (jobs.Length > 0)
        {
            Working = true;
            var job = jobs[Random.Range(0, jobs.Length)];

            agent.SetDestination(job.currentMachine.transform.position);

            this.TaskWaitUntil(() => job.currentMachine.GiveItemobjectToWorker(this, () =>
                {
                    DeliveryToStand(job);
                    delivery = true;
                },
                () => CancleJob()
            ), () => (Vector3.Distance(transform.position, job.currentMachine.transform.position) < 5f));
        }
    }

    public void DeliveryToStand(StandBuildObject stand)
    {
        agent.SetDestination(stand.transform.position);

        this.TaskWaitUntil(() =>
        {
            EndDelivery();

            var where = itemPoints.Where((n) => n.Value != null).ToArray();

            print(where.Count());

            for (int i = 0; i < where.Count(); i++)
            {
                if (where[i].Value != null)
                {
                    print(i);

                    var emptyPoint = stand.GetEmptyPoint();

                    if (emptyPoint == null)
                        Managers.Pool.Push(itemPoints[where[i].Key].GetComponentInChildren<Poolable>());
                    else
                        itemPoints[where[i].Key].Jump(emptyPoint);

                    stand.AddNewItem(where[i].Value);

                    itemPoints[where[i].Key] = null;
                }
            }

            // foreach (var point in where)
            // {
            //     if (point.Value != null)
            //     {
            //         if (stand.GetEmptyPoint().Key != null)
            //         {
            //             itemPoints[point.Key].Jump(stand.GetEmptyPoint().Key);
            //             stand.AddItemObject(point.Value.gameObject);
            //         }
            //         else
            //         {
            //             Managers.Pool.Push(point.Value.gameObject.GetComponentInChildren<Poolable>());
            //         }

            //         itemPoints[point.Key] = null;
            //     }
            // }

        }, () => (Vector3.Distance(stand.transform.position, transform.position) < 8f));
    }

    public void EndDelivery()
    {
        Working = false;
        delivery = false;
    }

    public void PlayLeftFootStepParticle()
    {
        leftFootStepDust.Play();
    }

    public void PlayRightFootStepParticle()
    {
        rightFootStepDust.Play();
    }

    public void CancleJob()
    {
        Working = false;
        delivery = false;
    }

    public void ChangeMoveSpeed(float speed)
    {
        agent.speed = speed;
    }

    public KeyValuePair<Transform, ItemObject> GetEmptyPoint()
    {
        foreach (var point in itemPoints)
        {
            if (point.Value == null)
                return point;
        }

        return default;
    }
}
