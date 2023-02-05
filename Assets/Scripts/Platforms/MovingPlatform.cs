using System.Collections;
using System.Collections.Generic;
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
    [SerializeField]
    float _threshold = 0.01f;

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
            return;
        }
        else
        {
            _platform.position = Vector2.Lerp(_platform.position, _positions[_curr],Time.deltaTime *_speed);
            if (Vector2.Distance(_platform.position, _positions[_curr]) <= _threshold)
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
