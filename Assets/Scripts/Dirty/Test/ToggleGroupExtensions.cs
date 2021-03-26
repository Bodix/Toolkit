using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UI;

namespace Dirty.Test
{
    public static class ToggleGroupExtensions
    {
        private static FieldInfo _toggleListMember;

        public static IList<Toggle> GetToggles(this ToggleGroup grp)
        {
            if (_toggleListMember == null)
            {
                _toggleListMember = typeof(ToggleGroup).GetField("m_Toggles", BindingFlags.Instance | BindingFlags.NonPublic);
                if (_toggleListMember == null)
                    throw new Exception("UnityEngine.UI.ToggleGroup source code must have changed in latest version and is no longer compatible with this version of code.");
            }
            return _toggleListMember.GetValue(grp) as IList<Toggle>;
        }
    }
}
