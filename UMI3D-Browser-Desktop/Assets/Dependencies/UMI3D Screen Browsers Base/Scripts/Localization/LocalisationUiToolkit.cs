using UnityEngine.UIElements;

public class LocalisationUiToolkit
{
    public static void InitLocalisation(UIDocument pDocument)
    {
        var textFields = pDocument.rootVisualElement.Query<TextField>().ToList();

        foreach (var textField in textFields)
        {
            if (textField.label == "") continue;
            if (textField.name == "") continue;
            var trad = LocalisationManager.Instance.GetTranslation(textField.name);
            if (trad != null)
                textField.label = trad;
        }

        var radioButtonGroups = pDocument.rootVisualElement.Query<RadioButtonGroup>().ToList();

        foreach (var radioButtonGroup in radioButtonGroups)
        {
            if (radioButtonGroup.label == "") continue;
            if (radioButtonGroup.name == "") continue;
            var trad = LocalisationManager.Instance.GetTranslation(radioButtonGroup.name);
            if (trad != null)
                radioButtonGroup.label = trad;
        }

        var sliders = pDocument.rootVisualElement.Query<Slider>().ToList();

        foreach (var slider in sliders)
        {
            if (slider.label == "") continue;
            if (slider.name == "") continue;
            var trad = LocalisationManager.Instance.GetTranslation(slider.name);
            if (trad != null)
                slider.label = trad;
        }

        var dropdowns = pDocument.rootVisualElement.Query<DropdownField>().ToList();

        foreach (var dropdown in dropdowns)
        {
            if (dropdown.label == "") continue;
            if (dropdown.name == "") continue;
            var trad = LocalisationManager.Instance.GetTranslation(dropdown.name);
            if (trad != null)
                dropdown.label = trad;
        }

        var radioButtons = pDocument.rootVisualElement.Query<RadioButton>().ToList();

        foreach (var radioButton in radioButtons)
        {
            if (radioButton.label == "") continue;
            if (radioButton.name == "") continue;
            var trad = LocalisationManager.Instance.GetTranslation(radioButton.name);
            if (trad != null)
                radioButton.label = trad;
        }

        var labels = pDocument.rootVisualElement.Query<TextElement>().ToList();

        foreach (var label in labels)
        {
            if (label.text == "") continue;
            if (label.name == "") continue;
            var trad = LocalisationManager.Instance.GetTranslation(label.name);
            if (trad != null)
                label.text = trad;
        }
    }
}
