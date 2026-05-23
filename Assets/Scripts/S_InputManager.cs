using UnityEngine;
using UnityEngine.UI;

public enum Direction { Up, Down, Left, Right }

public class S_InputManager : MonoBehaviour
{
    public event System.Action<Direction> OnMove;
    public event System.Action OnNewGame;

    [SerializeField] private Button newGameButton;
    
    // Configuración del swipe
    [SerializeField] private float swipeThreshold = 50f; // Distancia mínima para registrar swipe
    
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    private bool isSwiping = false;

    private void Start()
    {
        if (newGameButton != null)
            newGameButton.onClick.AddListener(() => OnNewGame?.Invoke());
    }

    private void Update()
    {
        HandleKeyboardInput();
        HandleMouseInput();
    }

    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            OnMove?.Invoke(Direction.Up);
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            OnMove?.Invoke(Direction.Down);
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            OnMove?.Invoke(Direction.Left);
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            OnMove?.Invoke(Direction.Right);

        // Alternativa WASD
        if (Input.GetKeyDown(KeyCode.W))
            OnMove?.Invoke(Direction.Up);
        else if (Input.GetKeyDown(KeyCode.S))
            OnMove?.Invoke(Direction.Down);
        else if (Input.GetKeyDown(KeyCode.A))
            OnMove?.Invoke(Direction.Left);
        else if (Input.GetKeyDown(KeyCode.D))
            OnMove?.Invoke(Direction.Right);
    }

    private void HandleMouseInput()
    {
        // Mouse o touch
        if (Input.GetMouseButtonDown(0))
        {
            touchStartPos = Input.mousePosition;
            isSwiping = true;
        }

        if (Input.GetMouseButton(0) && isSwiping)
        {
            touchEndPos = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            isSwiping = false;
            DetectSwipe();
        }
    }

    private void DetectSwipe()
    {
        Vector2 swipeDelta = touchEndPos - touchStartPos;
        float swipeDistance = swipeDelta.magnitude;
        
        if (swipeDistance < swipeThreshold)
            return;

        // Determinar dirección basada en el eje mayor
        float horizontalDistance = Mathf.Abs(swipeDelta.x);
        float verticalDistance = Mathf.Abs(swipeDelta.y);

        if (horizontalDistance > verticalDistance)
        {
            // Swipe horizontal
            if (swipeDelta.x > 0)
                OnMove?.Invoke(Direction.Right);
            else
                OnMove?.Invoke(Direction.Left);
        }
        else
        {
            // Swipe vertical
            if (swipeDelta.y > 0)
                OnMove?.Invoke(Direction.Up);
            else
                OnMove?.Invoke(Direction.Down);
        }
    }
}