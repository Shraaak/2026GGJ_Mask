using UnityEngine;
using UnityEngine.SceneManagement;

public class Retry : MonoBehaviour
{
    public void LoadScene1()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}