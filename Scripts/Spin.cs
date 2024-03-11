using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spin : MonoBehaviour
{
    public float speed;
    private bool rotate = true;

    private Vector3 lastPos;
    public float multiplier;
    public Transform cam;
    public Slider slider;

    float timer;
    // Update is called once per frame
    void Update()
    {
        if (rotate)
        {
            transform.RotateAround(transform.position, Vector3.up, -speed * Time.deltaTime);
        }
        else if (Input.GetMouseButton(1))
        {
            float move = Input.mousePosition.x - lastPos.x;
            transform.RotateAround(transform.position, Vector3.up, move* Time.deltaTime * multiplier);

        }
        lastPos = Input.mousePosition;

        cam.position += cam.forward * Input.mouseScrollDelta.y / 15 * Vector3.Distance(cam.position, Vector3.zero);
        transform.rotation = Quaternion.Euler(new Vector3(slider.value, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z));
    }

    public void ChangeBool()
    {
        if (rotate)
        {
            rotate = false;
        }
        else
            rotate = true;
    }
}
