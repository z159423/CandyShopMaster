using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StartCard : MonoBehaviour
{
    public CardSlot[] cardslots;

    public void GenearteCards()
    {
        List<RunCardType> typeList = EnumToListConverter.GetEnumList();

        foreach (var slot in cardslots)
        {
            var random = Random.Range(0, typeList.Count);

            slot.Init(typeList[random]);

            typeList.RemoveAt(random);
        }
    }
}

public class EnumToListConverter
{
    public static List<RunCardType> GetEnumList()
    {
        return new List<RunCardType>((RunCardType[])System.Enum.GetValues(typeof(RunCardType)));
    }
}

public enum RunCardType
{
    PlusCandy = 1,
    TripleShot = 2
}