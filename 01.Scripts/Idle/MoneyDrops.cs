using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public class MoneyDrops : SaveableObject
{
    [SerializeField] MoneyNode[] moneyNodes;

    Stack<MoneyNode> readyMoneyStack = new Stack<MoneyNode>();

    [SerializeField] int money = 0;

    Coroutine takeMoneyCoroutine = null;

    private void Start()
    {
        if (ES3.KeyExists(Guid + "currentMoney"))
        {
            money = ES3.Load<int>(Guid + "currentMoney");
        }

        var nodeCount = (money / 5);
        var remainCount = money % 5;

        if (remainCount > 0)
            nodeCount++;

        for (int i = 0; i < nodeCount; i++)
        {
            if (moneyNodes.Length > i)
            {
                moneyNodes[i].MoneyReady();
                readyMoneyStack.Push(moneyNodes[i]);
            }
        }
    }


    public void AddMoney(int value)
    {
        money += value;

        if (money > 0 && money <= 5)
        {
            moneyNodes[0].MoneyReady();
            readyMoneyStack.Push(moneyNodes[0]);
        }
        else if (money >= 5)
        {
            int remain = money % 5;

            for (int i = 0; i < (money / 5) + ((remain > 0) ? 1 : 0); i++)
            {
                if (moneyNodes.Length > i)
                {
                    moneyNodes[i].MoneyReady();
                    readyMoneyStack.Push(moneyNodes[i]);
                }
            }
        }

        ES3.Save<int>(Guid + "currentMoney", money);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            if (takeMoneyCoroutine == null)
                takeMoneyCoroutine = StartCoroutine(TakeMoneyCoroutine(other));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            if (takeMoneyCoroutine != null)
            {
                StopCoroutine(takeMoneyCoroutine);
                takeMoneyCoroutine = null;
            }
        }
    }

    IEnumerator TakeMoneyCoroutine(Collider other)
    {
        if (takeMoneyCoroutine != null)
            StopCoroutine(takeMoneyCoroutine);

        while (true)
        {
            yield return new WaitForSeconds(0.05f);

            if (money > 0)
            {
                if (readyMoneyStack.Count > 0)
                {
                    var node = readyMoneyStack.Pop();

                    node.FlyToPlayer(other.transform);
                }

                bool success = false;

                if (money < 5)
                {
                    SaveManager.instance.GetMoney(money);
                    other.GetComponent<PlayerMoneyText>().ChangeFloatingText(money);
                    money = 0;

                    if (ES3.KeyExists("NextStageEnable"))
                        if (ES3.Load<bool>("NextStageEnable") && IdleManager.instance.playIdle)
                            success = MondayOFF.AdsManager.ShowInterstitial();

                }
                else
                {
                    SaveManager.instance.GetMoney(5);
                    money -= 5;

                    other.GetComponent<PlayerMoneyText>().ChangeFloatingText(5);

                    if (ES3.KeyExists("NextStageEnable"))
                        if (ES3.Load<bool>("NextStageEnable") && IdleManager.instance.playIdle)
                            success = MondayOFF.AdsManager.ShowInterstitial();
                }

                if (success)
                    EventManager.instance.CustomEvent(AnalyticsType.ADS, "Idle_Interstital_GotMoney", true, true);

                ES3.Save<int>(Guid + "currentMoney", money);
            }
            else
            {
                if (readyMoneyStack.Count > 0)
                {
                    var node = readyMoneyStack.Pop();

                    node.FlyToPlayer(other.transform);
                }
            }

        }
    }

    [Button]
    public void AddMoneyFive()
    {
        AddMoney(5);
    }

}
