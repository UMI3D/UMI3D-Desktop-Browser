using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

[System.Serializable]
public class MicrophoneSliderEvent : UnityEvent<float>
{
}

public class MicrophoneSliderColor
{
    public float Startvalue;
    public Color color { set => StyleColor = new StyleColor(value); }
    public StyleColor StyleColor { get; private set; }

    public MicrophoneSliderColor(float startvalue, Color color)
    {
        this.Startvalue = startvalue;
        this.color = color;
    }
}

public class MicrophoneSlider
{
    const string Smicrophone_slider = "microphone_slider";
    const string Smask = "mask";
    const string Ssoundbar = "soundbar";
    const string Svoidbar = "voidbar";
    const string Scursor = "cursor";
    const string Scursor_body = "cursor_body";
    const string Sslider = "slider";
    const string Sname = "name";
    const string Svalue_input = "value_input";

    #region GetterSetter
    public VisualElement root { get; private set; }
    public float Min
    {
        get => min;
        set
        {
            min = value;
            if (max < min) max = min;
            Value = this.value;
            DisplayedValue = this.displayedValue;
        }
    }

    public string Name
    {
        get => name.text;
        set => name.text = value;
    }

    public float Max
    {
        get => max; 
        set
        {
            max = value;
            if (max < min) min = max;
            Value = this.value;
            DisplayedValue = this.displayedValue;
        }
    }
    public float Step { get => step; set => step = value; }
    public float Value
    {
        get => value;
        set
        {
            if (value < min) this.value = min;
            else if (value > max) this.value = max;
            else this.value = value;
            SetValue();
            OnValueChanged.Invoke(this.value);
        }
    }
    public float DisplayedValue
    {
        get => displayedValue;
        set
        {
            if (value < min) this.displayedValue = min;
            else if (value > max) this.displayedValue = max;
            else this.displayedValue = value;
            SetDisplayedValue();
            OnDisplayedValueChanged.Invoke(this.displayedValue);
        }
    }

    public List<MicrophoneSliderColor> Colors { get => colors; set => SetColors(value); }

    #endregion

    VisualElement mask;
    VisualElement soundBar;
    VisualElement voidBar;
    VisualElement cursor;
    VisualElement cursorBody;
    Label name;
    TextField input;
    public VisualElement slider { get; private set; }

    float min;
    float max;
    float step;

    float value;

    float displayedValue;
    private List<MicrophoneSliderColor> colors; 

    public MicrophoneSliderEvent OnValueChanged = new MicrophoneSliderEvent();
    public MicrophoneSliderEvent OnDisplayedValueChanged = new MicrophoneSliderEvent();

    Func<string, (bool,float)> InputToValue;
    Func<float, string> ValueToInput;

    public MicrophoneSlider(VisualElement root,string name,Func<string,(bool,float)> InputToValue, Func<float,string> ValueToInput, float value, float displayedValue, float min, float max, float step, IEnumerable<MicrophoneSliderColor> colors)
    {
        SetVisualElement(root);

        // first set value without setters.
        this.displayedValue = displayedValue;
        this.value = value;
        this.min = min;
        this.max = max;
        this.step = step;

        this.InputToValue = InputToValue;
        this.ValueToInput = ValueToInput;
        this.Name = name;

        SetColors(colors);

        if (umi3d.cdk.UMI3DClientServer.Exists)
            umi3d.cdk.UMI3DClientServer.Instance.StartCoroutine(waitBeforeSettingMicrophone());
    }

    IEnumerator waitBeforeSettingMicrophone() 
    {
        yield return null;
        // set max value via the setter to call all checks.
        this.Max = max;
    }

    #region Setter
    void SetColors(IEnumerable<MicrophoneSliderColor> colors)
    {
        this.colors = colors.OrderBy(c => c.Startvalue).ToList();
        RefreshColor();
    }

