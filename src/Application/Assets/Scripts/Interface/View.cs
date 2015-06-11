using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Linq;
using System.Collections.Generic;
using System;

public enum InteractionMode
{
    Disabled,
    Enabled,
    Partly
}

public abstract class View : MonoBehaviour {
    private int level = 0;
    private CanvasGroup[] _canvasGroups;

    public InteractionMode Interaction
    {
        get;
        private set;
    }

    protected virtual void Awake()
    {
        name = this.GetType().FullName;
        transform.position = World.CameraRig.transform.position;
        transform.rotation = World.WorldNode.transform.rotation;
    }


    protected void FadeIn(CanvasGroup group)
    {
        if (group.alpha >= 0.9f) group.alpha = 0;

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

    protected void FinishInitialization()
    {
        foreach (var group in GetCanvasGroups())
        {
            group.alpha = 0;
        }

    }

    protected IEnumerable<CanvasGroup> GetCanvasGroups()
    {
        return GetComponentsInChildren<Canvas>(true).Select(canvas =>
        {
            var group = canvas.GetComponent<CanvasGroup>();
            if (!group) group = canvas.gameObject.AddComponent<CanvasGroup>();
            return group;
        });

    }

    public void Disable(bool destroy = false)
    {
        StartCoroutine(InvokeInNextFrame(delegate
        {
            SetInteraction(InteractionMode.Disabled);
            OnViewDisable();

            // Destroy after 5 seconds if not used anymore
            if (destroy)
                Destroy(gameObject, 5);
        }));
    }

    public void Enable()
    {
        gameObject.SetActive(true);
        StartCoroutine(InvokeInNextFrame(delegate
        {
            SetInteraction(level == 0 ? InteractionMode.Enabled : InteractionMode.Disabled);
            OnViewEnable();
        }));
    }

    public virtual void SetInteraction(InteractionMode mode)
    {
        Interaction = mode;
        this.enabled = mode == InteractionMode.Enabled;

        float alpha = 1;
        if (mode == InteractionMode.Partly || mode == InteractionMode.Disabled)
            alpha = 0.5f;

        foreach (var group in GetCanvasGroups())
        {
            //group.interactable = enabled;
            group.blocksRaycasts = enabled;
            group.Fade(alpha, 0.5f);
        }

        var circle = GetComponent<CylinderLayout>();
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
            //transform.SetParent(World.WorldNode.transform);
            SetInteraction(InteractionMode.Disabled);
        }

        OnPush();
    }

    public void Pull()
    {
        level--;

        if (level == 0)
        {
            transform.SetParent(null);
            SetInteraction(InteractionMode.Enabled);
        }

        OnPull();
    }

    protected virtual void OnPush()
    {
        var circle = GetComponentInChildren<CylinderLayout>();
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
        var circle = GetComponentInChildren<CylinderLayout>();
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

    public virtual void BuildMenu(RingMenu menuBase) {
    }


    private IEnumerator InvokeInNextFrame(Action action)
    {
        yield return null;
        action();
    }

}
