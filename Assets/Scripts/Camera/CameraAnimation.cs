using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimation : MonoBehaviour
{

    
    private Transform _oldCamTransform;
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private Camera cam;

    private Vector3 _oldCamPosition;
    private Quaternion _oldCamRotation;

    private void Start()
    {
        _oldCamTransform = Camera.main.transform;
        _oldCamPosition = _oldCamTransform.position;
        _oldCamRotation = _oldCamTransform.rotation;
        anim.SetBool("oldCamDied", false);
        cam.enabled = false;
    }
    void Update()
    {
        if (_oldCamTransform.gameObject.activeInHierarchy)
        {
            this.transform.SetPositionAndRotation(_oldCamTransform.position, _oldCamTransform.rotation);
        }
        else
        {
            cam.enabled = true;
            anim.Play("cam_anim");
        }
    }
}
