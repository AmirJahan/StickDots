using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
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

    //XY offset for camera panning
    [SerializeField] float offsetX = 1f;
    [SerializeField] float offsetY = 1f;

    private Coroutine _zoomCoroutine;

    [SerializeField] private float _cameraSpeed = 4f;

    //Check to see if the user is dragging the camera around
    private bool _isDragging;
    private bool touchInput;

    //Player Touch Inputs 
    private PlayerInputs _controls;

    //record original position for reset
    private Vector3 cameraStartingPosition;

    //reset camera when the game end
    [SerializeField] public UnityEvent gameEndEvent;

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
        gameEndEvent.AddListener(ResetCamera);
    }

    //Function that disable touch controls
    private void OnDisable()
    {
        _controls.Disable();
        gameEndEvent.RemoveListener(ResetCamera);
    }

    //Start function where we detect the touch controls
    private void Start()
    {
        _controls.CameraMovement.SecondaryTouchContact.started += _ => ZoomStart();
        _controls.CameraMovement.SecondaryTouchContact.canceled += _ => ZoomEnd();
        _controls.CameraMovement.PrimaryTouchContact.canceled += _ => ZoomEnd();
        _controls.CameraMovement.Zoom.started += _ => ZoomStart();
        _controls.CameraMovement.Zoom.canceled += _ => ZoomEnd();
        _controls.CameraMovement.PrimaryTouchContact.started += _ => OnTouch();
        _controls.CameraMovement.PrimaryTouchContact.canceled += _ => OnTouchEnd();
        _controls.CameraMovement.DoubleTap.performed += _ => ResetCamera();
    }
    private void OnTouch()
    {
        touchInput = true;
    }
    private void OnTouchEnd() {
        touchInput = false;
    }

    private void ZoomStart()
    {
        //Debug.Log("zoom start");
        if(!IsOnUI()) _zoomCoroutine = StartCoroutine(ZoomDetection());
    } 
    
    private void ZoomEnd()
    {
        //Debug.Log("zoom end");
        if(_zoomCoroutine != null)StopCoroutine(_zoomCoroutine);
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
    public void OnDrag(InputAction.CallbackContext ctx)
    {
        
        // if click on dot, don't drag
        if (!IsOnDot() && !IsOnUI())
        {
            if (ctx.started) _origin = GetInputPosition();
            _isDragging = ctx.started || ctx.performed;
        }
        else if(ctx.canceled)
        {
            _isDragging = false;
        }
    }

    //method that check if the player is clicking on dot.
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
        Vector2 flatpos = new Vector2(transform.position.x, transform.position.y);

        _difference = GetInputPosition() - flatpos;
        _difference.z = 1;
        transform.position = _origin - _difference;

        // Restricts the camera movement by vector values

        // Left
        if (transform.position.x < _minXY.x + offsetX)
        {
            transform.position = new Vector3(_minXY.x + offsetX, transform.position.y, transform.position.z);
        }

        // Right
        if (transform.position.x > _maxXY.x - offsetX)
        {
            transform.position = new Vector3(_maxXY.x - offsetX, transform.position.y, transform.position.z);
        }

        // Bottom
        if (transform.position.y < _minXY.y + offsetY)
        {
            transform.position = new Vector3(transform.position.x, _minXY.y + offsetY, transform.position.z);
        }

        // Top
        if (transform.position.y > _maxXY.y - offsetY)
        {
            transform.position = new Vector3(transform.position.x, _maxXY.y - offsetY, transform.position.z);
        }

    }
    //method that decide to use finger position or mouse position as input
    private Vector2 GetInputPosition()
    {
        //if there is touch input
        if (touchInput)
        {
            //Debug.Log("finger position");
            return GetFingerPosition();
        }
        else
        {
            //Debug.Log("mousePosition");
            return GetMousePosition();
        }        
    }
    //Method that retrieves the current Finger touch position for touch device
    private Vector2 GetFingerPosition()
    {
        
        Vector3 FingerPos = _controls.CameraMovement.PrimaryFingerPosition.ReadValue<Vector2>();

        FingerPos.z = 1;

        return _mainCamera.ScreenToWorldPoint(FingerPos);
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
        cameraStartingPosition = startPos;
        cameraStartingPosition.z = -5f;
        _mainCamera.transform.position = cameraStartingPosition;
        _minZoom = minZoom;
        _maxZoom = maxZoom;

        // Minimum and maximum XY values for camera movement
        _minXY = minXY;
        _maxXY = maxXY;
    }

    [SerializeField] private AnimationCurve animationCurve;
    [SerializeField] private float resetAnimationDuration = 0.5f;
    private float resetCameraTimer = 0f;

    //function triggered when game end
    public void ResetCamera()
    {
        if (IsOnUI()) return;
        resetCameraTimer = resetAnimationDuration;
        _controls.CameraMovement.Disable();
        GetComponent<PlayerInput>().currentActionMap.Disable();
        StartCoroutine(ResetCameraLoop(_mainCamera.orthographicSize,_mainCamera.transform.position));
    }
    //for animation on reseting camera
    IEnumerator ResetCameraLoop(float currentZoom, Vector3 currentPos)
    {
        float timeElapse = Time.deltaTime;
        while (resetCameraTimer > 0f)
        {
            float scale = animationCurve.Evaluate(1 - resetCameraTimer / resetAnimationDuration);
            _mainCamera.orthographicSize = currentZoom + ((_maxZoom - currentZoom) * scale );
            _mainCamera.transform.position = currentPos + ((cameraStartingPosition - currentPos) * scale);
            yield return new WaitForSeconds(timeElapse);
            resetCameraTimer -= timeElapse;
        }
        //_mainCamera.orthographicSize = _maxZoom;
        //_mainCamera.transform.position = cameraStartingPosition;
        _controls.CameraMovement.Enable();
        GetComponent<PlayerInput>().currentActionMap.Enable();
    }

    private bool IsOnUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    ////////////////////////////////////

}
