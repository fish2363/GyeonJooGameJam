using UnityEngine;
using UnityEngine.SceneManagement;

public class MainmenuLogic : MonoBehaviour
{
    public void GameStart()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void GameExit()
    {

    }
}
