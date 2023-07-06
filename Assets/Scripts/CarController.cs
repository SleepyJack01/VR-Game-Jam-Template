using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    private float turn;
    private float currentRotation;
    private float accelerate;
    private float currentLocalVelocity;
    


    [SerializeField] Rigidbody sphereRB;
    [SerializeField] Transform carModel;

    [Header("Car Settings")]
    [SerializeField] float turnSpeed = 10f;
    [SerializeField] float accelerationSpeed = 10f;
    [SerializeField] float maxSpeed = 60f;
    [SerializeField] float minSpeed = 0f;
    [SerializeField] float brakingForce = 10f;
    [SerializeField] float gravity = 25f;
    [SerializeField] LayerMask layerMask;

    [Header("Model Parts")]
    [SerializeField] Transform frontWheels;
    [SerializeField] Transform backWheels;

    [Header("Multiplayer Settings")]
    public int id = 0;
    public Transform spawnPoint;

    void Awake()
    {
    }

    void Start()
    {
        transform.parent.position = spawnPoint.position;
        transform.parent.rotation = spawnPoint.rotation;
    }

    void Update()
    {
        transform.position = sphereRB.transform.position - new Vector3(0, 0.4f, 0);
        currentLocalVelocity = transform.InverseTransformDirection(sphereRB.velocity).z;

        //rotate wheels
        frontWheels.localEulerAngles = new Vector3(0, (turn * 15), frontWheels.localEulerAngles.z);
        frontWheels.localEulerAngles += new Vector3((sphereRB.velocity.magnitude/2), 0, 0);
        backWheels.localEulerAngles += new Vector3((sphereRB.velocity.magnitude/2), 0, 0);
    }

    void FixedUpdate()
    {
        Turning();

        //gravity
        sphereRB.AddForce(Vector3.down * gravity, ForceMode.Acceleration);

        //accelerate
        if (accelerate > 0 && sphereRB.velocity.magnitude < maxSpeed)
        {
            sphereRB.AddForce(transform.forward * accelerate * accelerationSpeed, ForceMode.Acceleration);
        }
        else if (accelerate < 0 && sphereRB.velocity.magnitude > minSpeed)
        {
            sphereRB.AddForce(transform.forward * accelerate * brakingForce, ForceMode.Acceleration);
        }

        RaycastHit hitOn;
        RaycastHit hitNear;

        Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitOn, 1.1f, layerMask);
        Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitNear, 2f, layerMask);


        // Car Rotation
        carModel.up = Vector3.Lerp(carModel.up, hitNear.normal, Time.deltaTime * 8f);
        carModel.Rotate(0, transform.eulerAngles.y, 0);
    }

    void Turning()
    {
        if (currentLocalVelocity > 2)
        {
            transform.Rotate(0, turn * turnSpeed * Time.deltaTime, 0);
        }
        else if (currentLocalVelocity < 2 && currentLocalVelocity > 0)
        {
            transform.Rotate(0, turn * turnSpeed * (currentLocalVelocity/2) * Time.deltaTime, 0);
        }
        else if (currentLocalVelocity < 0 && currentLocalVelocity > -2)
        {
            transform.Rotate(0, -turn * turnSpeed * (-currentLocalVelocity/2) * Time.deltaTime, 0);
        }
        else if (currentLocalVelocity < -2)
        {
            transform.Rotate(0, -turn * turnSpeed * Time.deltaTime, 0);
        }
    }

    public void OnTurn(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            turn = context.ReadValue<float>();
        }
    }

    public void OnAccelerate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            accelerate = context.ReadValue<float>();
        }
    }

    public void RespawnPlayer()
    {
        sphereRB.transform.position = spawnPoint.position;
        sphereRB.transform.rotation = spawnPoint.rotation;
        sphereRB.velocity = Vector3.zero;
        sphereRB.angularVelocity = Vector3.zero;
        transform.rotation = spawnPoint.rotation;
    }

    private void OnEnable() 
    {
    }

    private void OnDisable() 
    {

    }
}
