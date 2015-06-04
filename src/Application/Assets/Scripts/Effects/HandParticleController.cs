using UnityEngine;
using System.Collections;
using Gestures;
using System.Linq;


/// <summary>
/// Controls the particle hands. Note that this required SkinnedHandModels.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class HandParticleController : MonoBehaviour {
    public HandType handType;


    private HandController _handController;
    private ParticleSystem _particleSystem;
    private ParticleSystem.Particle[] _particles;

    private Mesh _bakedMesh;
    public Vector3 rotationOffset = Vector3.zero;
    private Quaternion _rotationOffset;

    public bool throttle = false;
    public float throttleFPS = 20f;

	// Use this for initialization
	void Start () {
        _handController = HandProvider.Instance.GetComponent<HandController>();

        var model = (handType == HandType.Left
            ? _handController.leftGraphicsModel
            : _handController.rightGraphicsModel
        );

        if (model != null && model is RiggedHand)
        {
            _particleSystem = GetComponent<ParticleSystem>();
            int count = model.transform.Descendants().First(d => d.GetComponent<SkinnedMeshRenderer>()).GetComponent<SkinnedMeshRenderer>().sharedMesh.vertexCount;

            // Emit particles for now.
            _particleSystem.Emit(count);
            {
                _particles = new ParticleSystem.Particle[count];
                _particleSystem.GetParticles(_particles);
                for (uint i = 0; i < _particles.Length; i++)
                {
                    _particles[i].randomSeed = i;
                    _particles[i].lifetime = _particles[i].startLifetime = 10000;
                    _particles[i].size = 0.01f;
                }
                _particleSystem.SetParticles(_particles, _particles.Length);
            }

            _rotationOffset = Quaternion.Euler(rotationOffset);

            _bakedMesh = new Mesh();
        }
        else
        {
            Debug.LogError("Hand must be rigged!");
            gameObject.SetActive(false);
        }
	}

    private float lastUpdate = 0;
	// Update is called once per frame
	void Update () {
        if (!throttle || (Time.time - lastUpdate) > (1 / throttleFPS))
        {
            var hand = HandProvider.Instance.GetHand(handType);
            if (hand != null)
            {

                var leapHand = _handController.GetAllGraphicsHands().FirstOrDefault(h => h.GetLeapHand().Id == hand.Id);
                if (leapHand != null)
                {
                    var handContainer = leapHand.transform.FindChild("HandContainer");

                    // Found hand representation

                    // Bake mesh
                    var skinnedMeshRenderer = leapHand.GetComponentInChildren<SkinnedMeshRenderer>();
                    skinnedMeshRenderer.enabled = false;
                    skinnedMeshRenderer.BakeMesh(_bakedMesh);

                    _particleSystem.GetParticles(_particles);
                    {
                        var vertices = _bakedMesh.vertices;
                        for (int i = 0; i < _particles.Length; i++)
                        {
                            var v = leapHand.transform.TransformPoint(_rotationOffset * vertices[_particles[i].randomSeed]);

                            //_particles[i].position = v;
                            //_particles[i].velocity = Vector3.zero;

                            var distance = (v - _particles[i].position);
                            var magnitude = distance.magnitude;
                            if (magnitude > 0.01f)
                                _particles[i].velocity = distance * 10;
                            else
                                _particles[i].velocity = distance;

                            //if(i == 0)
                            //    Debug.LogError(distance);

                            _particles[i].size = (Mathf.Sin((Time.time + _particles[i].randomSeed) * 2) + 1.5f) * 0.005f;
                        }
                    }
                    _particleSystem.SetParticles(_particles, _particles.Length);
                }
            }

            lastUpdate = Time.time;
        }
	}
}
