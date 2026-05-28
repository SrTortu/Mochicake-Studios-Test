using UnityEngine;
using UnityEngine.UI;

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public class S_InputManager : MonoBehaviour
{
    public event System.Action<Direction> OnMove;
    public event System.Action OnNewGame;

    [SerializeField] private Button _newGameButton;
    [SerializeField] private float _swipeThreshold = 50f; // Distancia mínima para registrar swipe

    private Vector2 _touchStartPos;
    private Vector2 _touchEndPos;
    private bool _isSwiping = false;

    private void Start()
    {
        if (_newGameButton != null)
            _newGameButton.onClick.AddListener(() => OnNewGame?.Invoke());
    }

    private void Update()
    {
        HandleKeyboardInput();
        HandleMouseInput();
    }

    private void HandleKeyboardInput()
    {
        // ===================== WASD =====================
        if (Input.GetKeyDown(KeyCode.W))
            OnMove?.Invoke(Direction.Up);
        if (Input.GetKeyDown(KeyCode.S))
            OnMove?.Invoke(Direction.Down);
        if (Input.GetKeyDown(KeyCode.D))
            OnMove?.Invoke(Direction.Right);
        if (Input.GetKeyDown(KeyCode.A))
            OnMove?.Invoke(Direction.Left);

        // ===================== Flechas de direccion =====================
        if (Input.GetKeyDown(KeyCode.UpArrow))
            OnMove?.Invoke(Direction.Up);
        if (Input.GetKeyDown(KeyCode.DownArrow))
            OnMove?.Invoke(Direction.Down);
        if (Input.GetKeyDown(KeyCode.RightArrow))
            OnMove?.Invoke(Direction.Right);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            OnMove?.Invoke(Direction.Left);
    }

    private void HandleMouseInput()
    {
        // Mouse o touch
        if (Input.GetMouseButtonDown(0))
        {
            _touchStartPos = Input.mousePosition;
            _isSwiping = true;
        }

        if (Input.GetMouseButton(0) && _isSwiping)
        {
            _touchEndPos = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0) && _isSwiping)
        {
            _isSwiping = false;
            DetectSwipe();
        }
    }

    private void DetectSwipe()
    {
        Vector2 swipeDelta = _touchEndPos - _touchStartPos;
        float swipeDistance = swipeDelta.magnitude;

        if (swipeDistance < _swipeThreshold)
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