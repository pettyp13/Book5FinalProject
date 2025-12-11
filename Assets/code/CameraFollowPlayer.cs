using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    GameObject target;
    public bool indoorMode = false;
    GameObject closerangeCamera;
    // Start is called before the first frame update
    void Start()
    {
        target = transform.parent.gameObject;
        closerangeCamera = GameObject.Find("cameraIndoor");
        closerangeCamera.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target.transform.position);
        if (Input.GetKeyDown(KeyCode.C)) indoorMode = !indoorMode;
        if (indoorMode)
        {
            closerangeCamera.SetActive(true);

        }
        else
        {

            if (closerangeCamera.active) closerangeCamera.SetActive(false);

        }
    }
}
