using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class LolliPopMachine : BuildObject
{
    [SerializeField] Dictionary<Transform, ItemObject> ItemObjectStackPoint = new Dictionary<Transform, ItemObject>();

    public KeyValuePair<Transform, ItemObject> GetEmptyPoint()
    {
        foreach (var point in ItemObjectStackPoint)
        {
            if (point.Value == null)
                return point;
        }

        return new KeyValuePair<Transform, ItemObject>();
    }

    [SerializeField] Transform spawnPoint;

    [SerializeField] string itemName;

    [SerializeField] Animator animator;

    [SerializeField] Stack<Item> items = new Stack<Item>();

    bool taskReady = true;

    private void Start()
    {
        this.TaskWhile(1f, 0, GenearteItemTask);

        if (ES3.KeyExists(Guid + "_items"))
        {
            var _items = ES3.Load<Item[]>(Guid + "_items");

            _items.ToList().ForEach((n) => items.Push(n));

            foreach (var item in _items)
            {
                foreach (var point in ItemObjectStackPoint)
                {
                    if (point.Value == null)
                    {
                        ItemObjectStackPoint[point.Key] = IdleManager.instance.GenerateItemObject(point.Key, item.id).GetComponentInChildren<ItemObject>();
                        ItemObjectStackPoint[point.Key].transform.localScale = Vector3.one * 0.2f;
                        break;
                    }
                }
            }
        }
    }

    void GenearteItemTask()
    {
        if (ItemObjectStackPoint.Where((n) => n.Value == null).Count() > 0 && taskReady)
        {
            taskReady = false;

            animator.SetTrigger("Work");
        }
    }

    public void GenerateItem()
    {
        print("41241");
        var item = Instantiate(Resources.Load<GameObject>("Item/" + itemName), spawnPoint.transform.position, spawnPoint.transform.rotation).GetComponent<ItemObject>();

        item.Jump(GetEmptyPoint().Key, rotate: true);

        var emptyPoint = GetEmptyPoint();
        ItemObjectStackPoint[emptyPoint.Key] = item;

        items.Push(item.GetItem);

        taskReady = true;

        ES3.Save<Item[]>(Guid + "_items", items.ToArray());
    }
}
