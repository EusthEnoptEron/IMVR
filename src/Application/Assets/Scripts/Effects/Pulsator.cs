using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Pulsator : MonoBehaviour {

    public void OnBeat(float rms)
    {
        Debug.Log(rms);

        var targetScale = Vector3.one * 1.5f;//(1 + rms * 2);


        if (targetScale.sqrMagnitude > transform.localScale.sqrMagnitude)
        {
            transform.DOKill(false);
            transform.DOScale(targetScale, 0.1f).OnComplete(delegate
            {
                transform.DOScale(Vector3.one, 1).SetEase(Ease.OutSine);
            });
        }
        
        //transform.DOPunchScale(Vector3.one * rms * 1.5f, 0.1f, 0);
    }
    
}
