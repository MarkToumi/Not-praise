using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    string sceneName;
	// Use this for initialization
	void Start () {
        sceneName = SceneManager.GetActiveScene().name;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SceneMove(int n = 0)
    {
        Debug.Log(sceneName);
        switch(sceneName)
        {
            case "chitose":
            case "haru":
            case "tsumiki":
                SceneManager.LoadScene("Title");
                break;
            case "Title":
                SceneManager.LoadScene("StageSelect");
                break;
            case "StageSelect":
                CharacterSelect(n);
                break;
            default:
                break;
        }
    }

    void  CharacterSelect(int n)
    {
        switch(n)
        {
            case 0:
                SceneManager.LoadScene("haru");
                break;
            case 1:
                SceneManager.LoadScene("chitose");
                break;
            case 2:
                SceneManager.LoadScene("tsumiki");
                break;
            default:
                break;
        }
    }

    public void gameQuit()
    {
        Application.Quit();
    }
}
