using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Pulsator : MonoBehaviour {


    public float maxScale = 1.5f;

    private Vector3 min;
    private Vector3 max;
    public void Start()
    {
        min = transform.localScale;
        max = transform.localScale * maxScale;
    }

    public void OnBeat(float rms)
    {
        //Debug.Log(rms);

        //var targetScale = Vector3.one * 1.5f;//(1 + rms * 2);


        if (max.sqrMagnitude > transform.localScale.sqrMagnitude)
        {
            transform.DOKill(false);
            transform.DOScale(max, 0.1f).OnComplete(delegate
            {
                transform.DOScale(min, 1).SetEase(Ease.OutSine);
            });
        }
        
        //transform.DOPunchScale(Vector3.one * rms * 1.5f, 0.1f, 0);
    }
    
}
