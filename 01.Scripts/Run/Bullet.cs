using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class Bullet : MonoBehaviour
{
    public Vector3 rotationAxis = Vector3.up;
    public float rotationSpeed = 1f;
    public float rotationDuration = 1f;

    [SerializeField] private MeshRenderer meshRenderer;
    // Start is called before the first frame update

    private Tween moveTween = null;
    private Tween rotateTween = null;
    TaskUtil.DelayTaskMethod pushDelay = null;

    private void OnEnable()
    {
        int randomIndex = Random.Range(0, RunManager.instance.jellyBeanMats.Length);

        var mat = RunManager.instance.jellyBeanMats[randomIndex];

        Material[] materials = { mat };

        meshRenderer.materials = materials;

        StartRotation();

        pushDelay = RunManager.instance.TaskDelay(RunManager.instance.GetBulletRange() / 100f, () => { GetComponentInChildren<Bullet>().Push(); });
    }

    void StartRotation()
    {
        // 랜덤한 회전 축을 생성합니다.
        Vector3 randomAxis = Random.onUnitSphere;

        // DoTween을 사용하여 오브젝트를 회전시킵니다.
        rotateTween = transform.DORotate(randomAxis * 360f, rotationSpeed, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .OnComplete(StartRotation);
    }

    public void Push(bool _particle = true)
    {
        if (_particle)
        {
            var particle = Managers.Pool.Pop(Managers.Resource.Load<GameObject>("Particles/Jelly Particle"));

            particle.transform.position = transform.position;

            // var shape = particle.GetComponentInChildren<ParticleSystem>().shape;
            // shape.meshRenderer = meshRenderer;

            var renderer = particle.GetComponentInChildren<ParticleSystemRenderer>();
            renderer.material = meshRenderer.material;

            particle.GetComponentInChildren<ParticleSystem>().Play();

            RunManager.instance.TaskDelay(3, () => Managers.Pool.Push(particle.GetComponentInChildren<Poolable>()));
        }

        Managers.Pool.Push(GetComponentInChildren<Poolable>());
    }

    public void SetDoMove(Vector3 targetPos)
    {
        moveTween = transform.DOMove(targetPos, 100);
    }

    private void OnDisable()
    {
        moveTween.Kill();
        rotateTween.Kill();
        if (pushDelay != null)
            pushDelay.Kill();
    }
}
