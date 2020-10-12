using UnityEngine.UIElements;

namespace BrowserDesktop.Menu
{
    public interface IDisplayerElement
    {
        VisualElement GetUXMLContent();

        void InitAndBindUI();
    }
}