using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelSelectorButton : MonoBehaviour
{
    [SerializeField]
    public int level;
    [SerializeField]
    TextMeshProUGUI _text;
    // Start is called before the first frame update
    private void Awake()
    {
        _text =  GetComponentInChildren<TextMeshProUGUI>();
    }
    void Start()
    {
        _text.text = level.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GoToLevel()
    {
        Loader.Load("Level "+level);
    }
}
