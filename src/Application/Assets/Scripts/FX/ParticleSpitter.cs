using UnityEngine;
using System.Collections;


[RequireComponent(typeof(ParticleSystem))]
public class ParticleSpitter : MonoBehaviour {
    public float countMultiplier = 100;

    ParticleSystem m_System;
    ParticleSystem.Particle[] m_Particles;

	// Use this for initialization
	void Start () {
        m_System = GetComponent<ParticleSystem>();

	}
	
	// Update is called once per frame
	void Update () {
	}

    public void OnBeat(float rms)
    {
        InitializeIfNeeded();

        int emitted = (int)(rms * countMultiplier);
        m_System.Emit(emitted);

        int numParticlesAlive = m_System.GetParticles(m_Particles);

        for (int i = numParticlesAlive - emitted; i < m_Particles.Length; i++)
        {
            var r = Random.onUnitSphere;
            m_Particles[i].color = new Color(r.x, r.y, r.z);
        }

        m_System.SetParticles(m_Particles, numParticlesAlive);
    }

    void InitializeIfNeeded()
    {
        if (m_System == null)
            m_System = GetComponent<ParticleSystem>();

        if (m_Particles == null || m_Particles.Length < m_System.maxParticles)
            m_Particles = new ParticleSystem.Particle[m_System.maxParticles];
    }

}
