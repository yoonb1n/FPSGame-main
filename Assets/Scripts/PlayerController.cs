using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, Health.IHealthListener
{

    public GameObject[] weapons;

    public float walkingSpeed = 7;
    public float mouseSens = 10;
    public float jumpSpeed = 10;

    public Transform cameraTransform;

    float verticalAngle;
    float horizontalAngle;
    float verticalSpeed;

    bool isGrounded;
    float groundedTimer;

    int currentWeapon;

    CharacterController characterController;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isGrounded = true;
        groundedTimer = 0;

        verticalSpeed = 0;
        verticalAngle = 0;
        horizontalAngle = transform.localEulerAngles.y;

        characterController = GetComponent<CharacterController>();

        currentWeapon = 0;
        UpdateWeapon();
    }

    // Update is called once per frame
    void Update()
    {

        if ( !characterController.isGrounded) 
        {
            if (isGrounded)
            {
                groundedTimer += Time.deltaTime;
                if (groundedTimer >0.5f)
                {
                    isGrounded =false;
                }
            }
        }
        else
        {
            isGrounded = true;
            groundedTimer = 0;
        }

        // ����

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            verticalSpeed = jumpSpeed;
            isGrounded = false;
        }

        // �����̵�
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (move.magnitude > 1)
        {
            move.Normalize();
        }

        move = move * walkingSpeed * Time.deltaTime;
        move = transform.TransformDirection(move);
        characterController.Move(move);

        // ��/�� ���콺

        float turnPlayer = Input.GetAxis("Mouse X") * mouseSens;
        horizontalAngle += turnPlayer;

        if (horizontalAngle > 360) horizontalAngle -= 360;
        if (horizontalAngle < 0) horizontalAngle += 360;

        Vector3 currentAngle = transform.localEulerAngles;
        currentAngle.y = horizontalAngle;
        transform.localEulerAngles = currentAngle;

        // ��/�� ���콺

        float turnCam = Input.GetAxis("Mouse Y") * mouseSens;
        verticalAngle -= turnCam;
        verticalAngle = Mathf.Clamp(verticalAngle, -89f, 89f);
        currentAngle = cameraTransform.localEulerAngles;
        currentAngle.x = verticalAngle;
        cameraTransform.localEulerAngles = currentAngle;

        // ����

        verticalSpeed -= 10 * Time.deltaTime;
        if (verticalSpeed < -10)
        {
            verticalSpeed = -10;
        }

        Vector3 verticalMove = new Vector3(0, verticalSpeed, 0);
        verticalMove = verticalMove * Time.deltaTime;
        CollisionFlags flag= characterController.Move(verticalMove);

        if ( (flag & CollisionFlags.Below) != 0)
        {
            verticalSpeed = 0;
        }

        // ���� ����

        if (Input.GetButtonDown("ChangeWeapon"))
        {
            currentWeapon++;
            if (currentWeapon>=weapons.Length)
            {
                currentWeapon = 0;
            }
            UpdateWeapon();
        }

    }

    void UpdateWeapon()
    {
        foreach (GameObject w in weapons)
        {
            w.SetActive(false);
        }

        weapons[currentWeapon].SetActive(true);
    }

    public void Die()
    {
        GetComponent<Animator>().SetTrigger("Die");
        Invoke("TellIamDie", 1.0f);
    }

    void TellIamDie()
    {
        GameManager.instance.GameOverScene();
    }
}
