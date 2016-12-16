using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using live2d;

public class SelectControll : MonoBehaviour {

    public GameObject haru;
    public GameObject chitose;
    public GameObject tsumiki;
    private ModelController haruMC;
    private ModelController chitoseMC;
    private ModelController tsumikiMC;
    private bool haruC = true;
    private bool chitoseC = true;
    private bool tsumikiC = true;
	// Use this for initialization
	void Start () {
        haruMC = haru.GetComponent<ModelController>();
        chitoseMC = chitose.GetComponent<ModelController>();
        tsumikiMC = tsumiki.GetComponent<ModelController>();
	}
	
	// Update is called once per frame
	void Update () {
        if (haruMC.MotionFinsh())
            haruC = true;
        if (chitoseMC.MotionFinsh())
            chitoseC = true;
        if (tsumikiMC.MotionFinsh())
            tsumikiC = true;
	}

    public void haru_Enter(BaseEventData eve)
    {
        if (haruC)
        {
            haruMC.MotionPlay();
            haruC = false;
        }
    }
    public void chitose_Enter(BaseEventData eve)
    {
        if (chitoseC)
        {
            chitoseMC.MotionPlay();
            chitoseC = false;
        }
    }

    public void tsumiki_Enter(BaseEventData eve)
    {
        if (tsumikiC)
        {
            tsumikiMC.MotionPlay();
            tsumikiC = false;
        }
    }
}
