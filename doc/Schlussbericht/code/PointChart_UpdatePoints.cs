public void UpdatePoints()
{
    float scale = transform.lossyScale.x;

    var particles = new ParticleSystem.Particle[points.Length];
    particleSystem.Emit(points.Length);
    particleSystem.GetParticles(particles);
    for (int i = 0; i < points.Length; i++)
    {
        particles[i].size = Mathf.Lerp(0.3f, 0.05f, points.Length / 5000) * scale;
        particles[i].lifetime = particles[i].startLifetime = 100000f;
        particles[i].velocity = Vector3.zero;
        particles[i].position = (points[i] + pivotOffset) * scale;
        particles[i].color = new Color(points[i].x, points[i].y, points[i].z);
        particles[i].angularVelocity = Random.RandomRange(-45f, 45f);
    }
    particleSystem.SetParticles(particles, particles.Length);
}