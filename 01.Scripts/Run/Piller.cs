using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PillerType
{
    Length = 1,
    FireRate = 2,
    Range = 3,
    Candy = 4,
    TripleShot = 5,
    CandyLevelUp = 6
}

public class Piller : MonoBehaviour
{
    public Text nameText;
    public Text valueText;
    [SerializeField] Image skillImage;

    public PillerType type;

    [SerializeField] private Material postiveMat;
    [SerializeField] private Material postivePillerCenterMat;

    [SerializeField] private Material negativeMat;
    [SerializeField] private Material negativePillerCenterMat;

    [SerializeField] private Material specialPillerCenterMat;

    [SerializeField] private MeshRenderer plag;
    [SerializeField] private MeshRenderer pillerCenter;


    public float value;
    public bool multiply = false;

    public bool enableActive = true;

    private void Awake()
    {
        StageManager.instance.pillerList.Add(this);
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {

        switch (type)
        {
            case PillerType.Length:
                // value = RunManager.instance.defaultPillerLengthValue;
                valueText.gameObject.SetActive(true);
                skillImage.gameObject.SetActive(false);

                nameText.text = "Length";
                break;

            case PillerType.FireRate:
                // value = RunManager.instance.defaultPillerFireRateValue;
                valueText.gameObject.SetActive(true);
                skillImage.gameObject.SetActive(false);

                nameText.text = "FireRate";
                break;


            case PillerType.Range:
                // value = RunManager.defaultBulletRange;
                valueText.gameObject.SetActive(true);
                skillImage.gameObject.SetActive(false);

                nameText.text = "Range";
                break;

            case PillerType.Candy:
                // value = RunManager.defaultCandyCount;
                nameText.text = "More Candy!";
                valueText.gameObject.SetActive(false);
                // nameText.enabled = false;
                skillImage.sprite = Resources.Load<Sprite>("UI/CandyPlus");
                skillImage.gameObject.SetActive(true);

                break;

            case PillerType.TripleShot:
                // value = RunManager.defaultCandyCount;
                nameText.text = "Triple Shot!";
                valueText.gameObject.SetActive(false);
                // nameText.enabled = false;
                skillImage.sprite = Resources.Load<Sprite>("UI/TripleShot");
                skillImage.gameObject.SetActive(true);

                break;

            case PillerType.CandyLevelUp:
                // value = RunManager.defaultCandyCount;
                nameText.text = "Candy Level Up!";
                valueText.gameObject.SetActive(false);
                // nameText.enabled = false;
                skillImage.sprite = Resources.Load<Sprite>("UI/CandyLevelUp");
                skillImage.gameObject.SetActive(true);

                break;
        }

        OnChangeValue();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet") && enableActive)
        {
            switch (type)
            {
                case PillerType.Length:
                    if (!multiply)
                        value += RunManager.instance.addCandyLengthValue;
                    break;

                case PillerType.FireRate:
                    value += RunManager.instance.addFireRateValue;
                    break;

                case PillerType.Range:
                    value += RunManager.instance.addBulletRangeValue;
                    break;

                    // case PillerType.Candy:
                    //     value += RunManager.defaultCandyCount;
                    //     break;
            }

            OnChangeValue();
            // Destroy(other.gameObject);

            other.GetComponentInChildren<Bullet>().Push();
        }

        if (other.CompareTag("Player") && enableActive)
        {
            EventManager.instance.CustomEvent(AnalyticsType.RUN, "Piller Pass_" + type.ToString(), true, true);

            RunManager.instance.PillerPass(type, value);

            var particle = Managers.Pool.Pop(Resources.Load<GameObject>("Particles/Piller Pass Particle")).GetComponentInChildren<ParticleSystem>();

            particle.transform.position = this.transform.position + (Vector3.up * 2);
            particle.Play();

            IdleManager.instance.TaskDelay(5, () => Managers.Pool.Push(particle.GetComponent<Poolable>()));

            if (GetComponentInParent<PillerSet>() != null)
            {
                GetComponentInParent<PillerSet>().gameObject.SetActive(false);
                // GetComponentInParent<PillerSet>().deactiveOtherPiller(this);
                // gameObject.SetActive(false);
            }
            else
                gameObject.SetActive(false);
        }
    }

    private void OnChangeValue()
    {
        if (type == PillerType.Candy || type == PillerType.TripleShot || type == PillerType.CandyLevelUp)
        {
            pillerCenter.materials = new Material[] { specialPillerCenterMat };
        }
        else if (value > 0)
        {
            var mat = new Material[] { postiveMat };
            plag.materials = mat;

            var mat2 = new Material[] { postivePillerCenterMat };
            pillerCenter.materials = mat2;

            if (multiply)
                valueText.text = "x" + value.ToString();
            else
                valueText.text = "+" + value.ToString();
        }
        else
        {
            var mat = new Material[] { negativeMat };
            plag.materials = mat;
            valueText.text = "" + value.ToString();

            var mat2 = new Material[] { negativePillerCenterMat };
            pillerCenter.materials = mat2;
        }
    }

    /// <summary>
    /// Called when the script is loaded or a value is changed in the
    /// inspector (Called in the editor only).
    /// </summary>
    private void OnValidate()
    {
        Init();
        OnChangeValue();
    }

    public void ChangeToTripleShot()
    {
        type = PillerType.TripleShot;

        Init();
    }

    public void ChangeToAddCandy()
    {
        type = PillerType.Candy;

        value = 1;

        Init();
    }

    public void ChangeToCandyLevelUp()
    {
        type = PillerType.CandyLevelUp;

        value = 1;

        Init();
    }

    public void ChangeToNormalRandom(PillerType type = 0)
    {
        if (type != 0)
            this.type = type;

        value = (value > 0) ? Random.Range(10, 30) * 10 : value = Random.Range(10, 30) * -10;

        Init();
    }

    public void ChangeNormalType()
    {
        type = (PillerType)Random.Range(1, 4);

        value = (value > 0) ? Random.Range(10, 30) * 10 : value = Random.Range(10, 30) * -10;

        Init();
    }
}
