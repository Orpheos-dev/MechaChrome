using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public enum CameraMode
    {
        Static,
        FollowPlayer,
        IsometricView
    }

    [Header("Camera Settings")]
    [SerializeField]
    private CameraMode _currentMode = CameraMode.Static;

    public CameraMode CurrentMode 
    { 
        get { return _currentMode; } 
        set 
        { 
            _currentMode = value; 
            Debug.Log("Camera mode changed to: " + _currentMode.ToString());
        } 
    }

    [Header("Static")]
    public Vector3 staticPosition;
    public Quaternion staticRotation;

    [Header("Player Follow")]
    [SerializeField]
    private Transform _actualPlayerTransform; // Directly reference the player's transform
    public Vector3 playerFollowOffset; 
    public Vector3 lookAtOffset;
    public float followSpeed = 5f;
	public static CameraManager Instance;
	
	public GameObject player;  // Reference to the player game object
	public Vector3 cameraOffset;  // Offset value for the camera position

    [Header("Isometric View")]
    public Vector3 isoOffset;

    private Vector3 velocity = Vector3.zero;
	
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}
	
	private void Start() 
	{
		GameObject player = PlayerManager.Instance.currentPlayer; // just an example
		if (player != null)
		{
			Debug.Log($"CameraManager currentPlayer: {player}");
		}
		else
		{
			Debug.Log("CameraManager couldn't find currentPlayer");
		}
	}
	
    private void Update()
    {
        switch (CurrentMode)
        {
            case CameraMode.Static:
                HandleStaticCamera();
                break;

            case CameraMode.FollowPlayer:
                HandleFollowPlayer();
                break;

            case CameraMode.IsometricView:
                HandleIsometricView();
                break;
        }
    }
	
	// Initialize the camera position based on the player
	public void InitializeCamera()
	{
		// Find the player GameObject by tag
		player = GameObject.FindGameObjectWithTag("Player");
		if (player != null)
		{
			transform.position = player.transform.position + cameraOffset;
		}
		else
		{
			Debug.LogWarning("Player not found");
		}
	}
	
	// Handle static camera mode
    private void HandleStaticCamera()
    {
        transform.position = staticPosition;
        transform.rotation = staticRotation;
    }
	
	// Handle follow player camera mode
    private void HandleFollowPlayer()
    {
        if (_actualPlayerTransform != null)
        {
            // Position
            Vector3 desiredPosition = _actualPlayerTransform.position + playerFollowOffset;
            Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, followSpeed * Time.deltaTime);
            transform.position = smoothedPosition;

            // Rotation
            Quaternion targetRotation = Quaternion.LookRotation((_actualPlayerTransform.position + lookAtOffset) - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, followSpeed * Time.deltaTime);
        }
    }

	// Handle isometric view camera mode
    private void HandleIsometricView()
    {
        if (_actualPlayerTransform != null)
        {
            Vector3 targetPosition = _actualPlayerTransform.position + isoOffset;
            targetPosition.y = transform.position.y;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
        }
    }

	// Set the actual player transform
	public void SetActualPlayerTransform(Transform newTransform)
	{
		_actualPlayerTransform = newTransform;
	}
}
