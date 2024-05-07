using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    //Initial spot of the camera
    private Vector3 _origin;

    //Calculated difference of the camera distance
    private Vector3 _difference;

    //Reference of the camera
    private Camera _mainCamera;

    // How far 
    private float _minZoom = 1.5f;
    private float _maxZoom;

    // XY boundary for camera position
    Vector3 _minXY;
    Vector3 _maxXY;

    private Coroutine _zoomCoroutine;

    [SerializeField] private float _cameraSpeed = 4f;

    //Check to see if the user is dragging the camera around
    private bool _isDragging;

    //Player Touch Inputs 
    private PlayerInputs _controls;

    //Awake method to initialize the camera
    private void Awake()
    {
        _mainCamera = Camera.main;
        _controls = new PlayerInputs();
    }

    //Function that enables touch controls
    private void OnEnable()
    {
        _controls.Enable();
    }

    //Function that disable touch controls
    private void OnDisable()
    {
        _controls.Disable();
    }

    //Start function where we detect the touch controls
    private void Start()
    {
        _controls.CameraMovement.SecondaryTouchContact.started += _ => ZoomStart();
        _controls.CameraMovement.SecondaryTouchContact.canceled += _ => ZoomEnd();
        _controls.CameraMovement.PrimaryTouchContact.canceled += _ => ZoomEnd();
        _controls.CameraMovement.Zoom.started += _ => ZoomStart();
        _controls.CameraMovement.Zoom.canceled += _ => ZoomEnd();

    }

    private void ZoomStart()
    {
        //Debug.Log("zoom start");
        _zoomCoroutine = StartCoroutine(ZoomDetection());
    } 
    
    private void ZoomEnd()
    {
        //Debug.Log("zoom end");
        StopCoroutine(_zoomCoroutine);
    }
    
    IEnumerator ZoomDetection()
    {
        float previousDistance = 0, distance = 0f;

        while (true)
        {
            distance = Vector2.Distance(_controls.CameraMovement.PrimaryFingerPosition.ReadValue<Vector2>(), _controls.CameraMovement.SecondaryFingerPosition.ReadValue<Vector2>());
            distance += _controls.CameraMovement.Zoom.ReadValue<Vector2>().y;

            if (distance > previousDistance && _mainCamera.orthographicSize > _minZoom)
            {
                _mainCamera.orthographicSize -= Time.deltaTime * _cameraSpeed;
            }

            else if(distance < previousDistance && _mainCamera.orthographicSize < _maxZoom)
            {
                _mainCamera.orthographicSize += Time.deltaTime * _cameraSpeed;
            }

            previousDistance = distance;
            yield return null;
        }
    }


    
    //Method that drags the camera around the scene
    public void OnDrag_Start(InputAction.CallbackContext ctx)
    {
        if (!IsOnDot())
        {
            if (ctx.started) _origin = GetMousePosition();
            _isDragging = ctx.started || ctx.performed;
        }
    }

    private bool IsOnDot()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if the ray hits an object with the "dot" tag
        if (Physics.Raycast(ray, out hit))
        {
            // if raycast hit dot and 
            if (hit.collider != null && hit.collider.CompareTag("dot"))
            {
                return true;
            }
        }
        return false;
    }

    //Late update method to check if the user is dragging the camera and updates its position
    private void LateUpdate()
    {
        if (!_isDragging) return;
        
        _difference = GetMousePosition() - transform.position;
        transform.position = _origin - _difference;

        // Restricts the camera movement by vector values

        // Left
        if (transform.position.x < _minXY.x)
        {
            transform.position = new Vector3(_minXY.x, transform.position.y, transform.position.z);
        }

        // Right
        if (transform.position.x > _maxXY.x)
        {
            transform.position = new Vector3(_maxXY.x, transform.position.y, transform.position.z);
        }

        // Bottom
        if (transform.position.y < _minXY.y)
        {
            transform.position = new Vector3(transform.position.x, _minXY.y, transform.position.z);
        }

        // Top
        if (transform.position.y > _maxXY.y)
        {
            transform.position = new Vector3(transform.position.x, _maxXY.y, transform.position.z);
        }

    }

    //Method that retrieves the current mouse position
    private Vector3 GetMousePosition()
    {
        Vector3 MousePos = Mouse.current.position.ReadValue();

        MousePos.z = 1;

        return _mainCamera.ScreenToWorldPoint(MousePos);

    }

    // Method that sets the beginning size, beginning position, min zoom, max zoom, and movement restrictions of the camera
    public void SetCamera(float startSize, Vector3 startPos, float minZoom, float maxZoom, Vector2 minXY, Vector2 maxXY)
    {
        _mainCamera.orthographicSize = startSize;
        _mainCamera.transform.position = startPos;
        _mainCamera.transform.position = new Vector3(_mainCamera.transform.position.x, _mainCamera.transform.position.y, -5f);
        _minZoom = minZoom;
        _maxZoom = maxZoom;

        // Minimum and maximum XY values for camera movement
        _minXY = minXY;
        _maxXY = maxXY;


    }
}
