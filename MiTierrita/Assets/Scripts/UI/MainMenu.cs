using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartButtonOnClick()
    {
        SceneManager.LoadSceneAsync(0);
    }
    public void ExitButtonOnClick()
    {
        Application.Quit();
    }
}
