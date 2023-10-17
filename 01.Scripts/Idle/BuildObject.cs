using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class BuildObject : SaveableObject
{
    // [SerializeField]protected BuildSaveData savedata;

    [SerializeField] Collector collector;

    [SerializeField] Vector3 buildSize = Vector3.one;

    [SerializeField] bool SpawnParticle = false;

    private void Awake()
    {
        Sleep();
    }

    override protected void NewGuid()
    {
        Guid = System.Guid.NewGuid().ToString();

        // savedata.SetGuid(Guid);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }


    [Button("ForceBuild")]
    public virtual void Build(bool direct = false)
    {
        gameObject.SetActive(true);

        if (direct)
        {
            transform.localScale = Vector3.one;
            OnCompleteBuild();
        }
        else
        {
            transform.DOScale(buildSize, 0.5f).SetEase(Ease.OutBack).OnComplete(OnCompleteBuild);
        }

        if (SpawnParticle)
            IdleManager.instance.PopParticle("Particles/Dust_Big", transform.position + Vector3.up * 3);

        void OnCompleteBuild()
        {
            if (GetComponentInChildren<UnityEngine.AI.NavMeshObstacle>() != null)
            {
                GetComponentInChildren<UnityEngine.AI.NavMeshObstacle>().enabled = false;
                GetComponentInChildren<UnityEngine.AI.NavMeshObstacle>().enabled = true;
            }
        }

        if (GetComponentInChildren<UnityEngine.AI.NavMeshObstacle>() != null)
        {
            GetComponentInChildren<UnityEngine.AI.NavMeshObstacle>().enabled = false;
            GetComponentInChildren<UnityEngine.AI.NavMeshObstacle>().enabled = true;
        }
    }

    public void Sleep()
    {
        transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }
}

[System.Serializable]
public class BuildSaveData
{
    [SerializeField] string guid;
    public void SetGuid(string _guid) => guid = _guid;
    [SerializeField] bool complete;
    [SerializeField] bool currentMoneyStack;
}
