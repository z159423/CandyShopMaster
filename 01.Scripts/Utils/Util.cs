﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Lofelt.NiceVibrations;
using DG.Tweening;

public class Util
{

    private static Dictionary<float, WaitForSeconds> waitDic = new Dictionary<float, WaitForSeconds>();
    public static WaitForSeconds WaitGet(float waitSec)
    {
        if (waitDic.TryGetValue(waitSec, out WaitForSeconds waittime)) return waittime;
        return waitDic[waitSec] = new WaitForSeconds(waitSec);
    }
    public static WaitForFixedUpdate waitFixed = new WaitForFixedUpdate();

    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;

        return transform.gameObject;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>(true))
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }
        return null;
    }

    public static T DeepCopy<T>(T obj)
    {
        using (var stream = new MemoryStream())
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
            stream.Position = 0;

            return (T)formatter.Deserialize(stream);
        }
    }

    public static T StringToEnum<T>(string name) where T : Enum
    {
        return (T)Enum.Parse(typeof(T), name);
    }

    ///<summary>분산랜덤</summary>
    /// <param name="variance">클수록 뾰족해짐! 최소값 기본값이 1</param>
    public static int DistributionRandom(int middleValue, float range, int variance = 1)
    {
        variance = Mathf.Max(1, variance);
        variance = (int)Math.Pow(10, variance);

        float u = UnityEngine.Random.Range(0.0f, 1.0f - Mathf.Epsilon);
        int ret = (int)(Mathf.Log(u / (1 - u), variance) * range + middleValue);

        while (ret < middleValue - range || ret > middleValue + range)
        {
            u = UnityEngine.Random.Range(0.0f, 1.0f - Mathf.Epsilon);
            ret = (int)(Mathf.Log(u / (1 - u), variance) * range + middleValue);
        }
        return ret;
    }

    /// <param name="power">1 부터 6 까지</param>
    public static void Haptic(int power = 1)
    {
        if (!Managers.Data.UseHaptic) return;

        if (power == 2)
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);
        else if (power == 3)
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        else if (power == 4)
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.RigidImpact);
        else if (power == 5)
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);
        else if (power == 6)
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.HeavyImpact);
        else
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
    }
    /// <param name="amplitude">Amplitude, from 0.0 to 1.0</param>
    /// <param name="frequency">Frequency, from 0.0 to 1.0</param>
    /// <param name="duration">Play duration in seconds</param>
    public static void HapticConstant(float amplitude, float frequency, float duration)
    {
        if (!Managers.Data.UseHaptic) return;

        HapticPatterns.PlayConstant(amplitude, frequency, duration);
    }

    public static Vector3 UiToRealPos(Vector2 uiPos)
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(uiPos.x, uiPos.y, (Camera.main.nearClipPlane + Camera.main.farClipPlane) * 0.5f));
    }
    public static Vector2 RealPosToUi(Vector3 realPos)
    {
        return Camera.main.WorldToScreenPoint(realPos);
    }

    public static void ParticleStart(GameObject particle)
    {
        particle.GetComponent<ParticleSystem>()?.PlayAllParticle();
    }

    public static void ParticleStop(GameObject particle)
    {
        particle.GetComponent<ParticleSystem>()?.StopAllParticle();
    }

    public static Tween ManualTo(Action<float> func, float duration, System.Action onComplete)
    {
        float elapse = 0f;
        return DOTween.To(() => elapse, (v) => { elapse = v; if (func != null) func(elapse); }, 1f, duration).
            OnComplete(() => { if (onComplete != null) onComplete(); });
    }

    public static List<T> ShuffleList<T>(List<T> list)
    {
        int random1, random2;
        T temp;

        for (int i = 0; i < list.Count; ++i)
        {
            random1 = UnityEngine.Random.Range(0, list.Count);
            random2 = UnityEngine.Random.Range(0, list.Count);

            temp = list[random1];
            list[random1] = list[random2];
            list[random2] = temp;
        }

        return list;
    }
}