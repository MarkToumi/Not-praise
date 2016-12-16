using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class charLabel : MonoBehaviour {

    public Transform target;
    private RectTransform rectTransform;
    private Vector3 pos;
	// Use this for initialization
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

	void Start () {
        pos = new Vector3(target.position.x, target.position.y - 2.0f, target.position.z);
	}
	
	// Update is called once per frame
	void Update () {
        rectTransform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, pos);
	}
}
