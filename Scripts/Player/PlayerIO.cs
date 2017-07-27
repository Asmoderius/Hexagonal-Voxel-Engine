using UnityEngine;
using System.Collections;
/*
Class: PlayerIO.
Date: 11-03-2016.
Author: Louis Fleron.
Description:
NOTE: 

THIS CLASS IS A DUMMY, USED ONLY FOR TESTING. REPLACE WITH PROPER SYSTEM.

SEE NYX VERSION 0.18
*/
public class PlayerIO : MonoBehaviour {

    public float maxRange = 10f;
    public GameObject flashLight;
    private Block primitiveInventory;
    private bool menuActive = false;
	// Use this for initialization


    void Awake()
    {
        Settings.currentSettings.Player = transform.gameObject;
    }
	void Start () {
       
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

	// Update is called once per frame
	void Update () 
    {

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxRange))
            {
                Chunk c = hit.transform.GetComponent<ChunkMesh>().GetParent();
                if (c != null)
                {
                    Settings.currentSettings.Operations.DamageBlock(hit, c);
                }
            }

        }
        else if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxRange))
            {
                Chunk c = hit.transform.GetComponent<ChunkMesh>().GetParent();
                if (c != null)
                {

                    Settings.currentSettings.Operations.PlaceBlock(primitiveInventory, hit, c);

                }
            }
        }


        if (Input.GetKeyDown(KeyCode.E))
        {
            flashLight.SetActive(!flashLight.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            primitiveInventory = Blocks.Get(3);
            primitiveInventory.Connected = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            primitiveInventory = Blocks.Get(5);
            primitiveInventory.Connected = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            primitiveInventory = Blocks.Get(18);
            primitiveInventory.Connected = true;
        }
         
    }


}
