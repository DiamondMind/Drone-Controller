using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiamondMind.Prototypes.DroneController
{
    public class dDroneController : MonoBehaviour
    {
        public Rigidbody _rb;

        public float verticalForce = 50f;
        public float maxVerticalForce = 200f;
        public float speed = 50f;
        public float sidewaysSpeed = 25f;
        public float turnSpeed = 3f;
        [Range(0f, 1f)] public float decelerationFactor = 0.95f;
        [Range(0f, 1f)] public float turnsmooth = 0.5f;
        public int tiltAngle = 5;
        [Range(0f, 1f)] public float tiltSmooth = 0.5f;

        float gravity = 9.81f;
        float force;
        float xRotation;
        float yRotation;
        float zRotation;
        float currentXRotation;
        [HideInInspector] public float currentYRotation;
        float currentZRotation;
        float xRotationSmooth;
        float yRotationSmooth;
        float zRotationSmooth;
        Vector3 dampVelocity;

        bool isMovingForward;
        bool isMovingBackward;
        bool isMovingLeft;
        bool isMovingRight;

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;   // lock mouse cursor
            _rb = GetComponent<Rigidbody>();
            force = _rb.mass * gravity; // calculate force acting on the object
        }

        void FixedUpdate()
        {
            VerticalMovement();
            HorizontalMovement();
            Turn();
            Tilt();

            _rb.AddRelativeForce(Vector3.up * force);   // overcome gravity & apply vertical force
            currentZRotation = Mathf.SmoothDamp(currentZRotation, zRotation, ref zRotationSmooth, tiltSmooth);
            currentZRotation = Mathf.Clamp(currentZRotation, -tiltAngle, tiltAngle);
            _rb.rotation = Quaternion.Euler(new Vector3(currentXRotation, currentYRotation, currentZRotation)); // update rotation to enable turning and tilting
        }

        void VerticalMovement()
        {
            // up and down movement
            if (Input.GetKey(KeyCode.Tab))
            {
                force += verticalForce;
                _rb.constraints = RigidbodyConstraints.None;
            }
            else if (Input.GetKey(KeyCode.Space))
            {
                force -= verticalForce;
                _rb.constraints = RigidbodyConstraints.None;
            }
            else if (!Input.GetKey(KeyCode.Tab) && !Input.GetKey(KeyCode.Space))
            {
                force = _rb.mass * gravity;     // F= m*a
                _rb.velocity = Vector3.SmoothDamp(_rb.velocity, new Vector3(_rb.velocity.x, 0, _rb.velocity.z), ref dampVelocity, decelerationFactor); // stabilize if no input is pressed
            }
            force = Mathf.Clamp(force, -maxVerticalForce, maxVerticalForce);    // clamp force
        }
        void HorizontalMovement()
        {
            // forward and backward movement
            if (Input.GetKey(KeyCode.W))
            {
                isMovingForward = true;
                _rb.AddRelativeForce(0, 0, speed);
            }
            else            
                isMovingForward = false;
            

            if (Input.GetKey(KeyCode.S))
            {
                isMovingBackward = true;
                _rb.AddRelativeForce(0, 0, -speed);
            }
            else
                isMovingBackward = false;
            if(!Input.GetKey(KeyCode.W) || !Input.GetKey(KeyCode.S))
            {
                _rb.velocity = Vector3.SmoothDamp(_rb.velocity, new Vector3(_rb.velocity.x, _rb.velocity.y, 0), ref dampVelocity, decelerationFactor); 
            }
            // sideways movement
            if (Input.GetKey(KeyCode.A))
            {
                isMovingLeft = true;
                _rb.AddRelativeForce(-sidewaysSpeed, 0, 0);
            }
            else
                isMovingLeft = false;

            if (Input.GetKey(KeyCode.D))
            {
                isMovingRight = true;
                _rb.AddRelativeForce(sidewaysSpeed, 0, 0);
            }
            else
                isMovingRight = false;
            if(!Input.GetKey(KeyCode.A) || !Input.GetKey(KeyCode.D))
            {
                _rb.velocity = Vector3.SmoothDamp(_rb.velocity, new Vector3(0, _rb.velocity.y, _rb.velocity.z), ref dampVelocity, decelerationFactor);
            }
        }
        void Turn()
        {
            // turn on the spot
            if(Input.GetKey(KeyCode.LeftArrow))
            {
                yRotation -= turnSpeed;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                yRotation += turnSpeed;
            }
            currentYRotation = Mathf.SmoothDamp(currentYRotation, yRotation, ref yRotationSmooth, turnsmooth);  //turn
        }
        void Tilt()
        {
            // forward & backward tilt
            if(isMovingForward)
            {
                xRotation += tiltAngle;
            }
            if (isMovingBackward)
            {
                xRotation -= tiltAngle;
            }
            if (!isMovingForward && !isMovingBackward)
            {
                xRotation = 0f;
            }
            else if (isMovingForward && isMovingBackward)
            {
                xRotation = 0f;
            }
            currentXRotation = Mathf.SmoothDamp(currentXRotation, xRotation, ref xRotationSmooth, tiltSmooth);
            currentXRotation = Mathf.Clamp(currentXRotation, -tiltAngle, tiltAngle);

            // sideways tilt
            if (isMovingLeft)
            {
                zRotation += tiltAngle;
            }
            if (isMovingRight)
            {
                zRotation -= tiltAngle;
            }
            if (!isMovingLeft && !isMovingRight)
            {
                zRotation = 0f;
            }
            else if (isMovingLeft && isMovingRight)
            {
                zRotation = 0f;
            }
            currentZRotation = Mathf.SmoothDamp(currentZRotation, zRotation, ref zRotationSmooth, tiltSmooth);
            currentZRotation = Mathf.Clamp(currentZRotation, -tiltAngle, tiltAngle);

            // freeze position on y axis while tilting
            if (_rb.velocity.y < 0.5f && _rb.velocity.y > -0.5)
            {
                if (isMovingForward || isMovingBackward || isMovingLeft || isMovingRight)
                {
                    _rb.constraints = RigidbodyConstraints.FreezePositionY;
                }
            }
        }
    }
}
