using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public void ResumeButton()
    {
        Debug.Log("Resume");
    }
    public void ExitButton()
    {
        SceneManager.LoadSceneAsync(1);
    }

}
