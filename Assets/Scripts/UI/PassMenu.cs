using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassMenu : MonoBehaviour
{
    RectTransform _rectTransform;
    // Start is called before the first frame update
    void Start()
    {
    _rectTransform= GetComponent<RectTransform>();


        GameManager.OnPlayerWin += HandleWin;

    }

    private void HandleWin()
    {
       // _rectTransform.localScale = Vector3.one;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
