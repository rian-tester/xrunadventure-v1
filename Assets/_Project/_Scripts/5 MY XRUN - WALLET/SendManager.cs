using UnityEngine;
using UnityEngine.SceneManagement;

public class SendManager : MonoBehaviour
{
    public void GoToMyXrunbutton()
    {
        StartCoroutine(SceneLoader.LoadScene(5, 0.25f));
    }
    public void ScanAdress()
    {
        SceneManager.LoadScene(23);
    }
}
