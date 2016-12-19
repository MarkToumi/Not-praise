using UnityEngine;
using System.Collections;

public class PositionController : MonoBehaviour {
    public Transform Right;
    public Transform Left;
    private Vector3 cameraPos;
    private Vector3 rightPos;
    private Vector3 leftPos;
	// Use this for initialization
	void Start () {
        cameraPos = Camera.main.transform.position;
        rightPos = new Vector3(cameraPos.x + 3, cameraPos.y, cameraPos.z);
        leftPos = new Vector3(cameraPos.x - 3, cameraPos.y, cameraPos.z);
	}

	// Update is called once per frame
	void Update () {
	}

    public void moveON()
    {
        Right.position = rightPos;
        Left.position = leftPos;
    }
}
