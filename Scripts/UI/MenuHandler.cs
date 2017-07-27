using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
public class MenuHandler : MonoBehaviour {

    public GameObject AvatarMenu;
    private bool showMenu = false;
    // Use this for initialization


    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.M))
        {
            showMenu = !showMenu;
            if (showMenu)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
          
            }
            if(GetComponent<CameraController>().cameraMode == CameraMode.Avatar)
            {
                GetComponent<FirstPersonController>().enabled = !showMenu;
                GetComponent<CharacterController>().enabled = !showMenu;
            }
            AvatarMenu.SetActive(showMenu);
        }
    }

    public void HideMenu()
    {
        showMenu = false;
        AvatarMenu.SetActive(showMenu);
    }
}
