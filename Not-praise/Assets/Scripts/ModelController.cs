using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using live2d;
using live2d.framework;

public class ModelController : MonoBehaviour {

    public string path;
    public TextAsset modelJson;
    private string modelpath;
    private TextAsset mocFile;
    private Texture2D[] textures;
    private TextAsset[] idleFiles;
    private AudioClip[] idleSound;
    private int[] idleFadeIn;
    private int[] idleFadeOut;
    private TextAsset[] mtnFiles;
    private AudioClip[] mtnSound;
    private int[] mtnFadeIn;
    private int[] mtnFadeOut;
    private TextAsset poseJson;
    private TextAsset physicsJson;
    private AudioSource audio;
    private Live2DModelUnity live2DModel;
    private Live2DMotion motion;
    private MotionQueueManager motionManager;
    private Matrix4x4 live2DCanvasPos;
    private L2DPose pose;
    private L2DPhysics physics;
	// Use this for initialization
	void Start () {
        Live2D.init();
        Json_Read();
        audio = GetComponent<AudioSource>();
        motionManager = new MotionQueueManager();
        motion = Live2DMotion.loadMotion(idleFiles[0].bytes);
        live2DModel.saveParam();
        float modelWidth = live2DModel.getCanvasWidth();
        live2DCanvasPos = Matrix4x4.Ortho(0, modelWidth, modelWidth, 0, -50.0f, 50.0f);
	}

    void Json_Read()
    {
        char[] buf = modelJson.text.ToCharArray();
        Value json = Json.parseFromBytes(buf);

        modelpath = path + "/";
        mocFile = new TextAsset();
        mocFile = (Resources.Load(modelpath + json.get("model").toString(), typeof(TextAsset)) as TextAsset);
        live2DModel = Live2DModelUnity.loadModel(mocFile.bytes);

        int texture_num = json.get("textures").getVector(null).Count;
        textures = new Texture2D[texture_num];
        for (int i = 0; i < texture_num; i++)
        {
            string texturenm = Regex.Replace(modelpath + json.get("textures").get(i).toString(), ".png$", "");
            textures[i] = (Resources.Load(texturenm, typeof(Texture2D)) as Texture2D);
            live2DModel.setTexture(i, textures[i]);
        }

        Value idlepath = json.get("motions").get("idle");
        int idle_num = idlepath.getVector(null).Count;
        idleFiles = new TextAsset[idle_num];
        idleSound = new AudioClip[idle_num];
        idleFadeIn = new int[idle_num];
        idleFadeOut = new int[idle_num];
        for (int i = 0; i < idle_num; i++)
        {
            idleFiles[i] = (Resources.Load(modelpath + idlepath.get(i).get("file").toString()) as TextAsset);
            if (idlepath.get(i).getMap(null).ContainsKey("sound"))
            {
                string soundnm = Regex.Replace(Regex.Replace(modelpath + idlepath.get(i).get("sound").toString(), ".mp3$", ""), ".wav$", "");
                idleSound[i] = (Resources.Load(soundnm, typeof(AudioClip)) as AudioClip);
            }
            if (idlepath.get(i).getMap(null).ContainsKey("fade_in"))
                idleFadeIn[i] = idlepath.get(i).get("fade_in").toInt();
            if (idlepath.get(i).getMap(null).ContainsKey("fade_out"))
                idleFadeOut[i] = idlepath.get(i).get("fade_out").toInt();
        }

        Value mtnpath = json.get("motions").get("");
        int mtn_num = mtnpath.getVector(null).Count;
        mtnFiles = new TextAsset[mtn_num];
        mtnSound = new AudioClip[mtn_num];
        mtnFadeIn = new int[mtn_num];
        mtnFadeOut = new int[mtn_num];

        for (int i = 0; i < mtn_num; i++)
        {
            mtnFiles[i] = (Resources.Load(modelpath + mtnpath.get(i).get("file").toString()) as TextAsset);
            if (mtnpath.get(i).getMap(null).ContainsKey("sound"))
            {
                string soundnm = Regex.Replace(Regex.Replace(modelpath + mtnpath.get(i).get("sound").toString(), ".mp3$", ""), ".wav$", "");
                mtnSound[i] = (Resources.Load(soundnm, typeof(AudioClip)) as AudioClip);
            }
            if (mtnpath.get(i).getMap(null).ContainsKey("fade_in"))
                mtnFadeIn[i] = mtnpath.get(i).get("fade_in").toInt();
            else
                mtnFadeIn[i] = 1000;
            if (mtnpath.get(i).getMap(null).ContainsKey("fade_out"))
                mtnFadeOut[i] = mtnpath.get(i).get("fade_out").toInt();
            else
                mtnFadeOut[i] = 1000;
        }
        
        if(json.getMap(null).ContainsKey("pose"))
        {
            Value posepath = json.get("pose");
            poseJson = new TextAsset();
            poseJson = (Resources.Load(modelpath + posepath.toString(), typeof(TextAsset)) as TextAsset);
            pose = L2DPose.load(poseJson.bytes);
        }

        if(json.getMap(null).ContainsKey("physics"))
        {
            Value physicspath = json.get("physics");
            physicsJson = new TextAsset();
            physicsJson = (Resources.Load(modelpath + physicspath.toString(), typeof(TextAsset)) as TextAsset);
            physics = L2DPhysics.load(physicsJson.bytes);
        }
    }

	// Update is called once per frame
    void Update()
    {
        live2DModel.loadParam();
        if (motionManager.isFinished())
        {
            int rn = UnityEngine.Random.Range(0, idleFiles.Length);
            motion = Live2DMotion.loadMotion(idleFiles[rn].bytes);
            motion.setFadeIn(idleFadeIn[rn]);
            motion.setFadeOut(idleFadeOut[rn]);
            motionManager.startMotion(motion, false);
        }
        if(pose != null)
            pose.updateParam(live2DModel);
        if (physics != null)
            physics.updateParam(live2DModel);
        motionManager.updateParam(live2DModel);
        live2DModel.saveParam();
        
        live2DModel.update();
    }

    void OnRenderObject()
    {
        if (live2DModel == null) return;
        live2DModel.setMatrix(transform.localToWorldMatrix * live2DCanvasPos);
        live2DModel.draw();
    }

    void motionSet()
    {
        int rn = UnityEngine.Random.Range(0, mtnFiles.Length);
        motion = Live2DMotion.loadMotion(mtnFiles[rn].bytes);
        motion.setFadeIn(mtnFadeIn[rn]);
        motion.setFadeOut(mtnFadeOut[rn]);
        motionManager.startMotion(motion, false);
        if (mtnSound[rn] != null)
        {
            audio.clip = mtnSound[rn];
            audio.Play();
        }
    }

    void motionSet(int n)
    {
        int rn = UnityEngine.Random.Range(0, mtnFiles.Length);
        motion = Live2DMotion.loadMotion(mtnFiles[rn].bytes);
        motion.setFadeIn(mtnFadeIn[rn]);
        motion.setFadeOut(mtnFadeOut[rn]);
        motionManager.startMotion(motion, false);
        if (mtnSound[rn] != null)
        {
            audio.clip = mtnSound[rn];
            audio.Play();
        }
    }

    public void ExpressionChange(int n = 0)
    {

        motionSet(n);
    }

    public void MotionPlay()
    {
        motionSet();
    }

    public bool MotionFinsh()
    {
        return motionManager.isFinished();
    }
}
