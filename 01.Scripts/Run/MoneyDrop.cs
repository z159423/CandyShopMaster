using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyDrop : MonoBehaviour
{
    public int moneyValue;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RunManager.instance.GetMoney(moneyValue);

            Managers.Pool.Push(GetComponent<Poolable>());

            var particle = Managers.Pool.Pop(Managers.Resource.Load<GameObject>("Particles/DollarbillDirectional"));

            particle.transform.position = transform.position;
            particle.GetComponentInChildren<ParticleSystem>().Play();

            this.TaskDelay(5, () => Managers.Pool.Push(particle.GetComponentInParent<Poolable>()));

            SaveManager.instance.GetMoney(moneyValue);

            EventManager.instance.CustomEvent(AnalyticsType.RUN, "GetDropMoney", true, true);


            //     MondayOFF.EventTracker.LogCustomEvent(
            // "RUN",
            // new Dictionary<string, string> { { "RUN_TYPE", "GetDropMoney" } }
            // );
        }
    }
}
