using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Item
{
    public int id;

    public int count = 1;
    public int CalculateTotalCost()
    {
        return Resources.LoadAll<IdleItem>("Item").Where((n) => n.id == id).First().cost * count;
    }
}


