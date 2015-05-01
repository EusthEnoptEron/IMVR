using UnityEngine;
using System.Collections;


[RequireComponent(typeof(ParticleSystem))]
public class ParticleSpitter : MonoBehaviour {
    public float countMultiplier = 100;

    private ParticleSystem particles;
	// Use this for initialization
	void Start () {
        particles = GetComponent<ParticleSystem>();

	}
	
	// Update is called once per frame
	void Update () {
	}

    public void OnBeat(float rms)
    {
        ParticleSystem.Particle[] partlc = new ParticleSystem.Particle[(int)(rms * countMultiplier)];

        particles.Emit((int)(rms * countMultiplier));
        particles.GetParticles(partlc);

        foreach (var particle in partlc)
        {
            var r = Random.onUnitSphere;
            particle.color = new Color(r.x, r.y, r.z);
        }

    }
}
