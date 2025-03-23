using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerJoystick : MonoBehaviour
{
    protected Joystick joystick;

    public Button _jumpButton;
    public float speed = 3f;
    public float rotateSpeed = 0.5f;
    [SerializeField] protected Rigidbody _rb;
    public float VelocityMagnitude;
    public Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        joystick = FindObjectOfType<Joystick>();
        cam = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var forward = cam.transform.forward;
        var right = cam.transform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        //var joystickMovement = (Vector3)joystick.Direction - transform.localRotation.eulerAngles;
        //Debug.Log(joystick.Direction);

        var moveDirection = (joystick.Vertical  * forward +  joystick.Horizontal * right) +
                            (Input.GetAxis("Vertical") * forward + Input.GetAxis("Horizontal") * right);
        Vector3 rotateTowards = new Vector3(0, Mathf.Atan2(joystick.Horizontal, joystick.Vertical) * 180 / Mathf.PI, 0);
        if (joystick.Direction.x != 0 || joystick.Direction.y != 0)
        {

            _rb.AddForce(new Vector3(moveDirection.x * speed,
                                         _rb.velocity.y,
                                         moveDirection.z * speed));

            if (joystick.Direction.y > 0)
            {
                // ileri gitme
                
                //Debug.Log("Rotate: " + rotateTowards);
                rotateTowards *= 0.01f;
                //transform.eulerAngles += AngleLerp(transform.localRotation.eulerAngles, rotateTowards, Time.deltaTime * rotateSpeed);
                transform.Rotate(rotateTowards);
            }
        }

        _rb.AddForce(new Vector3(moveDirection.x * speed,
                                        _rb.velocity.y,
                                        moveDirection.z * speed));
        //Debug.Log(rotateTowards);
        rotateTowards *= 0.01f;
        //transform.eulerAngles += AngleLerp(transform.localRotation.eulerAngles, rotateTowards, Time.deltaTime * rotateSpeed);
        transform.Rotate(rotateTowards);
        ////joystick aşagı tarafdaysa geri gitme
        //if (joystick.Direction.y < 0 && joystick.Direction.x < 0 || joystick.Direction.y < 0 && joystick.Direction.x >= 0)
        //{
        //    rigidbody.AddForce(new Vector3(moveDirection.x * speed,
        //                                 rigidbody.velocity.y,
        //                                moveDirection.z * speed));
        //    //Vector3 rotateTowards = new Vector3(0, Mathf.Atan2(joystick.Horizontal, joystick.Vertical) * 180 / Mathf.PI, 0);
        //    //Debug.Log(rotateTowards);
        //    //rotateTowards *= 0.003f;
        //    ////transform.eulerAngles += AngleLerp(transform.localRotation.eulerAngles, rotateTowards, Time.deltaTime * rotateSpeed);
        //    //transform.Rotate(-rotateTowards);
        //} else // ileri gitme
        //{

        //}

        // rotation
        //clamp the velocity
        VelocityMagnitude = _rb.velocity.magnitude;
       
        if (transform.localScale.x >= 3)
        {
            _rb.velocity = Vector3.ClampMagnitude(_rb.velocity,10f / (transform.localScale.x * 0.33f));
            //Debug.Log(_agent.velocity.magnitude);
        }
        else
        {
            _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, 10f);
            //Debug.Log(_agent.velocity.magnitude);
        }
    }


    Vector3 AngleLerp(Vector3 StartAngle , Vector3 FinishAngle, float t)
    {
        float xLerp = Mathf.LerpAngle(StartAngle.x, FinishAngle.x, t);
        float yLerp = Mathf.LerpAngle(StartAngle.y, FinishAngle.y, t);
        float zLerp = Mathf.LerpAngle(StartAngle.z, FinishAngle.z, t);
        Vector3 Lerped = new Vector3(xLerp, yLerp, zLerp);
        return Lerped;
    }
    public void JumpButtonClicked()
    {
        StartCoroutine(JumpButtonRoutine());
    }
    private IEnumerator JumpButtonRoutine()
    {
        _rb.velocity += Vector3.up * speed * 0.3f;
        _jumpButton.interactable = false;
        //10 sec cooldown for jump
        yield return new WaitForSeconds(10f);
        _jumpButton.interactable = true;
    }
}
