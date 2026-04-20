using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    // Settings
    [SerializeField] private float MoveSpeed = 50f;
    [SerializeField] private float MaxSpeed = 15f;
    [SerializeField] private float Drag = 0.98f;
    [SerializeField] private float DriftSteerAngle = 20f;
    [SerializeField] private float Traction = 1f;
    [SerializeField] private float Downforce = 5f; // nuevo: intensidad del downforce
    [SerializeField] private bool UseSquaredDownforce = false; // opcional: usar v^2
    private float SteerAngle = 20f;

    // Variables
    private Vector3 MoveForce;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        rb.centerOfMass = new Vector3(0f, -0.5f, 0f);
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    void FixedUpdate()
    {
        float forwardInput = Input.GetAxis("Vertical");
        float steerInput = Input.GetAxis("Horizontal");
        if(Input.GetKey(KeyCode.LeftShift))
        {
            SteerAngle = DriftSteerAngle;
        }
        else
        {
            SteerAngle = DriftSteerAngle/2;
        }

        // 1) Acumulo la fuerza (como en tu original)
        MoveForce += transform.forward * MoveSpeed * forwardInput * Time.fixedDeltaTime;

        // 2) Drag y límite sobre MoveForce
        MoveForce *= Drag;
        MoveForce = Vector3.ClampMagnitude(MoveForce, MaxSpeed);

        // 3) Tracción: alinear MoveForce hacia forward
        MoveForce = Vector3.Lerp(MoveForce.normalized, transform.forward, Traction * Time.fixedDeltaTime) * MoveForce.magnitude;

        // 4) Aplicar la velocidad calculada al Rigidbody (comportamiento "telegráfico" del original)
        Vector3 desiredVelocity = MoveForce;
        Vector3 velChange = desiredVelocity - rb.velocity;
        rb.AddForce(velChange, ForceMode.VelocityChange);

        // 5) Steering: rotación basada en la magnitud de MoveForce
        float yaw = steerInput * MoveForce.magnitude * SteerAngle * Time.fixedDeltaTime;
        Quaternion deltaRot = Quaternion.Euler(0f, yaw, 0f);
        rb.MoveRotation(rb.rotation * deltaRot);

        // 6) Downforce para mantener el coche pegado al suelo
        //    - UseSquaredDownforce = false -> proporcional a v
        //    - UseSquaredDownforce = true  -> proporcional a v^2 (más realista a altas velocidades)
        float speed = rb.velocity.magnitude;
        float dfFactor = UseSquaredDownforce ? speed * speed : speed;
        // ForceMode.Acceleration hace que el efecto no dependa de la masa
        rb.AddForce(-transform.up * dfFactor * Downforce, ForceMode.Acceleration);

        // Debug
        Debug.DrawRay(transform.position, MoveForce.normalized * 3f, Color.red);
        Debug.DrawRay(transform.position, transform.forward * 3f, Color.blue);
    }
}