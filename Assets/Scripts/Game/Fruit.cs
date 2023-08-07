using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Fruit : MonoBehaviour, IDragHandler, IEndDragHandler
{
    // Unique fruit identifier
    public int Id;

    // Variables that refer to the next fruit to select
    static Color selectedColor = new Color(.5f, .5f, .5f, 1);
    // static Fruit nextSelected = null;

    [SerializeField] AudioClip fruitDestroyAudio;
    // Movement curve when moving the fruit
    [SerializeField] Ease ease;

    AudioSource audioSource;

    // Directions to the sides of the "fruit"
    Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;

    // Time it takes to change the positions of the fruits when they are moved
    float timeChangePositionFruits = 0.25f;

    // This variable is in charge of telling us if there is a touch or a click on the screen or more exactly on a fruit
    bool hasTouched = false;

    bool hasFruitDisable;

    // When the fruit activates, you reset its target position because sometimes it changes its position
    void OnEnable()
    {
        transform.localPosition = transform.localPosition + Vector3.up * 1;
        MoveFruit(transform.localPosition - Vector3.up * 1, false);

        transform.rotation = Quaternion.Euler(0, 0, 180);
        transform.DORotate(Vector3.zero, 0.2f);

        transform.localScale = Vector3.one * 0.3f;
        transform.DOScale(Vector3.one, 0.35f);
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Selected Fruit
    void SelectFruit()
    {
        spriteRenderer.color = selectedColor;
    }

    // Deselect fruit
    void DeselectFruit()
    {
        spriteRenderer.color = Color.white;
    }

    // Method in charge of detecting the mouse or the touch
    void OnMouseDown()
    {
        SelectFruit();
    }

    // Method in charge of detecting when you release the mouse or touch
    void OnMouseUp()
    {
        DeselectFruit();
    }

    // Method in charge of detecting when we are dragging the fruit
    public void OnDrag(PointerEventData eventData)
    {
        StartCoroutine(ChangeOnTheBoard());

        if (GameManager.Instance.GameMode == GameMode.ScoringObjective)
            MultiplicationFactor.Instance.ResetMultiplicationFactor();

        if (BoardManager.Instance.IsShifting || GUIManager.Instance.MoveCounter <= 0)
            return;

        // Calculate the drag direction
        Vector2 direction = eventData.position - eventData.pressPosition;

        // Initialize the direction label with the vertical direction
        Vector2 directionLabel = direction.y > 0 ? Vector2.up : Vector2.down;

        // If the horizontal direction is greater than the vertical direction, change the direction label to the horizontal direction
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            directionLabel = direction.x > 0 ? Vector2.right : Vector2.left;
        }

        // Check if the method has not been called yet
        if (!hasTouched)
        {
            StartCoroutine(BoardManager.Instance.SwapFruit(this.gameObject, directionLabel));
            hasTouched = true;
            return;
        }
    }

    // Method in charge of detecting when we drop the fruit and therefore it stops dragging
    public void OnEndDrag(PointerEventData eventData)
    {
        hasTouched = false;
    }

    /// <summary>
    /// Moves a fruit to the specified position and updates its name and position in the BoardManager.
    /// </summary>
    /// <param name="position">The position to move the fruit to.</param>
    /// <param name="setPositionFruits">Indicates if the fruit has to be updated on the board.</param>
    public void MoveFruit(Vector3 position, bool setPositionFruits = true)
    {
        int x = (int)position.x;
        int y = (int)position.y;

        // Use the DOTween library to move the fruit to the new position with a specified ease and time.
        this.transform.DOLocalMove(position, timeChangePositionFruits).SetEase(ease).OnComplete(() =>
        {
            if (setPositionFruits)
            {
                // Set the name of the fruit to include its new position.
                this.name = string.Format("Fruit[{0}] [{1}]", x, y);
                // Update the BoardManager with the new position of the fruit.
                BoardManager.Instance.Fruits[x, y] = this.gameObject;
            }
        });
    }

    public void DisableFruit()
    {
        transform.DORotate(new Vector3(0, 0, -120f), 0.12f);
        transform.DOScale(Vector3.one * 1.2f, 0.085f).OnComplete(() =>
        {
            transform.DOScale(Vector3.zero, 0.1f).onComplete = () =>
            {
                gameObject.SetActive(false);
            };
        });
    }

    // If there are no more changes on the board, it exits the coroutine
    IEnumerator ChangeOnTheBoard()
    {
        yield return new WaitUntil(() => !BoardManager.Instance.IsShifting);
    }

    
}