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
        public static bool HasContainerSelected
        {
            get
            {
                return ContextMenuControlRef != null && Controls[new jQuery(ContextMenuControlRef).Data<int>(Constants.Identifier).ToString()].Type == ControlType.Container;
            }
        }

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

        public static void ToggleControlsList()
        {
            if (Document.GetElementById("controlsList").GetBoundingClientRect().Left >= new jQuery("body").InnerWidth())
            {
                UpdateList();
                new jQuery("#controlsList").Transition(new TransitionObject { X = "-512px" }, 300);
            }
            else
                new jQuery("#controlsList").Transition(new TransitionObject { X = "0" }, 300);
        }

        public static void UpdateList()
        {
            foreach (var control in Controls)
            {
                if (control.Value.Type == ControlType.Container)
                {
                    for (int i = 0; i < (control.Value as ContainerControl).Children.Count; i++)
                    {
                        try
                        {
                            var child = Controls[(control.Value as ContainerControl).Children[i].ToString()];
                        }
                        catch (KeyNotFoundException)
                        {
                            (control.Value as ContainerControl).Children.Splice(i, 1);
                            i--;
                        }
                    }
                }
            }

            new jQuery("#controlList").Empty();
            Global.PopulateList(new jQuery("#siteArea").Children(), 0);
            Waves.Attach(".listControl");
        }

        #region Controls
        public static void ButtonClicked()
        {
            if (HasContainerSelected)
            {
                var className = "floatingMaterialButton";

                var addedElement = new HTMLButtonElement
                {
                    ClassName = className,
                    TextContent = Strings.Default
                };
                new jQuery(ContextMenuControlRef).Append(addedElement);

                var control = new ButtonControl();

                GeneralControlAdd(addedElement, control, className);
            }
            else
                Snackbar.Show(Strings.SelectContainer);
        }

        public static void ContainerClicked()
        {
            Element addedElement = null;
            var className = "";
            var control = new ContainerControl();

            if(HasContainerSelected)
            {
                className = "childDiv";
                addedElement = new HTMLDivElement
                {
                    ClassName = className
                };

                new jQuery(ContextMenuControlRef).Append(addedElement);

                control.Color = nameof(Theme.Card);
            }
            else
            {
                className = "defaultDiv";
                addedElement = new HTMLDivElement
                {
                    ClassName = className
                };

                new jQuery("#siteArea").Append(addedElement);
            }

            GeneralControlAdd(addedElement, control, className);
        }

        public static void ImageClicked()
        {
            Element addedElement = null;
            var className = "";
            var control = new ImageControl();

            if(HasContainerSelected)
            {
                className = "defaultImage";
                addedElement = new HTMLImageElement
                {
                    ClassName = className,
                    Src = ImageControl.DefaultImage
                };

                new jQuery(ContextMenuControlRef).Append(addedElement);
            }
            else
            {
                className = "fullImage";
                addedElement = new HTMLImageElement
                {
                    ClassName = className,
                    Src = ImageControl.DefaultImage
                };

                new jQuery("#siteArea").Append(addedElement);
            }

            GeneralControlAdd(addedElement, control, className);
        }

        public static void LabelClicked()
        {
            Element addedElement = null;
            var className = "defaultLabel";
            var control = new LabelControl();

            if (HasContainerSelected)
            {
                addedElement = new HTMLParagraphElement
                {
                    ClassName = className,
                    TextContent = Strings.Default
                };
                new jQuery(ContextMenuControlRef).Append(addedElement);

                GeneralControlAdd(addedElement, control, className);
            }
            else
                Snackbar.Show(Strings.SelectContainer);
        }

        public static void GeneralControlAdd<T>(Element element, T control, string className) where T : Control
        {
            var addedElement = new jQuery(element);
            addedElement.Data(Constants.Identifier, control.Identifier);
            control.UpdateColors();
            Controls.Add(control.Identifier.ToString(), control);

            Waves.Attach("." + className, new string[] { "no-pointer" });

            addedElement.Click(e =>
            {
                ContextMenu.OpenMenu(control.Type, e.CurrentTarget);
                e.StopPropagation();
            });
        }
        #endregion
    }
}