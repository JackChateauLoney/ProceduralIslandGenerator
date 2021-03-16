using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] Transform kartModel;
    [SerializeField] Transform kartNormal;
    [SerializeField] Rigidbody sphere = null;
    [SerializeField] float acceleration = 5.0f;
    [SerializeField] float gravity = 9.8f;
    [SerializeField] float steering = 10.0f;

    LayerMask groundMask = ~0;

    float speed = 0.0f, rotate = 0.0f;
    float currentSpeed = 0.0f, currentRotate = 0.0f;


    // Update is called once per frame
    void Update()
    {
        //follow collider
        transform.position = sphere.transform.position - new Vector3(0.0f, 0.4f, 0.0f);


        if(Input.GetKey(KeyCode.W)) speed = acceleration;
        else if (Input.GetKey(KeyCode.S)) speed = -acceleration;


        if (Input.GetAxis("Horizontal") != 0) 
        {
            int dir = Input.GetAxis("Horizontal") > 0 ? 1 : -1;
            float amount = Mathf.Abs(Input.GetAxis("Horizontal"));
            Steer(dir, amount);
        }

        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 12.0f); speed = 0.0f;
        currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * 4.0f); rotate = 0.0f;
    }



    private void FixedUpdate()
    {

        //forward acceleration
        sphere.AddForce(kartModel.transform.forward * currentSpeed, ForceMode.Acceleration);

        //gravity
        sphere.AddForce(Vector3.down * gravity, ForceMode.Acceleration);

        //steering
        Debug.Log("currentRotate" + currentRotate);
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * 5.0f);

        RaycastHit hitNear;
        Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitNear, 2.0f, groundMask);

        //normal rotation
        kartNormal.up = Vector3.Lerp(kartNormal.up, hitNear.normal, Time.deltaTime * 8.0f);
        kartNormal.Rotate(0, transform.eulerAngles.y, 0);



    }




    void Steer(int direction, float amount) 
    {
        rotate = (steering * direction) * amount;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position - (transform.up * 2));
    }

}
