using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class fieldRVProbs : MonoBehaviour
{
    [SerializeField] FieldRvType type;

    Coroutine collectCoroutine = null;

    bool isComplete = false;

    Tween groundTween = null;

    [SerializeField] GameObject groundObject;
    [SerializeField] Vector3 groundDefaultScale;

    public string pos = "";


    private void OnTriggerEnter(Collider other)
    {
        if (isComplete)
            return;

        if (other.tag.Equals("Player"))
        {
            if (collectCoroutine != null)
            {
                StopCoroutine(collectCoroutine);
                collectCoroutine = null;
            }

            if (groundTween != null)
                groundTween.Kill();
            groundTween = groundObject.transform.DOScale(groundDefaultScale * 1.35f, 0.5f).SetEase(Ease.InOutBack);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isComplete)
            return;

        if (other.tag.Equals("Player"))
        {
            if (collectCoroutine != null && other.GetComponentInChildren<PlayerMovement>().GetCurrentMoveSpeed() != 0)
            {
                StopCoroutine(collectCoroutine);
                collectCoroutine = null;
            }

            if (other.GetComponentInChildren<PlayerMovement>().GetCurrentMoveSpeed() == 0 && collectCoroutine == null)
                collectCoroutine = StartCoroutine(CollectCoroutine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isComplete)
            return;

        if (other.tag.Equals("Player"))
        {
            if (collectCoroutine != null)
            {
                StopCoroutine(collectCoroutine);
                collectCoroutine = null;
            }

            if (groundTween != null)
                groundTween.Kill();
            groundTween = groundObject.transform.DOScale(groundDefaultScale, 0.5f).SetEase(Ease.InOutBack);
        }
    }

    IEnumerator CollectCoroutine()
    {
        yield return new WaitForSeconds(1f);

        IdleManager.instance.GenerateFieldRVUI(type, () => Destroy(gameObject), pos);
        EventManager.instance.CustomEvent(AnalyticsType.UI, type.ToString() + "- OnActivefieldRV " + pos, true, true);
    }
}
