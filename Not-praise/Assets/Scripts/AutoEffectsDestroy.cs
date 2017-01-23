using UnityEngine;
using System.Collections;

public class AutoEffectsDestroy : MonoBehaviour {

	private ParticleSystem particle;
	// Use this for initialization
	void Start () {
		particle = GetComponent<ParticleSystem>();
		Destroy(gameObject,particle.duration);
	}
	
	// Update is called once per frame
	void Update () {
	}

	public bool particlePlaying()
	{
		return particle.isPlaying;
	}
}
