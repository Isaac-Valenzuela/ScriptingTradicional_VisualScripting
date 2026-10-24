using UnityEngine;

/// <summary>
/// Controla el movimiento de un personaje usando CharacterController
/// y sincroniza las animaciones (idle / walking) con el Animator.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Gravedad")]
    [SerializeField] private float gravity = -9.81f;

    [Header("Animación")]
    [SerializeField] private float animationDamping = 0.1f; // Suaviza el cambio de parámetro

    private CharacterController controller;
    private Animator animator;
    private float verticalVelocity;

    // Usar un hash es más eficiente que escribir el string cada frame
    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        // Busca el Animator en este objeto o en sus hijos
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        // 1. Entrada
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 2. Dirección en XZ (sin ir más rápido en diagonal)
        Vector3 direction = new Vector3(horizontal, 0f, vertical);
        direction = Vector3.ClampMagnitude(direction, 1f);

        // 3. Rotación hacia la dirección de movimiento
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // 4. Gravedad
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }
        verticalVelocity += gravity * Time.deltaTime;

        // 5. Aplicar movimiento
        Vector3 velocity = direction * speed;
        velocity.y = verticalVelocity;
        controller.Move(velocity * Time.deltaTime);

        // 6. Animación: Speed va de 0 (quieto) a 1 (caminando a máxima velocidad)
        if (animator != null)
        {
            animator.SetFloat(SpeedHash, direction.magnitude, animationDamping, Time.deltaTime);
        }
    }
}