    void SetVisualElement(VisualElement root)
    {
        this.root = root;
        mask = root.Q<VisualElement>(Smask);
        soundBar = root.Q<VisualElement>(Ssoundbar);
        voidBar = root.Q<VisualElement>(Svoidbar);
        cursor = root.Q<VisualElement>(Scursor);
        cursorBody = root.Q<VisualElement>(Scursor_body);
        slider = root.Q<VisualElement>(Sslider);
        name = root.Q<Label>(Sname);
        input = root.Q<TextField>(Svalue_input);

        input.RegisterCallback<ChangeEvent<string>>(
            e => {
                var v = InputToValue(e.newValue);
                if (v.Item1)
                    Value = v.Item2;
            });

        var manip = new MicrophoneSliderManipulator(MouseButton.LeftMouse, e => SetValueFromLayout(e));


        slider.RegisterCallback<MouseDownEvent>(e =>
        {
            if (e.pressedButtons == 1)
            {
                SetValueFromLayout(e.localMousePosition.x);
                manip.OnMouseDown(e,Vector2.zero);
            }
        });

        cursor.AddManipulator(manip);

    }

    void SetValueFromLayout(float value)
    {
        var w = cursor.parent.layout.width;
        float percent = w != 0 ? value / cursor.parent.layout.width : 1;
        Value = percent * (Max - Min) + Min;
    }

    void SetValue()
    {
        float percent = Min != Max ? (Value - Min) / (Max - Min) : 1f;
        cursor.style.left = percent*cursor.parent.layout.width;
        input.value = ValueToInput?.Invoke(value) ?? "0";
        RefreshColor();
    }

    void SetDisplayedValue()
    {
        float percent = Min != Max ? (DisplayedValue - Min) * 100f / (Max - Min) : 100f;
        float antiPercent = 100 - percent;
        soundBar.style.flexGrow = new StyleFloat(percent);
        voidBar.style.flexGrow = new StyleFloat(antiPercent);
        RefreshColor();
    }
    #endregion

    public void RefreshColor()
    {
        var color = Colors.LastOrDefault(c => c.Startvalue <= DisplayedValue);
        soundBar.style.backgroundColor = color.StyleColor;
    }

}

public class MicrophoneSliderManipulator : MouseManipulator
{
    #region Init

    protected Vector2 start;
    protected bool active;
    protected Action<float> onValueChanged;
    public MouseButton button { get; }

    /// <summary>
    /// Constructor to create the manipulator.
    /// </summary>
    /// <param name="button">Button to use to move the VisualElement</param>
    public MicrophoneSliderManipulator(MouseButton button, Action<float> onValueChanged)
    {
        this.button = button;
        activators.Add(new ManipulatorActivationFilter { button = button });
        this.onValueChanged = onValueChanged;
        active = false;
    }

    #endregion

    #region Registrations

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
        target.RegisterCallback<MouseUpEvent>(OnMouseUp);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
        target.UnregisterCallback<MouseUpEvent>(OnMouseUp);

    }

    #endregion

    #region OnMouseDown
    public void OnMouseDown(MouseDownEvent e)
    {
        OnMouseDown(e, e.localMousePosition);
    }

    public virtual void OnMouseDown(MouseDownEvent e,Vector2 startPosition)
    {
        if (active)
        {
            e.StopImmediatePropagation();
            return;
        }

        if (CanStartManipulation(e))
        {
            start = startPosition;

            active = true;
            target.CaptureMouse();
            e.StopPropagation();

            OnMouseDownDone(e);
        }
    }

    /// <summary>
    /// Overrides it to performs action when a valid OnMouseDone is done by a user.
    /// </summary>
    protected virtual void OnMouseDownDone(MouseDownEvent e)
    {

    }

    #endregion

    #region OnMouseMove

    /// <summary>
    /// Makes the VisualElement follow user's mouse position.
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnMouseMove(MouseMoveEvent e)
    {
        if (!active || !target.HasMouseCapture())
            return;

        Vector2 diff = e.localMousePosition - start;

        //target.style.top = Mathf.Clamp(target.layout.y + diff.y, -target.layout.height, 0);
        var v = Mathf.Clamp(target.layout.x + diff.x, 0, target.parent.layout.width);
        target.style.left = v;
        onValueChanged?.Invoke(v);

        e.StopPropagation();

    }

    #endregion

    #region OnMouseUp

    /// <summary>
    /// Releases the VisualElement currently dragged.
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnMouseUp(MouseUpEvent e)
    {
        if (!active || !target.HasMouseCapture() || !CanStopManipulation(e))
            return;

        active = false;
        target.ReleaseMouse();
        e.StopPropagation();
        OnMouseUpDone();
    }

    /// <summary>
    /// Overrides it to performs action when a valid OnMouseUp is done by a user.
    /// </summary>
    protected virtual void OnMouseUpDone()
    {
    }

    #endregion
}
