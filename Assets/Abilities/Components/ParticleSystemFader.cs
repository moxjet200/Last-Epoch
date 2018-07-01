using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemFader : MonoBehaviour {

    ParticleSystem particleSystem = null;
    float fadePerSecond = 1f;

	// Use this for initialization
	void Start () {
        particleSystem = GetComponent<ParticleSystem>();
        if (!particleSystem) { this.enabled = false; }
    }
	
	// Update is called once per frame
	void Update () {
        fadeParticleSystem();
    }

    private void fadeParticleSystem()
    {
        Color colour = particleSystem.main.startColor.color;
        colour.a -= colour.a * (fadePerSecond * Time.deltaTime);
        ParticleSystem.MainModule settings = particleSystem.main;
        settings.startColor = new ParticleSystem.MinMaxGradient(colour);

        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.particleCount];

        //get the particles
        particleSystem.GetParticles(particles);

        //then iterate through the array changing the color 1 by 1
        for (int p = 0; p < particles.Length; p++)
        {
            Color c = particles[p].startColor;
            c.a -= c.a * (fadePerSecond * Time.deltaTime);
            particles[p].startColor = c;
        }

        //set the particles back to the system
        particleSystem.SetParticles(particles, particles.Length);
    }

    public void setFadePerSecond(float newFade)
    {
        fadePerSecond = newFade;
    }

}
