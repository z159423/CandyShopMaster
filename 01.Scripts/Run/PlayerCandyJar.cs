using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class PlayerCandyJar : MonoBehaviour
{
    [SerializeField] GameObject jar;
    [SerializeField] GameObject cork;
    [SerializeField] Transform candySpawnPoint;
    [SerializeField] Transform corkMoveEndPoint;

    [SerializeField] Transform[] candyStackPoints;

    public int stackCount = 0;

    public void InitJar()
    {
        jar.transform.DOScale(new Vector3(0.25f, 0.25f, 0.25f), 0.25f).OnComplete(() => RunManager.instance.enableCandyStack = true);
    }

    public void StackCandy(GameObject ob)
    {
        var candy = Instantiate(Resources.Load<GameObject>("StackCandy"), candySpawnPoint.transform.position, Quaternion.identity, candyStackPoints[stackCount]);

        candy.transform.localScale = ob.transform.localScale * 2.5f;
        candy.GetComponent<MeshRenderer>().materials = ob.GetComponentInChildren<MeshRenderer>(true).materials;
        candy.GetComponent<MeshFilter>().mesh = ob.GetComponentInChildren<MeshFilter>(true).mesh;

        candy.transform.DOLocalJump(Vector3.zero, 3f, 1, 0.35f);
        candy.transform.DOLocalRotate(candyStackPoints[stackCount].rotation.eulerAngles, 0.35f).OnComplete(() => candy.GetComponent<Rigidbody>().isKinematic = true);
        stackCount++;

        if (stackCount >= RunManager.instance.maxCandyStackCount)
        {
            FullStackJar();
        }
        // candy.GetComponent<Rigidbody>().useGravity = true;
        // this.TaskDelay(0.3f, () => candy.GetComponent<Rigidbody>().useGravity = false);

        RunManager.instance.OnChangeCandyStack();
    }

    public void FullStackJar()
    {
        RunManager.instance.enableCandyStack = false;

        this.TaskDelay(0.2f, () =>
        {
            cork.transform.DOScale(new Vector3(0.01f, 0.01f, 0.01f), 0.2f).OnComplete(() =>
            {
                cork.transform.DOLocalMoveY(corkMoveEndPoint.transform.localPosition.y, 0.2f).OnComplete(() =>
                {
                    MoveToRail();
                    // RunManager.instance.enableCandyStack = true;
                });
            });
        });
    }

    public void MoveToRail()
    {
        RunManager.instance.MoveToRail(this);
        GetComponentInChildren<Collider>().enabled = false;
    }
}
