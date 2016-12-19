using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using live2d;

public class Favorability : MonoBehaviour
{

    public Slider Favorabillity;
    public GameObject live2DModel;
    public Image curtains;
    public float fadeSpeed;
    private float fav;
    private int favInt;
    private bool isON;
    private bool upFlag;
    private bool downFlag;
    private bool gameEnd;
    private ModelController mc;
    private SceneChanger sc;
    private SoundController sound;
    private PositionController pc;
    private float r;
    private float g;
    private float b;
    private float a;
    private float diff;
	// Use this for initialization
	void Start () {
        curtains.enabled = false;
        fav = Favorabillity.value;
        diff = fav;
        isON = false;
        upFlag = false;
        downFlag = false;
        gameEnd = false;

        mc = live2DModel.GetComponent<ModelController>();
        sc = GetComponent<SceneChanger>();
        sound = GetComponent<SoundController>();
        pc = GetComponent<PositionController>();
        r = curtains.color.r;
        g = curtains.color.g;
        b = curtains.color.b;
        a = curtains.color.a;
	}
	
	// Update is called once per frame
	void Update () {
        if (mc.MotionFinsh())
        {
            FavChangeOn();
            if(fav == Favorabillity.maxValue || fav == Favorabillity.minValue)
            {
                curtains.enabled = true;
                gameEnd = true;
                if (fav == Favorabillity.maxValue)
                {
                    r = 1f;
                    g = 1f;
                    b = 1f;
                }
                else if(fav == Favorabillity.minValue)
                {
                    r = 0f;
                    g = 0f;
                    b = 0;
                }
            }
        }
        if (gameEnd)
        {
            a += fadeSpeed;
            if (a >= 1f)
                a = 1f;
            curtains.color = new Color(r, g, b, a);
        }
        if (a >= 0.75f)
        {
            pc.moveON();
            sound.CompletedSound();
        }
	}

    void LateUpdate()
    {
        if (gameEnd && sound.isRightVoice() && sound.isLeftVoice() && a == 1f)
            Invoke("GameCompleted", 0.5f);
    }

    public void upOn(BaseEventData eve)
    {
        isON = true;
        upFlag = true;
    }

    public void downOn(BaseEventData eve)
    {
        isON = true;
        downFlag = true;
    }


    public void FavChangeOn(BaseEventData eve)
    {
        isON = true;
        int rn = UnityEngine.Random.Range(0, 2);
        switch (rn)
        {
            case 0:
                upFlag = true;
                break;
            case 1:
                downFlag = true;
                break;
            default:
                break;
        }
    }

    void FavChangeOn()
    {
        isON = true;
        int rn = UnityEngine.Random.Range(0, 2);
        switch (rn)
        {
            case 0:
                upFlag = true;
                break;
            case 1:
                downFlag = true;
                break;
            default:
                break;
        }
    }

    public void FavChange(BaseEventData eve)
    {
        if(isON)
        {
            if (upFlag)
                FavUp();
            else if (downFlag)
                FavDown();
            Favorabillity.value = fav;
            favInt = (int)fav;
            if (favInt % 100 == 0)
            {
                upFlag = false;
                downFlag = false;
                isON = false;
                if (fav > diff)
                    mc.ExpressionChange(0);
                else if (fav < diff)
                    mc.ExpressionChange(1);
                diff = fav;
            }
        }
    }

    void FavUp()
    {
        fav += 1;
        if (fav >= Favorabillity.maxValue)
            fav = Favorabillity.maxValue;
    }

    void FavDown()
    {
        fav -= 1;
        if (fav <= Favorabillity.minValue)
            fav = Favorabillity.minValue;
    }

    public void FavChangeOff(BaseEventData eve)
    {
        isON = false;
        upFlag = false;
        downFlag = false;
    }

    public void motionSet(BaseEventData eve)
    {
        mc.MotionPlay();
    }

    void GameCompleted()
    {
        sc.SceneMove();
    }
}
