using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    // public List<CandyItem> candyInventory = new List<CandyItem>();
    public List<candySaveData> candyInventory = new List<candySaveData>();

    [SerializeField] int money;
    public int GetCurrentMoney => money;
    [SerializeField] int royalCandy;

    [SerializeField] bool enableShop = false;
    public bool GetEnableShop => enableShop;

    private List<Text> moneyTextList = new List<Text>();
    private List<CandyInventory> inventorys = new List<CandyInventory>();
    [SerializeField] private List<CandyUnlockStatus> candyUnlockStatuses = new List<CandyUnlockStatus>();
    public List<CandyUnlockStatus> GetCandyUnlockStatuses() => candyUnlockStatuses;

    public List<Text> royalCandyTextList = new List<Text>();


    public UnityEngine.Events.UnityEvent onMoneyChangeEvent = new UnityEngine.Events.UnityEvent();
    public UnityEngine.Events.UnityEvent onChangeCandyInventoryEvent = new UnityEngine.Events.UnityEvent();
    public UnityEngine.Events.UnityEvent onRoyalCandyChangeEvent = new UnityEngine.Events.UnityEvent();


    public bool enableCandyInventoryUIUpdate = true;

    [SerializeField] string USER_GUID;
    public string Get_USER_GUID => USER_GUID;

    public static SaveManager instance = null;

    private TaskUtil.DelayTaskMethod moneySaveDelayTask = null;


    private void Awake()
    {
        if (!PlayerPrefs.HasKey("ClearZero"))
        {
            MondayOFF.EventTracker.ClearStage(0);
            PlayerPrefs.SetInt("ClearZero", 1);
        }

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (ES3.KeyExists("Money"))
            money = ES3.Load<int>("Money");
        else
            money = 0;

        if (ES3.KeyExists("RoyalCandy"))
            royalCandy = ES3.Load<int>("RoyalCandy");
        else
            royalCandy = 0;

        if (ES3.KeyExists("CandyInventory"))
            candyInventory = ES3.Load<List<candySaveData>>("CandyInventory");

        if (ES3.KeyExists("USER_GUID"))
            USER_GUID = ES3.Load<string>("USER_GUID");
        else
        {
            var USER_GUID = System.Guid.NewGuid().ToString();
            ES3.Save<string>("USER_GUID", USER_GUID);
        }

        if (ES3.KeyExists("CandyUnlockStatus"))
            candyUnlockStatuses = ES3.Load<List<CandyUnlockStatus>>("CandyUnlockStatus");



    }

    private void Start()
    {
        OnChangeMoney();
        OnChangeRoyalCandy();

        // MondayOFF.AdsManager.Initialize();
        // MondayOFF.AdsManager.ShowBanner();

        MondayOFF.AdsManager.OnAfterInterstitial += () => { if (!ES3.KeyExists("IS_Showend")) ES3.Save<bool>("IS_Showend", true); };
        MondayOFF.IAPManager.OnAfterPurchase += (isSuccess) =>
            {
                EventManager.instance.CustomEvent(AnalyticsType.IAP, "NoAdsPurchase", true, true);
            };
    }

    public int GetMoney() => money;

    public void GetMoney(int value)
    {
        money += value;

        // ES3.Save<int>("Money", money);

        MoneySaveDelay();

        OnChangeMoney();
    }

    public void LossMoney(int value)
    {
        money -= value;

        ES3.Save<int>("Money", money);

        // MoneySaveDelay();

        OnChangeMoney();
    }
    public void OnChangeMoney()
    {
        moneyTextList.ForEach((n) => n.text = money.ToString());

        onMoneyChangeEvent.Invoke();
    }

    public void AddRoyalCandy(int value)
    {
        royalCandy += value;

        ES3.Save<int>("RoyalCandy", royalCandy);

        OnChangeRoyalCandy();
    }

    public void UseRoyalCandy(int value)
    {
        royalCandy -= value;

        ES3.Save<int>("RoyalCandy", royalCandy);

        OnChangeRoyalCandy();
    }

    public void OnChangeRoyalCandy()
    {
        royalCandyTextList.ForEach((n) => n.text = royalCandy.ToString());

        onRoyalCandyChangeEvent.Invoke();
    }


    public void OnChangeCandyInventory()
    {
        if (!enableCandyInventoryUIUpdate)
            return;

        onChangeCandyInventoryEvent.Invoke();
    }

    public void AddCandy(List<CandyItem> newCandys, bool uiUpdate = true)
    {
        foreach (var newCandy in newCandys)
        {
            bool isNewCandy = true;
            foreach (var candy in candyInventory)
            {
                if (newCandy.candy.id == candy.id)
                {
                    isNewCandy = false;

                    candy.count += newCandy.count;

                    break;
                }
            }

            if (isNewCandy)
            {
                candyInventory.Add(new candySaveData() { id = newCandy.candy.id, count = newCandy.count });
            }
        }

        ES3.Save<List<candySaveData>>("CandyInventory", candyInventory);

        // IdleManager.instance.CheckingCandyJar();

        if (uiUpdate)
            OnChangeCandyInventory();

        IdleManager.instance.CheckingCandyMachine();
    }

    public void TakeCandy(int id, int count)
    {
        for (int i = 0; i < candyInventory.Count; i++)
        {
            if (candyInventory[i].id == id)
            {
                candyInventory[i].TakeCandy(count);

                if (candyInventory[i].count <= 0)
                    candyInventory[i].count = 0;

                OnChangeCandyInventory();

                ES3.Save<List<candySaveData>>("CandyInventory", candyInventory);

                return;
            }
        }

        foreach (var candy in candyInventory)
        {
            if (candy.id == id)
            {
                candy.TakeCandy(count);
            }
        }

        OnChangeCandyInventory();

        ES3.Save<List<candySaveData>>("CandyInventory", candyInventory);
    }

    public candySaveData FindCandyItem(int id)
    {
        if (candyInventory.Find((n => n.id == id)) == null)
        {
            candyInventory.Add(new candySaveData() { id = id, count = 0 });
            return candyInventory.Find((n => n.id == id));
        }
        else
            return candyInventory.Find((n => n.id == id));
    }

    public CandyObject FindCandyObject(int id)
    {
        return Resources.LoadAll<CandyObject>("Candy").Where((n) => n.id == id).FirstOrDefault();
    }

    public void AddMoneyText(Text text)
    {
        moneyTextList.Add(text);

    }

    public void AddRoyalCandyText(Text text)
    {
        royalCandyTextList.Add(text);

        OnChangeRoyalCandy();
    }

    public void RemoveMoneyText(Text text)
    {
        moneyTextList.Remove(text);
    }

    public bool CheckPossibleUpgrade(int cost)
    {
        if (money >= cost)
            return true;
        else
            return false;
    }

    public bool CheckCandyExist(int id, int count = 1)
    {
        foreach (var candy in candyInventory)
        {
            if (candy.id == id)
                if (candy.count >= count)
                    return true;
                else
                    return false;
        }

        return false;
    }

    public CandyObject FindCandyObjectInReousrce(int id)
    {
        return Resources.LoadAll<CandyObject>("Candy").FirstOrDefault<CandyObject>((n) => n.id == id);
    }

    public void MoneySaveDelay()
    {
        if (moneySaveDelayTask == null)
        {
            // ES3.Save<int>("Money", money);
            Save();
        }
        else
        {
            moneySaveDelayTask.Kill();
            moneySaveDelayTask = null;
            Save();
        }

        void Save()
        {
            moneySaveDelayTask = this.TaskDelay(2, () => { ES3.Save<int>("Money", money); moneySaveDelayTask.Kill(); moneySaveDelayTask = null; });
        }
    }

    public void AddUnlockPoint(List<CandyItem> candyItems)
    {
        var currentUnlockStatus = candyUnlockStatuses.Where((n) => !n.unlocked).OrderBy((n) => n.id).ToArray();

        if (currentUnlockStatus.Length > 0)
        {
            float totalPoint = 0;

            candyItems.ForEach((n) => totalPoint += (n.candy.unlockPoint * n.count));

            while (totalPoint > 0 && currentUnlockStatus != null)
            {
                totalPoint = currentUnlockStatus[0].AddPercent(totalPoint);

                currentUnlockStatus[0] = candyUnlockStatuses.Where((n) => !n.unlocked).OrderBy((n) => n.id).ToArray()[0];
            }
        }

        SaveCandyUnlockStatus(candyUnlockStatuses);
    }

    public void AddCandyUnlockPercent(int id, float precent = 1f)
    {
        var list = candyUnlockStatuses.Where((n) => n.id == id).Where((n) => !n.unlocked).ToList();

        // if ()
    }

    public void SaveCandyUnlockStatus(List<CandyUnlockStatus> statuses)
    {
        candyUnlockStatuses = statuses;

        ES3.Save<List<CandyUnlockStatus>>("CandyUnlockStatus", candyUnlockStatuses);
    }
}

