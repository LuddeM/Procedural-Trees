using UnityEngine;

public class CharacterController : MonoBehaviour {

    private Rigidbody _rigidBody;
    public float MovementSpeed = 0.5f;
    public float RotationSpeed = 1.0f;

	// Use this for initialization
	void Start () {
        _rigidBody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {

        var horizontalInput = Input.GetAxis("Horizontal");
        transform.Rotate(transform.up, horizontalInput);
    }

    private void FixedUpdate()
    {
        var verticalInput = Input.GetAxis("Vertical");
        _rigidBody.MovePosition(transform.position + transform.forward * verticalInput * MovementSpeed);
    }
}
