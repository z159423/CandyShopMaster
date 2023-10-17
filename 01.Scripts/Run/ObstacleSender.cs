using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSender : MonoBehaviour
{
    public void ResetObstacle()
    {
        if (GetComponentInChildren<RunObstacle>() != null)
        {
            GetComponentInChildren<RunObstacle>().Reset();
        }
    }
}
