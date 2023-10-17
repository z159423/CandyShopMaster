using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class RunMapGenerator : MonoBehaviour
{
    [BoxGroup("참조")][SerializeField] Transform mapParent;

    [BoxGroup("value")][SerializeField] Vector2 addCandyValue;
    [BoxGroup("value")][SerializeField] Vector2 candyLevelUpValue;

    [BoxGroup("currentValue")][SerializeField] int targetAddCandyPiller;
    [BoxGroup("currentValue")][SerializeField] int currentAddCandyPiller;

    [BoxGroup("currentValue")][SerializeField] int targetCandyLevelUpPiller;
    [BoxGroup("currentValue")][SerializeField] int currentAddCandyLevelUpPiller;
    [BoxGroup("currentValue")][SerializeField] bool tripleShot;


    [SerializeField] GameObject[] addCandyPillerPrefabs;
    [SerializeField] GameObject[] randomPillerPrefabs;
    [SerializeField] GameObject[] obstaclePrefabs;
    [SerializeField] GameObject[] randomObjectPrefabs;
    [SerializeField] GameObject[] tripleShotPillerPrefabs;


    [SerializeField] List<Transform> mapPoints = new List<Transform>();


    public void GenerateMap()
    {
        targetAddCandyPiller = (int)Random.Range(addCandyValue.x, addCandyValue.y);

        targetCandyLevelUpPiller = (int)Random.Range(candyLevelUpValue.x, candyLevelUpValue.y);

        tripleShot = RandomBooleanGenerator.GenerateRandomBoolean();

        List<MapPart> mapParts = new List<MapPart>();

        while (mapPoints.Count > 0)
        {
            Transform point = GetRandomPoint();

            if (tripleShot && (Random.Range(0, 25) < 1))
            {
                var prefab = Instantiate(randomPillerPrefabs[Random.Range(0, randomPillerPrefabs.Length)], point);

                mapParts.Add(prefab.GetComponentInChildren<MapPart>());

                var pillers = prefab.GetComponentsInChildren<Piller>();

                foreach (var piller in pillers)
                {
                    if (piller.value > 0)
                        piller.ChangeToTripleShot();
                    break;
                }

                tripleShot = false;

                print("triple shot ready");
            }
            else if (currentAddCandyPiller < targetAddCandyPiller && currentAddCandyLevelUpPiller < targetCandyLevelUpPiller)
            {
                switch (Random.Range(0, 2))
                {
                    case 0:
                        AddCandy();
                        break;

                    case 1:
                        CandyLevelUp();
                        break;
                }
            }
            else if (currentAddCandyPiller < targetAddCandyPiller)
            {
                AddCandy();

                continue;
            }
            else if (currentAddCandyLevelUpPiller < targetCandyLevelUpPiller)
            {
                CandyLevelUp();

                continue;
            }
            else
            {
                var prefab = Instantiate(randomObjectPrefabs[Random.Range(0, randomObjectPrefabs.Length)], point);

                var pillers = prefab.GetComponentsInChildren<Piller>();

                mapParts.Add(prefab.GetComponentInChildren<MapPart>());

                Dictionary<PillerType, float> probabilities = new Dictionary<PillerType, float>();

                // probabilities.Add(PillerType.Length, 60);
                probabilities.Add(PillerType.FireRate, 50);
                probabilities.Add(PillerType.Range, 30);

                foreach (var piller in pillers)
                {
                    piller.ChangeToNormalRandom(SelectItemByProbability(probabilities));
                }
            }

            void AddCandy()
            {
                var prefab = Instantiate(randomPillerPrefabs[Random.Range(0, randomPillerPrefabs.Length)], point);

                var pillers = prefab.GetComponentsInChildren<Piller>();

                mapParts.Add(prefab.GetComponentInChildren<MapPart>());

                foreach (var piller in pillers)
                {
                    if (piller.value > 0)
                        piller.ChangeToAddCandy();

                    break;
                }

                currentAddCandyPiller++;
            }

            void CandyLevelUp()
            {
                var prefab = Instantiate(randomPillerPrefabs[Random.Range(0, randomPillerPrefabs.Length)], point);

                var pillers = prefab.GetComponentsInChildren<Piller>();

                mapParts.Add(prefab.GetComponentInChildren<MapPart>());

                foreach (var piller in pillers)
                {
                    if (piller.value > 0)
                        piller.ChangeToCandyLevelUp();

                    break;
                }

                currentAddCandyLevelUpPiller++;
            }
        }
        int num = Random.Range(0, 4);
        mapParts.ForEach((n) => n.ChangeCandyModel(num));

        mapParts.ForEach((n) => n.ChanceToRoyalCandy());

        EventManager.instance.CustomEvent(AnalyticsType.RUN, "RunMapRandomGenerated", true, true);

        //         MondayOFF.EventTracker.LogCustomEvent(
        // 		"RUN", 
        // 		new Dictionary<string, string>{ {"RUN_TYPE", "RunMapRandomGenerated"} }
        // );
    }

    public Transform GetRandomPoint()
    {
        var num = Random.Range(0, mapPoints.Count);

        Transform point = mapPoints[num];

        mapPoints.RemoveAt(num);

        return point;
    }

    private T SelectItemByProbability<T>(Dictionary<T, float> probabilities)
    {
        float totalProbability = 0;
        foreach (var pair in probabilities)
        {
            totalProbability += pair.Value;
        }

        float randomValue = UnityEngine.Random.Range(0f, totalProbability);

        foreach (var pair in probabilities)
        {
            if (randomValue <= pair.Value)
            {
                return pair.Key;
            }
            randomValue -= pair.Value;
        }

        // 딕셔너리에 등록된 항목 중 하나도 선택되지 않았을 경우 기본값 반환
        return default(T);
    }
}

public class RandomBooleanGenerator
{
    private static readonly System.Random random = new System.Random();

    public static bool GenerateRandomBoolean()
    {
        // 0부터 1까지의 무작위 정수를 생성합니다.
        int randomNumber = random.Next(0, 2);

        // randomNumber가 0일 때 false를 반환하고, 1일 때 true를 반환합니다.
        return randomNumber == 1;
    }
}
