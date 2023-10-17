using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using DG.Tweening;
using static UnityEngine.Rendering.DebugUI;

public class UIDynamicPanel : MonoBehaviour
{
    enum PanelPositionType
    {
        LOCAL,
        ANCHORED
    }

    [System.Serializable]
    class PanelSetting
    {
        public PanelPositionType PositionType;
        public RectTransform Panel;
        [ReadOnly] public Vector3 ExpandPos;
        [ReadOnly] public Vector2 ExpandSize;
        [ReadOnly] public Vector3 CollapsePos;
        [ReadOnly] public Vector2 CollapseSize;

        [HorizontalGroup("Setting")]
        [Button]
        void WriteExpand()
        {
            if (PositionType == PanelPositionType.LOCAL)
                ExpandPos = Panel.localPosition;
            if (PositionType == PanelPositionType.ANCHORED)
                ExpandPos = Panel.anchoredPosition;
            ExpandSize = Panel.sizeDelta;
        }

        [HorizontalGroup("Setting")]
        [Button]
        void WriteCollapse()
        {
            if (PositionType == PanelPositionType.LOCAL)
                CollapsePos = Panel.localPosition;
            if (PositionType == PanelPositionType.ANCHORED)
                CollapsePos = Panel.anchoredPosition;
            CollapseSize = Panel.sizeDelta;
        }
    }

    [TitleGroup("Panel")]
    [SerializeField] PanelSetting[] panels;

    [TitleGroup("Animation")]
    [SerializeField] bool enablePreview;
    [SerializeField] AnimationCurve animationCurve;
    [SerializeField] float animationDuration = 0.3f;
    float previewProgress;
    bool previewExpand;

    bool isExpand;
    bool first;
    Tween animationTween;

    [HorizontalGroup("Preview")]
    [Button()]
    void PreviewToggle()
    {
        previewExpand = !previewExpand;
    }

    void PanelLerp(PanelSetting setting, float t)
    {
        if (setting.PositionType == PanelPositionType.LOCAL)
            setting.Panel.localPosition = Vector3.Lerp(setting.CollapsePos, setting.ExpandPos, animationCurve.Evaluate(t));
        if (setting.PositionType == PanelPositionType.ANCHORED)
            setting.Panel.anchoredPosition = Vector3.Lerp(setting.CollapsePos, setting.ExpandPos, animationCurve.Evaluate(t));
        setting.Panel.sizeDelta = Vector2.Lerp(setting.CollapseSize, setting.ExpandSize, animationCurve.Evaluate(t));
    }

    public void Expand(bool immediate = false, System.Action onComplete = null)
    {
        if (animationTween != null)
            animationTween.Kill();
        if (isExpand && first)
            return;

        isExpand = true;
        first = true;

        if (immediate)
        {
            foreach (var panel in panels)
                PanelLerp(panel, 1f);
        }
        else
        {
            animationTween = Util.ManualTo((t) =>
            {
                foreach (var panel in panels)
                    PanelLerp(panel, t);
            }, animationDuration, onComplete);
        }
    }

    public void Collapse(bool immediate = false, System.Action onComplete = null)
    {
        if (animationTween != null)
            animationTween.Kill();

        if (!isExpand && first)
            return;

        isExpand = false;
        first = true;

        if (immediate)
        {
            foreach (var panel in panels)
                PanelLerp(panel, 0f);
        }
        else
        {
            animationTween = Util.ManualTo((t) =>
            {
                t = 1f - t;
                foreach (var panel in panels)
                    PanelLerp(panel, t);
            }, animationDuration, onComplete);
        }
    }

    public void Toggle(bool immediate = false)
    {
        if (isExpand)
            Collapse(immediate);
        else
            Expand(immediate);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying && enablePreview)
        {
            if (previewExpand)
                previewProgress = Mathf.Clamp(previewProgress + 0.016f / animationDuration, 0f, 1f);
            else
                previewProgress = Mathf.Clamp(previewProgress - 0.016f / animationDuration, 0f, 1f);

            if (panels.Length == 0)
                return;

            foreach (var panel in panels)
                PanelLerp(panel, previewProgress);
        }
    }
}
