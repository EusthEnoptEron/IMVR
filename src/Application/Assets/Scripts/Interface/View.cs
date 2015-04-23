using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Linq;
using System.Collections.Generic;

public abstract class View : MonoBehaviour {
    private int level = 0;

    protected virtual void Awake()
    {
        name = this.GetType().FullName;
        transform.position = World.CameraRig.transform.position;
    }

    protected void FadeIn(CanvasGroup group)
    {
        if (group.alpha == 1) group.alpha = 0;
        gameObject.SetActive(true);
        group.DOKill();
        group.DOFade(1, 1f);
    }

    protected void FadeOut(CanvasGroup group)
    {
        group.DOKill();
        group.DOFade(0, 1f).OnComplete(delegate
        {
            gameObject.SetActive(false);
        });
    }

    protected IEnumerable<CanvasGroup> GetCanvasGroups()
    {
        return GetComponentsInChildren<Canvas>().Select(canvas =>
        {
            var group = canvas.GetComponent<CanvasGroup>();
            if (!group) group = canvas.gameObject.AddComponent<CanvasGroup>();
            return group;
        });

    }

    public void Disable()
    {
        SetInteraction(false);
        OnViewDisable();
    }

    public void Enable()
    {
        SetInteraction(true);
        OnViewEnable();
    }

    public virtual void SetInteraction(bool enabled)
    {
        this.enabled = enabled;
        foreach (var group in GetCanvasGroups())
        {
            group.interactable = enabled;
            group.blocksRaycasts = enabled;
            group.Fade(enabled ? 1 : 0.5f, 0.5f);
        }

        var circle = GetComponent<CircleLayout>();
        if (circle)
        {
            var raycaster = circle.GetComponent<CylinderRaycaster>();
            if (raycaster)
                raycaster.enabled = enabled;
        }
    }

    public void Push()
    {
        level++;

        if (level == 1)
        {
            transform.SetParent(World.WorldNode.transform);
            SetInteraction(false);
        }

        OnPush();
    }

    public void Pull()
    {
        level--;

        if (level == 0)
        {
            transform.SetParent(null);
            SetInteraction(true);
        }

        OnPull();
    }

    protected virtual void OnPush()
    {
        var circle = GetComponent<CircleLayout>();
        if (circle != null)
        {
            // Increase radius
            DOTween.To(
                () => circle.radius,
                val => circle.radius = val,
                circle.radius + 1,
            1).Play();

        }
    }

    protected virtual void OnPull()
    {
        var circle = GetComponent<CircleLayout>();
        if (circle != null)
        {
            // Increase radius
            DOTween.To(
                () => circle.radius,
                val => circle.radius = val,
                circle.radius - 1,
            1).Play();
        }
    }

    protected virtual void OnViewDisable()
    {
        GetCanvasGroups().ToList().ForEach(FadeOut);
    }

    protected virtual void OnViewEnable()
    {
        GetCanvasGroups().ToList().ForEach(FadeIn);
    }
}
