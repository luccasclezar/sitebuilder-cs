using System;
using Bridge;
using Bridge.Html5;
using TransitionJS;
using Bridge.jQuery2;
using System.Collections.Generic;

namespace SiteBuilder
{
    public class App
    {
        public static bool contextMenuVisible = false;
        public static Element ContextMenuControlRef = null;
        public static Dictionary<string, Control> Controls = new Dictionary<string, Control>();
        public static Theme Theme = new Theme();

        [Ready]
        public static void Main()
        {
            ContextMenu.SetBindings();
            Theme.SetThemeBindings();

            jQuery.Document.KeyDown(e =>
            {
                if (e.Which == 46)
                    ContextMenu.DeleteControl();
            });
        }

        public static void ThemeClicked()
        {
            Theme.UpdateThemePanel();

            if (Document.GetElementById("theme").GetBoundingClientRect().Left >= new jQuery("body").InnerWidth())
                new jQuery("#theme").Transition(new TransitionObject { X = "-512px" }, 300);
            else
                new jQuery("#theme").Transition(new TransitionObject { X = "0" }, 300);
        }

        public static void ToggleControlList()
        {

        }
    }
}