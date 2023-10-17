using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CandyHead : MonoBehaviour
{
    public Transform firePos;
    public GameObject bulletPrefab;

    public Transform cutCandyPos;
    public GameObject cutCandyPrefab;

    public CandyObject candyObject;

    public float cpi2Length = 100;

    public void GenerateBullet()
    {
        if (!StageManager.instance.IsAllowJellyGun)
            return;

        if (RunManager.instance.tripleShot)
        {
            Transform[] bullets = new Transform[3];

            for (int i = 0; i < 3; i++)
            {
                var bullet = Managers.Pool.Pop(bulletPrefab);
                bullet.transform.position = firePos.transform.position;
                bullets[i] = bullet.transform;
                // this.TaskDelay(RunManager.instance.GetBulletRange() / 100f, () => {bullet.GetComponentInChildren<Bullet>().Push();});
            }

            bullets[0].GetComponent<Bullet>().SetDoMove(bullets[0].transform.position + new Vector3(500, 0, 3000));
            bullets[1].GetComponent<Bullet>().SetDoMove(bullets[0].transform.position + new Vector3(0, 0, 3000));
            bullets[2].GetComponent<Bullet>().SetDoMove(bullets[0].transform.position + new Vector3(-500, 0, 3000));
        }
        else
        {
            var bullet = Managers.Pool.Pop(bulletPrefab);
            bullet.transform.position = firePos.transform.position;

            bullet.GetComponent<Bullet>().SetDoMove(bullet.transform.position + new Vector3(0, 0, 3000));

            // bullet.transform.DOMoveZ(3000, 100);

            // this.TaskDelay(RunManager.instance.GetBulletRange() / 100f, () => {bullet.GetComponentInChildren<Bullet>().Push();});
        }
    }

    public void CutCandy(List<GameObject> list, float length = 100f, bool torque = true)
    {
        var candy = Instantiate(cutCandyPrefab, cutCandyPos.position, Quaternion.identity);

        candy.GetComponentInChildren<SkinnedMeshRenderer>().materials = new Material[] { candyObject.mat };

        if (length < 50)
            length = 50;

        candy.transform.GetChild(0).transform.localScale = new Vector3(candy.transform.GetChild(0).transform.localScale.x, candy.transform.GetChild(0).transform.localScale.y, length / 1000f);

        if (torque)
            candy.GetComponentInChildren<Rigidbody>().AddTorque(Random.insideUnitSphere * Random.Range(3f, 10f), ForceMode.Impulse);

        list.Add(candy);

        //new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f)
    }

    public void UpgradeCandy()
    {
        if (candyObject.nextCandy == null)
            return;

        candyObject = candyObject.nextCandy;
        transform.DOScale(new Vector3(0, 0, 1), 0.1f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            GetComponentInChildren<SkinnedMeshRenderer>().materials = new Material[] { candyObject.mat };

            transform.DOScale(new Vector3(1.25f, 1.25f, 1), 0.1f).SetEase(Ease.InCubic);

            var CandyTailController = GetComponent<CandyTailController>();

            for (int i = 0; i < CandyTailController.GetTailParts().Length * 0.75f; i += 2)
            {
                var particle = Managers.Pool.Pop(Managers.Resource.Load<GameObject>("Particles/Jelly Particle"));

                particle.transform.position = CandyTailController.GetTailParts()[i].position;

                var renderer = particle.GetComponentInChildren<ParticleSystemRenderer>();
                renderer.material = candyObject.mat;

                particle.GetComponentInChildren<ParticleSystem>().Play();

                RunManager.instance.TaskDelay(3, () => Managers.Pool.Push(particle.GetComponentInChildren<Poolable>()));
            }

        });

        print("upgrade");
    }

}
