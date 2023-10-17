using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class RunObstacle : MonoBehaviour
{
    public int hp;

    public float damage = 100;

    public bool isUsed = false;
    public bool isDestroyed = false;
    public bool knockBack = false;
    public bool enableReuse = false;
    public float reuseTime = 1.5f;

    [SerializeField] MeshRenderer meshRenderer;

    Tweener hitTween = null;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet") && !isDestroyed)
        {
            TakeDamage();

            other.GetComponentInChildren<Bullet>().Push();
        }

        if (other.CompareTag("Player") && !isUsed)
        {
            if (other.GetComponentInParent<CandyHead>().transform.position.z - transform.position.z > 8f)
                return;

            isUsed = true;

            // print(Vector3.Distance(transform.position, other.transform.position));

            // Debug.DrawLine(transform.position, other.GetComponentInParent<CandyHead>().transform.position, Color.blue, 100f);

            if (knockBack)
                if (Vector3.Distance(transform.position, other.GetComponentInParent<CandyHead>().transform.position) < 3f)
                {
                    knockBack = true;
                }
                else
                    knockBack = false;


            RunManager.instance.TakeDamage(200, GetComponent<Collider>().ClosestPointOnBounds(other.bounds.center), knockBack);

            EventManager.instance.CustomEvent(AnalyticsType.RUN, "hitByObstacle_" + gameObject.name, true, true);

            if (enableReuse)
                SaveManager.instance.TaskDelay(reuseTime, () => isUsed = true);

            //     MondayOFF.EventTracker.LogCustomEvent(
            // "RUN",
            // new Dictionary<string, string> { { "RUN_TYPE", "hitByObstacle" }, { "StageNum", StageManager.instance.currentStageNum.ToString() } }
            // );
        }
    }

    public void TakeDamage()
    {
        hp--;

        if (hitTween != null)
        {
            if (!hitTween.IsPlaying())
            {
                hitTween = transform.DOPunchScale(Vector3.one * 0.07f, 0.2f, 2, 1);
            }
        }
        else
            hitTween = transform.DOPunchScale(Vector3.one * 0.07f, 0.2f, 2, 1);


        if (hp <= 0)
        {
            Destroy();
        }
    }

    public void Destroy()
    {
        isDestroyed = true;
        gameObject.SetActive(false);

        var money = Managers.Pool.Pop(Managers.Resource.Load<GameObject>("Money"));

        money.transform.position = transform.position;

        money.transform.DOJump(money.transform.position, 1.5f, 1, 0.5f);

        var particle = Managers.Pool.Pop(Managers.Resource.Load<GameObject>("Particles/Obstacle Particle"));
        var shape = particle.GetComponentInChildren<ParticleSystem>().shape;

        shape.meshRenderer = meshRenderer;

        particle.GetComponentInChildren<ParticleSystem>().Play();

        this.TaskDelay(3f, () => Managers.Pool.Push(particle.GetComponentInChildren<Poolable>()));

        EventManager.instance.CustomEvent(AnalyticsType.RUN, "destroyObstacle", true, true);

        // MondayOFF.EventTracker.LogCustomEvent(
        // "RUN",
        // new Dictionary<string, string> { { "RUN_TYPE", "destroyObstacle" }, { "StageNum", StageManager.instance.currentStageNum.ToString() } }
        // );
    }

    public void Reset()
    {
        isUsed = false;
    }
}
