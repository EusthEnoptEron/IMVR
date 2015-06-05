using UnityEngine;
using System.Collections;
using Gestures;
using System.Linq;
using Foundation;


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

            StartCoroutine(UpdateParticles());
        }
        else
        {
            Debug.LogError("Hand must be rigged!");
            gameObject.SetActive(false);
        }
	}

    private float lastUpdate = 0;

    private Transform lastTransform;
    private float _lastTime = 0;
    private bool _visible = false;
	// Update is called once per frame
	IEnumerator UpdateParticles () {
        while (true)
        {
            if (!throttle || (Time.time - lastUpdate) > (1 / throttleFPS))
            {
                var hand = HandProvider.Instance.GetHand(handType);
                float time = Time.time;
                float deltaTime = time - lastUpdate;
                if (hand != null)
                {

                    var leapHand = _handController.GetAllGraphicsHands().FirstOrDefault(h => h.GetLeapHand().Id == hand.Id);
                    if (leapHand != null)
                    {
                        bool appeared = !_visible;
                        _visible = true;
                        _rotationOffset = Quaternion.Euler(rotationOffset);

                        var handContainer = leapHand.transform.FindChild("HandContainer");

                        // Found hand representation

                        // Bake mesh
                        var skinnedMeshRenderer = leapHand.GetComponentInChildren<SkinnedMeshRenderer>();
                        skinnedMeshRenderer.enabled = false;
                        skinnedMeshRenderer.BakeMesh(_bakedMesh);

                        _particleSystem.GetParticles(_particles);
                        {
                            var vertices = _bakedMesh.vertices;

                            // Scale needs to be ignored, so we are required to make our own transformation matrix
                            var M1 = Matrix4x4.TRS(skinnedMeshRenderer.transform.localPosition, skinnedMeshRenderer.transform.localRotation, Vector3.one);
                            var M2 = Matrix4x4.TRS(leapHand.transform.localPosition, leapHand.transform.localRotation, Vector3.one);
                            var color = Theme.Current.ActivatedColor;
                            var mat = M2 * M1;

                            lastTransform = leapHand.transform;
                            var task = Task.Run(delegate
                            {
                                for (int i = 0; i < _particles.Length; i++)
                                {
                                    var v = mat.MultiplyPoint((_rotationOffset * vertices[_particles[i].randomSeed]));

                                    //if (i == 0)
                                    //    Task.RunOnMain(() => { Debug.Log(deltaTime); });

                                    //_particles[i].position = Vector3.Lerp(_particles[i].position, v, deltaTime * 10);
                                    //_particles[i].velocity = Vector3.zero;

                                    var distance = (v - _particles[i].position);
                                    var magnitude = distance.magnitude;

                                    //if(magnitude > 0.01f)
                                    //    _particles[i].velocity = Vector3.Lerp(_particles[i].velocity, distance * 10, deltaTime * 10);
                                    //else
                                        _particles[i].velocity = distance * 0.1f;
                                        _particles[i].position = Vector3.Lerp(_particles[i].position, v, deltaTime * 10);
                                       

                                    //if(i == 0)
                                    //    Debug.LogError(distance);

                                    _particles[i].size = (Mathf.Sin((time + _particles[i].randomSeed) * 2) + 1.5f) * 0.002f;
                                    _particles[i].color = color;
                                }
                            });

                            yield return StartCoroutine(task.WaitRoutine());
                        }
                        _particleSystem.SetParticles(_particles, _particles.Length);
                    }
                }
                else
                {
                    _visible = false;
                }

                lastUpdate = time;
            }

            yield return null;
        }
	}

    public void OnDrawGizmos()
    {
        if (lastTransform)
        {
            Gizmos.color = Color.red;
            //Gizmos.matrix = lastTransform.worldToLocalMatrix;
            Gizmos.DrawMesh(_bakedMesh, lastTransform.localPosition, lastTransform.localRotation, lastTransform.localScale);
        }
    }
}
