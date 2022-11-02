using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class FaqItem : MonoBehaviour
{

    [SerializeField] Text questionTextKR;
    [SerializeField] Text answerTextKR;

    public string Question { get; set; }
    public string Answer { get; set; }

    public void Setup()
    {
        questionTextKR.text = this.Question;
        answerTextKR.text = this.Answer;
    }
}
