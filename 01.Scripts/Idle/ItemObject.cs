using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class ItemObject : MonoBehaviour
{
    [SerializeField] Item item;
    public Item GetItem => item;

    public void Init(Item item)
    {
        this.item = item;
    }

    public void Jump(Transform parent, float jumpPower = 3f, System.Action onComplete = null, bool rotate = false)
    {

        transform.DOLocalJump(Vector3.zero, jumpPower, 1, 0.3f).OnComplete(() => { if (onComplete != null) Managers.Pool.Push(GetComponent<Poolable>()); }).OnStart(() => gameObject.transform.SetParent(parent));
        if (rotate)
            transform.DOLocalRotate(Vector3.zero, 0.3f);
    }

    public void Move(Transform parent)
    {
        transform.DOLocalMove(Vector3.zero, 0.1f).OnStart(() => gameObject.transform.SetParent(parent));
    }
}
