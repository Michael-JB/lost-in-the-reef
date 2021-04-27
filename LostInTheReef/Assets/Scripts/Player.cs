using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {
    public float moveSpeed = 3;
    public float rotationSpeed = 130;

    private Rigidbody2D rb;
    public CinemachineVirtualCamera virtualCamera;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {
        // Controls
        if (Input.GetKey(KeyCode.Escape)) SceneManager.LoadScene("TitleScene");

        // Movement
        Vector2 direction = transform.up;
        Vector2 velocity = new Vector2(0, 0);
        if (Input.GetKey(KeyCode.W)) velocity += direction * moveSpeed;
        if (Input.GetKey(KeyCode.S)) velocity -= direction * moveSpeed;
        rb.velocity = velocity;

        // Rotation
        if (Input.GetKey(KeyCode.Q))
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.E))
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime * -1);

        // Camera follows rotation
        virtualCamera.transform.rotation = rb.transform.rotation;
    }
}
