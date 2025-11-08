using Ami.BroAudio;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectNextScene : MonoBehaviour
{
    public string sceneName;
    [SerializeField] private SoundID meow;

    public void SceneStart()
    {
        BroAudio.Play(meow);
        SceneManager.LoadScene(sceneName);
    }
}
