using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lineDrawer : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Transform start;
    [SerializeField] Transform end;

    private void Start()
    {
        lineRenderer.SetPosition(0, start.transform.position);
    }

    private void Update()
    {
        lineRenderer.SetPosition(1, end.transform.position);
    }
}
