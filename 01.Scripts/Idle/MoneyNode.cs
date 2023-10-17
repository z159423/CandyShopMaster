using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoneyNode : MonoBehaviour
{
    public bool isReady = false;

    Sequence nodeTween = null;

    Vector3 localOrigin;

    private void Start()
    {
        localOrigin = transform.localPosition;
    }

    public void MoneyReady()
    {
        if (isReady)
            return;

        isReady = true;

        nodeTween.Append(transform.DOScale(new Vector3(2, 2, 2), 0.3f));
    }

    public void FlyToPlayer(Transform target)
    {
        isReady = false;

        nodeTween.Append(gameObject.transform.DOJump(target.position, 10f, 1, 0.3f).OnComplete(() => { transform.localScale = Vector3.zero; transform.localPosition = localOrigin;}));
    }
}
