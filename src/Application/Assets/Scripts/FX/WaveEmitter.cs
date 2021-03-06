﻿using UnityEngine;
using System.Collections;
using DG.Tweening;

public class WaveEmitter : MonoBehaviour {
    public Material material;
    public float duration = 2;
    public float radius = 20f;
    public int renderQueue = 2910;
    public void OnBeat(float strength)
    {
        SendWave(strength * 20);
    }

    private void SendWave(float width)
    {
        var wave = new GameObject().AddComponent<RingMesh>();
        wave.transform.SetParent(transform, false);
        
        wave.InnerRadius = 1;
        wave.OuterRadius = wave.InnerRadius + width;
        wave.Color = Theme.Current.ActivatedColor;
        wave.GetComponent<MeshRenderer>().material = material;
        wave.GetComponent<MeshRenderer>().material.renderQueue = renderQueue;

        DOTween.To(
            () => wave.InnerRadius,
            x =>
            {
                wave.InnerRadius = x;
                wave.OuterRadius = x + width;
            },
            radius,
            duration
        ).OnComplete(delegate
        {
            GameObject.Destroy(wave.gameObject);
        });
    }
}

