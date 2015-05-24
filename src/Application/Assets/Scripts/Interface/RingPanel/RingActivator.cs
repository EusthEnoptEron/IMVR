using UnityEngine;
using System.Collections;


[RequireComponent(typeof(FillableRing))]
public abstract class RingActivator : MonoBehaviour {
    public float fillDuration = 2;
    public float fallDuration = 4;

    private FillableRing _model;
    private bool _touched = false;
    private bool _activated = false;

    public void Fill()
    {
        _model.Fill += Time.deltaTime / fillDuration;
        _touched = true;
    }

	// Use this for initialization
	void Awake () {
        _model = GetComponent<FillableRing>();

        if (GetComponentInChildren<Collider>() == null)
            gameObject.AddComponent<MeshCollider>();

        //GetComponent<RingMesh>().Updated += delegate
        //{
        //    GameObject.DestroyImmediate(GetComponent<MeshCollider>());
        //    gameObject.AddComponent<MeshCollider>();
        //};
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (!_activated)
        {
            if (_model.Fill == 1)
            {
                // Activate!
                _activated = true;
                Activate();
            }
            else if (!_touched)
            {
                _model.Fill -= Time.deltaTime / fallDuration;
            }
        }
        else
        {
            if (!IsStillActive)
            {
                _activated = false;
                Deactivate();
            }
        }

        _touched = false;
	}
    protected abstract bool IsStillActive { get; }
    protected abstract void Activate();

    protected void Deactivate()
    {
        _model.Fill = 0;
    }
}
