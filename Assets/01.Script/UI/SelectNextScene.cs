using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectNextScene : MonoBehaviour
{
    public string sceneName;
    public void SceneStart()
    {
        SceneManager.LoadScene(sceneName);
    }
}
