using UnityEngine;

namespace DebugSelector
{
    public class DebugSelector : DebugSelector<GameObjectSelectable>
    {
    }

    
    /// <summary>
    /// Can be used to find performance weaknesses by turning off objects sequentially
    /// and measuring the impact on performance in the profiler.
    /// </summary>
    /// <typeparam name="T">
    /// The logic for turning an object on and off can be overridden
    /// by implementing the ISelectable interface.
    /// </typeparam>
    public class DebugSelector<T> : MonoBehaviour where T : ISelectable
    {
#if UNITY_2021
        // https://issuetracker.unity3d.com/issues/serialized-items-in-a-list-overlap-each-other-when-adding-new-items-in-inspector
        // http://answers.unity.com/answers/1828671/view.html
        // https://forum.unity.com/threads/case-1412863-inspector-renders-data-of-first-element-of-array-over-the-others-when-expanded.1258950/#post-8046629
        [NonReorderable]
#endif
        public T[] Items;
        public bool IsInvertSelection;
        public bool AddSelectAllOption;
        public bool AddUnselectAllOption;
        public bool Logs;

        private int _currentItemIndex = -1;
        private bool _isSelectedAllOption;
        private bool _isUnselectedAllOption;

        private void Update()
        {
            if (GetInput())
                SelectNextItem();
        }

        protected virtual bool GetInput()
        {
            return Input.GetKeyDown(KeyCode.PageDown);
        }

        private void SelectNextItem()
        {
            if (_currentItemIndex == Items.Length - 1 && AddSelectAllOption && !_isSelectedAllOption)
            {
                _isSelectedAllOption = true;

                SelectAll();
            }
            else if (_currentItemIndex == Items.Length - 1 && AddUnselectAllOption && !_isUnselectedAllOption)
            {
                _isUnselectedAllOption = true;

                UnselectAll();
            }
            else
            {
                _isSelectedAllOption = false;
                _isUnselectedAllOption = false;
                _currentItemIndex = (_currentItemIndex + 1) % Items.Length;

                if (!IsInvertSelection)
                {
                    UnselectAll();
                    Items[_currentItemIndex].Select();
                }
                else
                {
                    SelectAll();
                    Items[_currentItemIndex].Unselect();
                }
            }

            if (Logs)
                UnityEngine.Debug.Log("Selected item: " + (_isUnselectedAllOption
                    ? "None"
                    : _isSelectedAllOption
                        ? "All"
                        : _currentItemIndex.ToString()));
        }

        private void SelectAll()
        {
            foreach (T group in Items)
                group.Select();
        }

        private void UnselectAll()
        {
            foreach (T group in Items)
                group.Unselect();
        }
    }
}