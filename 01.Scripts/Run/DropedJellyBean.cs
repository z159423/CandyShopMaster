using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DropedJellyBean : MonoBehaviour
{

    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] GameObject[] models;
    [SerializeField] Animator animator;


    [SerializeField] int currentModelNum = 0;

    [SerializeField] MeshRenderer jellyMesh;

    [SerializeField] GameObject royalCandyModel;

    [SerializeField] bool royalCandy = false;

    int jellyMatNum = 0;


    public float value = 50f;
    public float maxValue = 200f;

    public bool changeJellyColor = true;

    private void Start()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();

        animator = GetComponentInChildren<Animator>();

        Init();
    }

#if UNITY_STANDALONE || UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            NextCandyModel();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            animator.enabled = !animator.enabled;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            jellyMatNum++;

            if (jellyMatNum >= RunManager.instance.jellyBeanMats.Length)
                jellyMatNum = 0;

            var mat = RunManager.instance.jellyBeanMats[jellyMatNum];
            Material[] materials = { mat };
            meshRenderer.materials = materials;
        }
    }
#endif

    public void NextCandyModel()
    {
        currentModelNum++;

        meshRenderer.gameObject.SetActive(false);

        if (currentModelNum >= models.Length)
        {
            currentModelNum = 0;
        }

        meshRenderer = models[currentModelNum].GetComponent<MeshRenderer>();
        meshRenderer.gameObject.SetActive(true);
    }

    public void Init()
    {
        if (changeJellyColor)
        {
            int randomIndex = Random.Range(0, RunManager.instance.jellyBeanMats.Length);
            var mat = RunManager.instance.jellyBeanMats[randomIndex];
            Material[] materials = { mat };
            meshRenderer.materials = materials;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var particle = Managers.Pool.Pop(Managers.Resource.Load<GameObject>("Particles/Jelly Particle"));

            particle.transform.position = transform.position;
            var renderer = particle.GetComponentInChildren<ParticleSystemRenderer>();
            renderer.material = meshRenderer.material;

            particle.GetComponentInChildren<ParticleSystem>().Play();

            RunManager.instance.TaskDelay(3, () => Managers.Pool.Push(particle.GetComponentInChildren<Poolable>()));

            Managers.Sound.Play("J.BoB - Mobile Game - Interface Hollow Woody Click", volume: 0.7f, pitch: Random.Range(0.75f, 1f));

            if (royalCandy)
            {
                SaveManager.instance.AddRoyalCandy(1);
                EventManager.instance.CustomEvent(AnalyticsType.RUN, "Player Get RoyalCandy", true, true);

                // var particle2 = Managers.Pool.Pop(Resources.Load<GameObject>("Particles/UIAttractor_RoyalCandy"), RunManager.instance.runGameUI.transform);
                var particle2 = Instantiate(Resources.Load<GameObject>("Particles/UIAttractor_RoyalCandy"), RunManager.instance.runGameUI.transform);

                // particle2.transform.position = Vector3.zero;
                // particle.transform.localScale = Vector3.one;

                // Vector2 anchordPos = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);

                Vector2 anchordPos = Camera.main.WorldToViewportPoint(transform.position);

                particle2.GetComponentInChildren<UIAttractorCustom>().Init(RunManager.instance.royalCandyTargetTrans, new Vector2(anchordPos.x * RunManager.instance.runGameUI.GetComponent<RectTransform>().sizeDelta.x, anchordPos.y * RunManager.instance.runGameUI.GetComponent<RectTransform>().sizeDelta.y));
                particle2.GetComponentInChildren<ParticleSystem>().Play();

                // RunManager.instance.TaskDelay(3, () => Managers.Pool.Push(particle2.GetComponentInChildren<Poolable>()));
            }
            else
            {
                RunManager.instance.AddCandyLength(value);
                EventManager.instance.CustomEvent(AnalyticsType.RUN, "Player Get Jelly", true, true);
            }
            gameObject.SetActive(false);


            switch (IdleManager.instance.runGameType)
            {
                case RunGameType.CPI1:
                    RunManager.instance.candyStackQueue.Enqueue(meshRenderer.gameObject);
                    // RunManager.instance.currentPlayerCandyJar.StackCandy(1);
                    break;
            }
        }

        if (other.CompareTag("Bullet") && value < maxValue)
        {
            value += 5f;
            meshRenderer.transform.localScale = new Vector3(meshRenderer.transform.localScale.x + 0.04f, meshRenderer.transform.localScale.y + 0.04f, meshRenderer.transform.localScale.z + 0.04f);
            meshRenderer.transform.DOPunchScale(Vector3.one * 0.15f, 0.15f, 1, 0.1f);
            other.GetComponentInChildren<Bullet>().Push();
            // Destroy(other.gameObject);
        }
    }

    public void ChangeCandy(int num)
    {
        if (meshRenderer == null)
            meshRenderer = GetComponentInChildren<MeshRenderer>();

        Init();

        meshRenderer.gameObject.SetActive(false);

        meshRenderer = models[num].GetComponent<MeshRenderer>();
        meshRenderer.gameObject.SetActive(true);

        changeJellyColor = false;
    }

    public void ChanceToRoyalCandy(bool confirm = false)
    {
        if (confirm ? true : Random.Range(0, 30) == 0)
        {
            if (meshRenderer == null)
                meshRenderer = GetComponentInChildren<MeshRenderer>();

            meshRenderer.gameObject.SetActive(false);

            royalCandyModel.SetActive(true);

            royalCandy = true;
            changeJellyColor = false;
        }
    }
}
