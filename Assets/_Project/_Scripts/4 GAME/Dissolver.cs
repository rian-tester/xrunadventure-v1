using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolver : MonoBehaviour
{
    [SerializeField] MeshRenderer coinMaterial;
    float lerp;
    public bool isDissolving;

    void Update()
    {
        if (isDissolving)
        {
            lerp = Mathf.Lerp(0, 1f,1.5f);
            coinMaterial.materials[0].SetFloat("Vector1_6b2e70487f2a43dcafcadc5610c187fc", lerp);
        }
        
    }
}
