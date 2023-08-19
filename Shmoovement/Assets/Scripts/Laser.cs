using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Laser : MonoBehaviour
{
    private InputAction fireAction;
    public InputActionAsset actions;

    [SerializeField] float range;
    [SerializeField] float laserScale;

    [SerializeField] GameObject debug;

    Camera c;
    // Start is called before the first frame update
    void Start()
    {
        c = transform.parent.GetComponentInChildren<Camera>();
        fireAction = actions.FindActionMap("gameplay").FindAction("Fire");

        fireAction.performed += OnFirePerformed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnFirePerformed(InputAction.CallbackContext ctx) 
    {
        Vector3 pointA = c.ViewportToWorldPoint(new Vector3(1, 0, c.nearClipPlane));
        Vector3 pointB = c.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, c.nearClipPlane + range));

        float length = (pointB - pointA).magnitude;

        Vector3 midpoint = new Vector3((pointA.x + pointB.x) / 2, (pointA.y + pointB.y) / 2, (pointA.z + pointB.z) / 2);



        transform.position = midpoint;

        Quaternion newRotation = Quaternion.FromToRotation(Vector3.up, pointB - pointA);
        transform.rotation =  newRotation;
        
        CapsuleCollider col = GetComponent<CapsuleCollider>();
        col.height = length;

        Instantiate(debug, pointA, Quaternion.identity);
        Instantiate(debug, midpoint, Quaternion.identity);
        Instantiate(debug, pointB, Quaternion.identity);

    }
}
