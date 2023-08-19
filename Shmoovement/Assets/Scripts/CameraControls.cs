using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControls : MonoBehaviour
{

    [SerializeField] float lookSensitivity = 0.1f;
    [SerializeField] float minTurnAngle = -90.0f;
    [SerializeField] float maxTurnAngle = 90.0f;
    [SerializeField] float cameraYOffset = 2.0f;
    [Range(0.0f, 1.0f)] [SerializeField] float lookSpeed = 0.5f;

    public InputActionAsset actions;
    private InputAction cameraX;
    private InputAction cameraY;
    private float xDelta;
    private float yDelta;
    private GameObject player;

    private float rotX;
    private Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        cameraX = actions.FindActionMap("gameplay").FindAction("Camera X");
        cameraY = actions.FindActionMap("gameplay").FindAction("Camera Y");

        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
       

        //float mouseX = Mouse.
        //transform.Rotate(transform.localEulerAngles)
    }

    private void LateUpdate()
    {
        Vector2 mouseDelta = new Vector2(xDelta, yDelta);




        float y = Mouse.current.delta.x.ReadValue() * lookSensitivity * Time.deltaTime;


        rotX += Mouse.current.delta.y.ReadValue() * lookSensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, minTurnAngle, maxTurnAngle);


        //if (newCameraRotation.x > maxLookDown)
        //  Debug.Log(newCameraRotation.x + " > " + maxLookDown);



        //Debug.Log(newCameraRotation);

        Vector3 newCameraRotation = new Vector3(-rotX, transform.eulerAngles.y + y, 0);

        transform.eulerAngles = newCameraRotation;

        Vector3 target = new Vector3(player.transform.position.x, player.transform.position.y + cameraYOffset, player.transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, 0.005f);
    }


}