[System.Serializable]
public class TempCandyInventory
{
    public List<CandyItem> candyItems = new List<CandyItem>();

    public void AddCandy(CandyItem item)
    {
        foreach (var candy in candyItems)
        {
            if (candy.candy.id == item.candy.id)
            {
                candy.count += item.count;

                return;
            }
        }
        candyItems.Add(new CandyItem() { candy = item.candy, count = item.count });
    }
}

[System.Serializable]
public class candySaveData
{
    public int id;
    public int count;

    public candySaveData DuplicateCandy(int minCount, int maxCount)
    {
        int count = Random.Range(minCount, maxCount);

        // this.count -= count;

        count = Mathf.Clamp(count, 0, int.MaxValue);

        return new candySaveData() { id = this.id, count = count };
    }

    public void TakeCandy(int count)
    {
        this.count -= count;
    }
}

[System.Serializable]
public class CandyUnlockStatus
{
    public int id;
    public float percent = 0;
    public float goalPercent = 100;
    public bool unlocked = false;


    public float AddPercent(float p = 1f)
    {
        if (unlocked)
            return 0;

        percent += p;

        if (percent > goalPercent)
        {
            Unlock();

            return percent - goalPercent;
        }

        return 0;
    }

    public void Unlock()
    {
        unlocked = true;
    }

    public float GetCurrentPercent()
    {
        return Mathf.Clamp((percent / goalPercent) * 100f, 0, 100f);
    }
}
