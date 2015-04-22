using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Linq;
using System.Collections.Generic;

public abstract class View : MonoBehaviour {
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

    public virtual void OnDisable()
    {
        GetCanvasGroups().ToList().ForEach(FadeOut);
    }

    public virtual void OnEnable()
    {
        GetCanvasGroups().ToList().ForEach(FadeIn);
    }
}
