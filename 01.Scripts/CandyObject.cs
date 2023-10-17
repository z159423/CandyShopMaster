using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New CandyObject", menuName = "Games/CandyObject")]
public class CandyObject : ScriptableObject
{
    [SerializeField] public int id;
    [SerializeField] public Material mat;
    [SerializeField] public Sprite icon;
    [SerializeField] public Material particleMat;
    [SerializeField] public int cost;
    [SerializeField] public float unlockPoint;


    [SerializeField] public CandyObject nextCandy;

    // private void OnValidate()
    // {
    //     var candys = Resources.LoadAll<CandyObject>("");

    //     foreach (var candy in candys)
    //     {
    //         if (candy != this && candy.id == this.id)
    //         {
    //             Debug.LogError("중복되는 id가 있습니다. id : " + candy.id);
    //         }
    //     }
    // }
}
