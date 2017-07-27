using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class CameraController : MonoBehaviour {

    public GameObject AvatarCamera;
    public GameObject WorldCamera;
    public CameraMode cameraMode;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void WorldCameraMode()
    {

        GetComponent<FirstPersonController>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
        WorldCamera.SetActive(true);
        AvatarCamera.SetActive(false);
        cameraMode = CameraMode.World;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }

    public void AvatarMode()
    {
        GetComponent<FirstPersonController>().enabled = true;
        GetComponent<CharacterController>().enabled = true;
        AvatarCamera.SetActive(true);
        WorldCamera.SetActive(false);
        cameraMode = CameraMode.Avatar;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}

public enum CameraMode
{
    Avatar,
    World
}
