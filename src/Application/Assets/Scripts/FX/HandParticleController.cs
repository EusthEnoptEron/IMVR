using UnityEngine;
using System.Collections;
using Gestures;
using System.Linq;
using Foundation;
using System.Collections.Generic;


/// <summary>
/// Controls the particle hands. Note that this required SkinnedHandModels.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class HandParticleController : MonoBehaviour {
    public HandType handType;

    private class ParticleMapping
    {
        public int Vertex1 = 0;
        public int Vertex2 = 0;
        public float Ratio = 0;
        public int Mesh;
    }

    private HandController _handController;
    private ParticleSystem _particleSystem;
    private ParticleSystem.Particle[] _particles;


    public bool throttle = false;
    public float throttleFPS = 20f;

    private Vector3[] _vertices;
    private MeshRenderer[] _renderers;
    private ParticleMapping[] _mappings;

    public int particlesPerMesh = 50;

    private BeatEventArgs beat = null;

	// Use this for initialization
	void Start () {
        _handController = HandProvider.Instance.GetComponent<HandController>();

        var model = (handType == HandType.Left
            ? _handController.leftGraphicsModel
            : _handController.rightGraphicsModel
        );

        if (model != null && model is SkeletalHand)
        {
            _particleSystem = GetComponent<ParticleSystem>();
            _renderers = model.GetComponentsInChildren<MeshRenderer>(true);
            var vList = new List<Vector3>();

            // Emit all particles
            int count = _renderers.Length * particlesPerMesh;

            _particles = new ParticleSystem.Particle[count];
            _mappings = new ParticleMapping[count];
            _particleSystem.Emit(count);

            _particleSystem.GetParticles(_particles);

            int i = 0;
            int meshCounter = 0;
            foreach (var renderer in _renderers)
            {
                int offset = vList.Count;
                vList.AddRange(renderer.GetComponent<MeshFilter>().sharedMesh.vertices);
                int vertexCount = vList.Count - offset;


                for (int j = 0; j < particlesPerMesh; j++)
                {
                    int v1, v2;
                    v1 = v2 = 0;
                    while (v1 == v2)
                    {
                        v1 = Random.Range(offset, offset + vertexCount);
                        v2 = Random.Range(offset, offset + vertexCount);
                    }

                    _mappings[i] = new ParticleMapping()
                    {
                         Ratio = Random.value,
                         Vertex1 = v1,
                         Vertex2 = v2,
                         Mesh = meshCounter
                    };

                    // Abuse seed to correlate
                    _particles[i].randomSeed = (uint)i;
                    _particles[i].size = 0.01f;
                    _particles[i].lifetime = _particles[i].startLifetime = 10000;

                    i++;
                }

                meshCounter++;

            }

            _particleSystem.SetParticles(_particles, count);
            _vertices = vList.ToArray();

            BeatDetector.Instance.Beat += OnBeat;

            StartCoroutine(UpdateParticles());
        }
        else
        {
            Debug.LogError("Hand must be rigged!");
            gameObject.SetActive(false);
        }
	}

    private void OnBeat(object sender, BeatEventArgs e)
    {
        beat = e;
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
                        _renderers = leapHand.GetComponentsInChildren<MeshRenderer>();
                        //_renderers.ToList().ForEach(r => r.enabled = false);
                        var mats = _renderers.Select(r => r.transform.localToWorldMatrix).ToArray();


                        // Found hand representation
                        _particleSystem.GetParticles(_particles);
                        {

                            // Scale needs to be ignored, so we are required to make our own transformation matrix
                            var color = Theme.Current.ActivatedColor;
                            
                            lastTransform = leapHand.transform;
                            var task = Task.Run(delegate
                            {
                                for (int i = 0; i < _particles.Length; i++)
                                {
                                    uint index = _particles[i].randomSeed;

                                    var v = mats[_mappings[index].Mesh].MultiplyPoint( 
                                        Vector3.Lerp(
                                            _vertices[_mappings[index].Vertex1],
                                            _vertices[_mappings[index].Vertex2], 
                                            _mappings[index].Ratio
                                        ) 
                                    );

                                    var distance = (v - _particles[i].position);
                                    var magnitude = distance.magnitude;

                                    _particles[i].velocity = distance * 0.1f;
                                    _particles[i].position = Vector3.Lerp(_particles[i].position, v, deltaTime * 10);
                                       
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
                    if (_visible)
                        Bang();
                    _visible = false;
                }

                lastUpdate = time;
            }

            beat = null;
            yield return null;
        }
	}

    private void Bang()
    {
        _particleSystem.GetParticles(_particles);

        for (int i = 0; i < _particles.Length; i++)
        {
            _particles[i].velocity = Random.insideUnitSphere;
        }

        _particleSystem.SetParticles(_particles, _particles.Length);
    }

}
