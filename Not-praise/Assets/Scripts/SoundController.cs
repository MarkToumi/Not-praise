using UnityEngine;
using System.Collections;

public class SoundController : MonoBehaviour {

    public GameObject Right;
    public GameObject Left;
    public AudioClip[] voice;
    private AudioSource rightAS;
    private AudioSource leftAS;
    private PositionController pc;
    private bool gameSet;
   
	// Use this for initialization
	void Start () {        
        rightAS = Right.GetComponent<AudioSource>();
        leftAS = Left.GetComponent<AudioSource>();
        gameSet = true;
	}

	// Update is called once per frame
	void Update () {
	}

    public void CompletedSound()
    {
        if (gameSet)
        {
            if(!rightAS.isPlaying && !leftAS.isPlaying)
            {
                int rn = UnityEngine.Random.Range(0, 2);
                if(rn == 0)
                {
                    int r = UnityEngine.Random.Range(0, voice.Length);
                    rightAS.clip = voice[r];
                    rightAS.Play();
                    gameSet = false;
                }
                else if(rn == 1)
                {
                    int r = UnityEngine.Random.Range(0, voice.Length);
                    leftAS.clip = voice[r];
                    leftAS.Play();
                    gameSet = false;
                }
           }
        }
    }
    public bool isRightVoice()
    {
        return !rightAS.isPlaying;
    }

    public bool isLeftVoice()
    {
        return !leftAS.isPlaying;
    }
}
