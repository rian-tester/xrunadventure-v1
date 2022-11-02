using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingManager : MonoBehaviour
{
    public void BackToHomeButtonclicked()
    {
        SceneManager.LoadScene(4);
    }
    public void DataOkButtonClicked()
    {
        Debug.Log("To update data!");
    }
}
