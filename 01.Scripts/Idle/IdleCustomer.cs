using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

public class IdleCustomer : SerializedMonoBehaviour
{
    public OrderLine line;
    public CandyOrder order;
    public CandyItem candyInventory;

    private Vector3 targetPos;
    private System.Action nextAction = null;
    private Transform spawnPoint;

    [SerializeField] Canvas CandyCanvas;
    [SerializeField] Dictionary<Transform, ItemObject> itemPoints = new Dictionary<Transform, ItemObject>();
    [SerializeField] CandyJar candyJar;
    [SerializeField] GameObject[] skin;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] Animator animator;
    [SerializeField] Vector3 destination;
    [SerializeField] UnityEngine.UI.Image timer;
    [SerializeField] ParticleSystem leftFootStepDust;
    [SerializeField] ParticleSystem rightFootStepDust;
    [SerializeField] CandyInventoryUI candyInventoryUI;

    public bool waitForCandy = false;

    public int itemId = 0;
    public int requestItemCount = 1;
    public int currentItemCount = 0;

    public int CalculateTotalCost()
    {
        int cost = 0;

        foreach (var item in itemPoints)
        {
            if (item.Value != null)
                cost += item.Value.GetItem.CalculateTotalCost();
        }

        return cost;
    }

    public void Init(Transform spawnPoint, int id = 0)
    {
        this.spawnPoint = spawnPoint;
        agent.enabled = true;

        RandomSkin();

        requestItemCount = Random.Range(1, 4);

        itemId = id;

        UpdateUI();

        animator.enabled = false;
        animator.enabled = true;
    }

    private void Update()
    {
        if (nextAction != null)
            if (agent.remainingDistance < 0.1f)
            {
                nextAction.Invoke();
                nextAction = null;
            }

        if (agent.velocity.magnitude > 0.1f)
        {
            animator.SetBool("Move", true);
        }
        else
            animator.SetBool("Move", false);

        // if (candyInventory.count > 0)
        // {
        //     print(1);
        //     animator.SetLayerWeight(1, 1);
        // }
        // else
        // {
        //     animator.SetLayerWeight(1, 0);
        // }
    }

    public void SetDestination(Vector3 pos, System.Action onComplete = null)
    {
        agent.SetDestination(pos);
        if (onComplete != null)
            this.TaskWaitUntil(() => { onComplete.Invoke(); animator.SetBool("Move", false); }, () => (agent.remainingDistance < 0.2f));

        destination = pos;
        // this.nextAction = onComplete;
    }

    // public void WaitUntilCandy()
    // {

    //     waitForCandy = true;

    //     OnChangeOrder();

    //     CandyCanvas.transform.DOScale(new Vector3(0.004f, 0.004f, 0.004f), 0.4f);

    //     // this.TaskWaitUntil(() => Exit(), () => (order.currentCount >= order.requestCount));
    //     // if (order.currentCount >= order.requestCount)
    //     //     Exit();
    // }

    // public void AddCandyToOrder(CandyItem item)
    // {
    //     animator.SetBool("Move", false);

    //     if (order.candy.id == item.candy.id)
    //     {
    //         order.currentCount += item.count;

    //         OnChangeOrder(true);

    //         if (order.currentCount >= order.requestCount)
    //             Exit();
    //     }
    // }

    public void Exit()
    {
        animator.SetBool("Move", true);

        waitForCandy = false;
        line.currentCustomer = null;

        CandyCanvas.gameObject.SetActive(false);
        SetDestination(spawnPoint.position, () => { IdleManager.instance.ExitCustomer(transform.root.gameObject); Destroy(transform.root.gameObject); });

        // candyJar.ChangeJarModel(order.candy.id);
        // candyJar.gameObject.SetActive(true);

        // SaveManager.instance.GetMoney(order.CalculateTotalCost());


        // var particle = Managers.Pool.Pop(Managers.Resource.Load<GameObject>("Particles/DollarbillDirectional Large"));

        // particle.transform.position = transform.position + (Vector3.up * 2);
        // particle.GetComponentInChildren<ParticleSystem>().Play();

        // this.TaskDelay(5, () => Managers.Pool.Push(particle.GetComponentInParent<Poolable>()));

        // Managers.Pool.Push(transform.GetComponentInParent<Poolable>()
    }

    // void OnChangeOrder(bool wiggle = false)
    // {
    //     // test_candyName.text = order.candy.name;
    //     candyImage.sprite = order.candy.icon;
    //     test_candyCount.text = "X " + (order.requestCount - order.currentCount);

    //     if (wiggle)
    //         CandyCanvas.transform.DOPunchScale(CandyCanvas.transform.localScale * 0.3f, 0.2f, 2);
    // }


    void RandomSkin()
    {
        skin[Random.Range(0, skin.Length)].SetActive(true);
    }

    public void CheckComplete()
    {

    }

    public void UpdateCandyJar()
    {
        candyJar.ChangeJarModel(candyInventory.candy.id);
        candyJar.gameObject.SetActive(true);

    }


    public void GenerateEmoji(string path)
    {
        var emojis = Resources.LoadAll<GameObject>(path);

        var emoji = Managers.Pool.Pop(emojis[Random.Range(0, emojis.Length)], transform);
        emoji.transform.localPosition = Vector3.up * 10;

        emoji.GetComponent<ParticleSystem>().Play();

        this.TaskDelay(5f, () => Managers.Pool.Push(emoji));
    }

    public void SetTimer(float time)
    {
        timer.enabled = true;
        timer.fillAmount = 0;
        timer.DOFillAmount(1, time).OnComplete(() => timer.enabled = false);
    }

    public void AddItem(GameObject item)
    {
        foreach (var point in itemPoints)
        {
            if (point.Value == null)
            {
                item.GetComponentInChildren<ItemObject>().Jump(point.Key);
                itemPoints[point.Key] = item.GetComponentInChildren<ItemObject>();

                animator.SetLayerWeight(1, 1);


                break;
            }
        }

        UpdateUI();
    }

    public void GenerateOrder(Item item)
    {

    }

    public void PlayLeftFootStepParticle()
    {
        // leftFootStepDust.Play();
    }

    public void PlayRightFootStepParticle()
    {
        // rightFootStepDust.Play();
    }

    public void UpdateUI()
    {
        if (requestItemCount <= currentItemCount || itemId == 0)
            candyInventoryUI.gameObject.SetActive(false);
        else
        {
            candyInventoryUI.gameObject.SetActive(true);
            candyInventoryUI.UpdateUI(itemId, requestItemCount - currentItemCount, true, false);
        }
    }
}

public enum CustomerStateus
{

}
