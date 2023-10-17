using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyPieces : MonoBehaviour
{
    [SerializeField] Rigidbody[] rigidbodies;
    [SerializeField] float force = 100f;

    public void Init(Material mat)
    {
        foreach (var rigid in rigidbodies)
        {
            rigid.GetComponent<MeshRenderer>().materials = new Material[] { mat };
        }
    }

    public void ExplosionPieces(Transform from)
    {
        foreach (var rigid in rigidbodies)
        {
            rigid.isKinematic = false;
            rigid.AddForce((from.position - rigid.transform.position).normalized * force, ForceMode.Impulse);

            this.TaskDelay(5, () => Destroy(gameObject));
        }
    }
}
