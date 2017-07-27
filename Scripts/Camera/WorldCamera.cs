using UnityEngine;
using System.Collections;
using System;

public class WorldCamera : MonoBehaviour {

    private float cameraMoveSpeed = 20f;
    private float smoothFactor = 2f;

    private float zoomDistance = 60f;
    private float sensitivityDistance = 50f;
    private float damping  = 2.5f;

    public float minFOV = 10f;
    public float maxFOV = 80;

    public float minCameraHover = 25;
    public float maxCameraHover = 75;
    public float cameraHover = 25f;
    private float currentCameraY;
    private float cameraStartHeight;

    public KeyCode forwardButton = KeyCode.W;
    public KeyCode backwardButton = KeyCode.S;
    public KeyCode rightButton = KeyCode.D;
    public KeyCode leftButton = KeyCode.A;

    public float minY = -80f;
    public float maxY = 80.0f;

    public float sensX = 100.0f;
    public float sensY = 100.0f;
    public float rotateModifier = 2.3f;

    private float rotationY = -45.0f;
    private float rotationX = 0.0f;
    private Vector3 deltaPosition;
    private Vector3 rotationVector;
    private bool isMoving = false;
    private bool isStartMoving = false;
    // Use this for initialization
    void Start () {

        zoomDistance = Camera.main.fieldOfView;
    }
	
    void OnEnable()
    {
        InitializeCamera();
    }

    private void InitializeCamera()
    {
        SetCameraStartHeight();
        currentCameraY = cameraStartHeight;
        deltaPosition = transform.parent.position;
        deltaPosition.y = currentCameraY;
        rotationVector = new Vector3(-rotationY, rotationX, 0f);
        isStartMoving = true;
    }

	// Update is called once per frame
	void LateUpdate () {

        if(isStartMoving)
        {
            transform.parent.position = Vector3.Lerp(transform.parent.position, deltaPosition, Time.deltaTime * smoothFactor);
            if(Mathf.Abs(transform.parent.position.y - deltaPosition.y) < 0.1f)
            {
                isStartMoving = false;
            }
        }
        else
        {
            if (CheckIfMovementKeysPressed())
            {
                Move();
            }

            if (Input.GetAxis("Mouse ScrollWheel") != 0f && !Input.GetKey(KeyCode.LeftControl))
            {
                zoomDistance -= Input.GetAxis("Mouse ScrollWheel") * sensitivityDistance;
                zoomDistance = Mathf.Clamp(zoomDistance, minFOV, maxFOV);

            }

            if (Input.GetMouseButton(1))
            {
                rotationX += Input.GetAxis("Mouse X") * sensX * Time.deltaTime;
                rotationY += Input.GetAxis("Mouse Y") * sensY * Time.deltaTime;
                rotationY = Mathf.Clamp(rotationY, minY, maxY);
                rotationVector = new Vector3(-rotationY, rotationX, 0f);

            }

            if (isMoving)
            {
                AdjustCameraHeight(deltaPosition);
                transform.parent.position = Vector3.Lerp(transform.parent.position, deltaPosition, Time.deltaTime * smoothFactor);
                if (Mathf.Abs(transform.parent.position.x - deltaPosition.x) < 0.1f && Mathf.Abs(transform.parent.position.y - deltaPosition.y) < 0.1f && Mathf.Abs(transform.parent.position.z - deltaPosition.z) < 0.1f)
                {
                    isMoving = false;
                }
            }
        }
        transform.parent.rotation = Quaternion.Lerp(transform.parent.rotation, Quaternion.Euler(rotationVector), Time.deltaTime * smoothFactor * rotateModifier);
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, zoomDistance, Time.deltaTime * damping);
    }

    void AdjustCameraHeight(Vector3 position)
    {
        RaycastHit hit;
        position.y = 500f;
        if(Physics.Raycast(position, Vector3.down, out hit, Mathf.Infinity))
        {
            float difference = (cameraHover + hit.point.y) - cameraStartHeight;
            currentCameraY = cameraStartHeight + difference;
        }
        deltaPosition.y = currentCameraY;
    }

    void SetCameraStartHeight()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.parent.position, Vector3.down, out hit, Mathf.Infinity))
        {
            cameraStartHeight = cameraHover + hit.point.y;
            currentCameraY = cameraStartHeight;
        }
        deltaPosition.y = currentCameraY;
    }

    private void Move()
    {
        deltaPosition = transform.parent.position;
        Vector3 moveVector = transform.forward;
        moveVector.y = 0f;
        if(Input.GetKey(KeyCode.LeftControl))
        {
            float wheelModifier = Input.GetAxis("Mouse ScrollWheel") * sensitivityDistance;
            if(wheelModifier != 0f)
            {
                if(cameraHover+wheelModifier >= minCameraHover && cameraHover+wheelModifier <= maxCameraHover)
                {
                    deltaPosition.y += wheelModifier;
                    cameraHover += wheelModifier;
                }
            }                   
        }
        if (Input.GetKey(forwardButton))
        {
            deltaPosition += moveVector * cameraMoveSpeed;
        }
        if (Input.GetKey(backwardButton))
        {
            deltaPosition += -moveVector * cameraMoveSpeed;
        }
        if (Input.GetKey(rightButton))
        {
            deltaPosition += transform.parent.right * cameraMoveSpeed;
        }
        if (Input.GetKey(leftButton))
        {
            deltaPosition += -transform.parent.right * cameraMoveSpeed;
        }


        isMoving = true;
    }

    private bool CheckIfMovementKeysPressed()
    {
        return Input.GetKey(forwardButton) || Input.GetKey(backwardButton) || Input.GetKey(leftButton) || Input.GetKey(rightButton) || Input.GetKey(KeyCode.LeftControl);
    }


}
