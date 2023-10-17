using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleMap : MonoBehaviour
{
    public OrderLine[] orderLines;
    public Transform[] candyJarSpawnPos;
    [SerializeField] Transform[] customerSpawnPoints;

    public Transform workerSpawnPoint;


    public Transform GetRandomSpawnPoint() => customerSpawnPoints[Random.Range(0, customerSpawnPoints.Length)];
}
