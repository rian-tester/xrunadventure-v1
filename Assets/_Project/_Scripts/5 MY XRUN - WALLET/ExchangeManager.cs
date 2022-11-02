using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExchangeManager : MonoBehaviour
{
    public void GoToMyXrunbutton()
    {
        StartCoroutine(SceneLoader.LoadScene(5, 0.25f));
    }
}
