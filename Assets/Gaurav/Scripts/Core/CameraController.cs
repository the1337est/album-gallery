using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform _cameraTransform;
    private Vector3 _cameraPos;
    private Vector3 _cameraTargetPos;
    
    private const float _cameraPanSpeed = 10f;
    
    private bool _animating;

    /// <summary>
    /// Initializes Camera Controller
    /// </summary>
    public void Initialize()
    {
        _cameraTransform = Camera.main.transform;
        _cameraPos = _cameraTransform.position;
        _cameraTargetPos = _cameraPos;
    }

    /// <summary>
    /// Sets Target Position on the X axis that the Camera should tween to
    /// </summary>
    /// <param name="posX">X position</param>
    public void SetTargetX(float posX)
    {
        _cameraTargetPos = new Vector3(posX, _cameraPos.y, _cameraPos.z);
        _animating = true;
    }
    
    private void Update()
    {
        if(!_animating) return;
        if (Mathf.Abs(_cameraTargetPos.x - _cameraPos.x) > 0.01f)
        {
            _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, _cameraTargetPos, Time.deltaTime * _cameraPanSpeed);
        }
        else
        {
            _cameraTransform.position = _cameraTargetPos;
            _animating = false;
        }
    }
}
