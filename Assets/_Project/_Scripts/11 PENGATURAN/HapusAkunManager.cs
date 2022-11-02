using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HapusAkunManager : MonoBehaviour
{
    [SerializeField] UltimateUICheckbox layananCheck;
    [SerializeField] UltimateUICheckbox fiturCheck;
    [SerializeField] UltimateUICheckbox lainLainCheck;
    private void Awake()
    {
        layananCheck.OnChecked.AddListener(LayananCheckbox);
        fiturCheck.OnChecked.AddListener(LayananCheckbox);
        lainLainCheck.OnChecked.AddListener(LayananCheckbox);
    }
    private void OnDestroy()
    {
        layananCheck.OnChecked.RemoveListener(LayananCheckbox);
        fiturCheck.OnChecked.RemoveListener(LayananCheckbox);
        lainLainCheck.OnChecked.RemoveListener(LayananCheckbox);
    }
    public void GoBackToPengaturan()
    {
        StartCoroutine(SceneLoader.LoadScene(11, 0.25f));
        //SceneManager.LoadScene(11);
    }
    public void ConfirmDeleteButton()
    {
        var deletePrompt = GetComponent<HapusAkunPrompt>();
        deletePrompt.AgreeToDelete();
    }
    public void LayananCheckbox(UltimateUICheckbox switchObject)
    {
        //print(switchObject.gameObject.name);
        //print(switchObject.Value);
    }
    public void FiturCheckbox(UltimateUICheckbox switchObject)
    {
        //print(switchObject.gameObject.name);
        //print(switchObject.Value);
    }
    public void LainLainCheckbox(UltimateUICheckbox switchObject)
    {
        //print(switchObject.gameObject.name);
        //print(switchObject.Value);
    }
    public List<UltimateUICheckbox> GetAllCheckbox()
    {
        var list = new List<UltimateUICheckbox>();
        list.Add(layananCheck);
        list.Add(fiturCheck);
        list.Add(lainLainCheck);
        return list;
    }
}
