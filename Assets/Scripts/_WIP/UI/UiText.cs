using TMPro;
using UnityEngine;

namespace _WIP.UI
{
    public class UiText : UiElement
    {
        [SerializeField]
        protected TextMeshProUGUI text;

        public TextMeshProUGUI Text => text;
    }
}