using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class LolliPopStand : MonoBehaviour
{
    [SerializeField] Dictionary<Transform, ItemObject> displayPoints = new Dictionary<Transform, ItemObject>();

    // private TaskUtil.WhileTaskMethod checkPlayerItemWhileTask = null;

    [SerializeField] public int itemId;

    


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

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.tag.Equals("Player"))
    //     {
    //         checkPlayerItemWhileTask = this.TaskWhile(0.25f, 0, () => CheckPlayerItems(other.GetComponent<IdlePlayer>()));
    //     }
    // }

    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.tag.Equals("Player"))
    //     {
    //         if (checkPlayerItemWhileTask != null)
    //         {
    //             checkPlayerItemWhileTask.Kill();
    //             checkPlayerItemWhileTask = null;
    //         }
    //     }
    // }

    // private void CheckPlayerItems(IdlePlayer player)
    // {
    //     if (GetEmptyPoint().Key != null)
    //         player.PopoutItem(itemId, GetEmptyPoint().Key, this);
    // }
}
