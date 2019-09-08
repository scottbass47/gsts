using UnityEngine;
using UnityEngine.EventSystems;

public class InputModule : StandaloneInputModule
{
    private static InputModule instance;

    public static bool Enabled
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<InputModule>();
            }

            return instance.enabled;
        }
        set
        {
            if (!instance)
            {
                instance = FindObjectOfType<InputModule>();
            }

            instance.enabled = value;
        }
    }

    [SerializeField]
    private Vector2 mousePosition;

    [SerializeField]
    private MouseState mouseState = new MouseState();

    /// <summary>
    /// Returns the position of the mouse in world space.
    /// </summary>
    /// <returns></returns>
    private Vector2 GetCursorPosition()
    {
        Vector2 mousePosition = Input.mousePosition;
        //Debug.Log($"Unscaled Screen Space: {mousePosition}");
        bool scaleToFit = true;

        var gameWidth = GameSettings.Settings.GameWidth;
        var gameHeight = GameSettings.Settings.GameHeight;

        //if in scale to fit, honor the ratio
        //if (GameSettings.ScaleMode == ScaleMode.ScaleToFit)
        if(scaleToFit)
        {
            Vector2 multiplier = Vector3.one;
            float gameRatio = GameSettings.Settings.ResolutionRatio;
            float screenRatio = Screen.width / (float)Screen.height;

            if (screenRatio > gameRatio)
            {
                //width is greater than height
                float width = Screen.height * gameRatio;
                multiplier.x = Screen.width / width;
                mousePosition.x -= (Screen.width - width) * 0.5f;
            }
            else
            {
                //height is greater than width
                float height = Screen.width / gameRatio;
                multiplier.y = Screen.height / height;
                mousePosition.y -= (Screen.height - height) * 0.5f;
            }


            //multiply the mouse positions
            mousePosition.x *= gameWidth / (float)Screen.width * 1f;
            mousePosition.y *= gameHeight / (float) Screen.height * 1f;
            mousePosition.x *= multiplier.x;
            mousePosition.y *= multiplier.y;
        }
        else
        {
            //otherwise assume the actual screen position
            mousePosition.x *= gameWidth / (float)Screen.width * 1f;
            mousePosition.y *= gameHeight / (float)Screen.height * 1f;
        }
        //Debug.Log($"Scaled Screen Space: {mousePosition}");

        Vector2 cursor = Camera.main.ScreenToWorldPoint(mousePosition);
        return cursor;
    }

    public override void UpdateModule()
    {
        mousePosition = GetCursorPosition();
        Mouse.WorldPos = mousePosition;

        //subtract camera position to make it screen position
        mousePosition.x -= Camera.main.transform.position.x;
        mousePosition.y -= Camera.main.transform.position.y;

        //add half of base to compensate for the center offset
        mousePosition += new Vector2(GameSettings.Settings.GameWidth, GameSettings.Settings.GameHeight) * 0.5f;

    }

    protected override MouseState GetMousePointerEventData(int id = 0)
    {
        bool created = GetPointerData(kMouseLeftId, out PointerEventData leftData, true);
        leftData.Reset();

        if (created)
        {
            //override the mouse position with our custom position
            leftData.position = mousePosition;
        }

        //replicate the original method functions
        Vector2 pos = mousePosition;
        leftData.delta = pos - leftData.position;
        leftData.position = pos;
        leftData.scrollDelta = Input.mouseScrollDelta;
        leftData.button = PointerEventData.InputButton.Left;
        eventSystem.RaycastAll(leftData, m_RaycastResultCache);
        RaycastResult raycast = FindFirstRaycast(m_RaycastResultCache);
        leftData.pointerCurrentRaycast = raycast;
        m_RaycastResultCache.Clear();

        GetPointerData(kMouseRightId, out PointerEventData rightData, true);
        CopyFromTo(leftData, rightData);
        rightData.button = PointerEventData.InputButton.Right;

        GetPointerData(kMouseMiddleId, out PointerEventData middleData, true);
        CopyFromTo(leftData, middleData);
        middleData.button = PointerEventData.InputButton.Middle;

        mouseState.SetButtonState(PointerEventData.InputButton.Left, StateForMouseButton(0), leftData);
        mouseState.SetButtonState(PointerEventData.InputButton.Right, StateForMouseButton(1), rightData);
        mouseState.SetButtonState(PointerEventData.InputButton.Middle, StateForMouseButton(2), middleData);

        return mouseState;
    }
}
