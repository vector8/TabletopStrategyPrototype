using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    public float cameraScrollSpeed;
    public float cameraRotateSpeed;
    public float cameraMaxZoom;
    public float cameraMinZoom;
    public float cameraZoomStep;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Pan
        Vector3 input = new Vector3();
        input.x = Input.GetAxisRaw("Horizontal");
        input.z = Input.GetAxisRaw("Vertical");
        input.Normalize();

        // Cancel out x rotation so we only pan on x-z plane
        Vector3 rotationEulers = transform.rotation.eulerAngles;
        rotationEulers.x = 0;
        input = Quaternion.Euler(rotationEulers) * input;

        // Need to scale this by camera's ortho size so that it feels like it moves at the 
        // same speed at any level of zoom.
        input *= cameraScrollSpeed * (Camera.main.orthographicSize / 2.5f) * Time.deltaTime;

        Vector3 currentPos = transform.position;
        currentPos.x += input.x;
        currentPos.z += input.z;
        transform.position = currentPos;

        // Rotate
        float rotationInput;
        if(Input.GetMouseButton(1)) // rotate using mouse x if right mouse button is pressed
        {
            rotationInput = Input.GetAxis("Mouse X");
        }
        else // rotate using q-e buttons otherwise
        {
            rotationInput = Input.GetAxisRaw("Rotate");
        }

        // Rotate around the focus of the camera, on the x-z plane
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Camera.main.pixelWidth / 2.0f, Camera.main.pixelHeight / 2.0f, 0f));
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Vector3 rotatePivot = new Vector3();
        float distance = 0;
        if(plane.Raycast(ray, out distance))
        {
            rotatePivot = ray.GetPoint(distance);

            transform.RotateAround(rotatePivot, Vector3.up, rotationInput * cameraRotateSpeed * Time.deltaTime);
        }

        // Zoom
        float zoomInput = Input.GetAxis("Mouse ScrollWheel");
        float orthoSize = Camera.main.orthographicSize;
        orthoSize += zoomInput * 10 * cameraZoomStep; // Mouse wheel steps are 0.1, so multiply by 10 to bring each step to 1
        Camera.main.orthographicSize = Mathf.Clamp(orthoSize, cameraMinZoom, cameraMaxZoom);
    }
}
