using System.Collections;
using System.Collections.Generic;
using UnityEditor.XR;
using UnityEngine;


[ExecuteInEditMode]
public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    Transform _platform;
    [SerializeField]
    Transform _start;
    [SerializeField]
    Transform _end;
    [SerializeField]
    float _speed;

    Vector2[] _positions = new Vector2[2];
    int _curr = 1;
    // Start is called before the first frame update
    private void Awake()
    {

    }
    void Start()
    {
        _positions[0] = _start.position;
        _positions[1] = _end.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Application.isPlaying && (_start.hasChanged || _end.hasChanged))
        {
            _platform.position = 0.5f * (_start.position + _end.position);

            Debug.Log("Editor");
            return;
        }
        else
        {
            Debug.Log("Gamee");
            _platform.position = Vector2.Lerp(_platform.position, _positions[_curr],Time.deltaTime *_speed);
            if (Vector2.Distance(_platform.position, _positions[_curr]) <= 0.1f)
            {
                _curr = (_curr + 1) % 2;
            }
        }

      
    }

    private void OnValidate()
    {
        _platform.position = 0.5f*(_start.position + _end.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(_start.position, _end.position);
    }
}
