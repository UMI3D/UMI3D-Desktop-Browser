using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ApplicationLibraryDisplayer : MonoBehaviour
{
    public TMP_Text label;
    public TMP_Text date;
    public TMP_Dropdown dropdown;
    public Button delete;

    public void set(string label, List<string> libName, Action Ondelete)
    {
        this.label.text = label;
        dropdown.options = libName.Select(l => new TMP_Dropdown.OptionData(l)).ToList();
        this.delete.onClick.RemoveAllListeners();
        this.delete.onClick.AddListener(Ondelete.Invoke);
    }
}
