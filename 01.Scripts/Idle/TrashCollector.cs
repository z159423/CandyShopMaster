using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TrashCollector : BuildObject
{
    TaskUtil.WhileTaskMethod collectorWhileTask = null;

    Tween groundTween = null;
    public GameObject groundObject;
    public Vector3 groundDefaultScale;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            collectorWhileTask = this.TaskWhile(0.2f, 0, () => TrashCollectTask(other.GetComponentInChildren<IdlePlayer>()));

            if (groundTween != null)
                groundTween.Kill();
            groundTween = groundObject.transform.DOScale(groundDefaultScale * 1.2f, 0.5f).SetEase(Ease.InOutBack);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            if (collectorWhileTask != null)
            {
                collectorWhileTask.Kill();
                collectorWhileTask = null;
            }

            if (groundTween != null)
                groundTween.Kill();
            groundTween = groundObject.transform.DOScale(groundDefaultScale, 0.5f).SetEase(Ease.InOutBack);
        }
    }

    void TrashCollectTask(IdlePlayer player)
    {
        player.PopoutAnyItem(transform);
    }
}
