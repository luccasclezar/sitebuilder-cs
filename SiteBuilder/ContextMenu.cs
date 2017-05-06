﻿using Bridge;
using Bridge.Html5;
using Bridge.jQuery2;
using System;
using System.Linq;

namespace SiteBuilder
{
    public static class ContextMenu
    {
        public static void CalculateWidth(Element element)
        {
            var jElement = new jQuery(element);

            var control = App.Controls.First(item => item.Key == jElement.Data<string>(Constants.Identifier)).Value;
            var left = control.Left ?? "0px";
            var right = control.Right ?? "0px";
            var width = control.Width ?? "0px";
            string paddingLeft = "0";
            string paddingRight = "0";

            try
            {
                paddingLeft = (control as ContainerControl).PaddingLeft ?? "0px";
                paddingRight = (control as ContainerControl).PaddingRight ?? "0px";
            }
            catch (InvalidCastException) { }

            if (!left.Contains("px") && !left.Contains("%"))
                left += "px";

            if (!right.Contains("px") && !right.Contains("%"))
                right += "px";

            if (!paddingLeft.Contains("px") && !paddingLeft.Contains("%"))
                paddingLeft += "px";

            if (!paddingRight.Contains("px") && !paddingRight.Contains("%"))
                paddingRight += "px";

            if (!width.Contains("px") && !width.Contains("%"))
                width += "px";

            new jQuery(App.ContextMenuControlRef).Css("width", "calc(" + width + " - (" + left + " + " + right + " + " + paddingLeft + " + " + paddingRight + "))");
        }

        public static void ClearImageClicked()
        {
            (new jQuery(App.ContextMenuControlRef).Children().Get(0) as HTMLInputElement).Src = "../images/default_image.jpg";

            var control = App.Controls[new jQuery(App.ContextMenuControlRef).Data<string>(Constants.Identifier)];
            control.GetType().GetProperty(nameof(ImageControl.Source), System.Reflection.BindingFlags.Public).SetValue(control, null);
        }

        public static void CloseMenu()
        {
            new jQuery(App.ContextMenuControlRef).Css("border", "none");
            new jQuery("#contextMenu").Css("visibility", "hidden");
            App.ContextMenuControlRef = null;
            App.contextMenuVisible = false;
        }

        public static void DeleteControl()
        {
            var element = new jQuery("#contextMenu");

            App.Controls.Remove(element.Data<string>(Constants.Identifier));
            element.Remove();
            CloseMenu();
        }

        public static void MoveDown()
        {
            var jElement = new jQuery(App.ContextMenuControlRef);
            var currentIndex = jElement.Index();

            if (currentIndex != jElement.Parent().Children().Length - 1)
                Global.SwapElements(App.ContextMenuControlRef, new jQuery(App.ContextMenuControlRef).Parent().Children().Get(currentIndex + 1));
        }

        public static void MoveUp()
        {
            var currentIndex = new jQuery(App.ContextMenuControlRef).Index();

            if (currentIndex != 0)
                Global.SwapElements(App.ContextMenuControlRef, new jQuery(App.ContextMenuControlRef).Parent().Children().Get(currentIndex - 1));
        }

        public static void OpenMenu(ControlType type, Element element)
        {
            new jQuery("#controlName").Text(Enum.GetName(typeof(ControlType), type));

            var offsetBottom = new jQuery(element).Offset().Top + new jQuery(element).Height() + 8;

            var contextMenu = new jQuery("#contextMenu");
            contextMenu.Css("top", offsetBottom.ToString());
            contextMenu.Css("visibility", "visible");

            new jQuery(App.ContextMenuControlRef).Css("border", "none");
            App.ContextMenuControlRef = element;
            new jQuery(App.ContextMenuControlRef).Css("border", "1px solid #FF3D00");

            var control = App.Controls[new jQuery(App.ContextMenuControlRef).Data<string>(Constants.Identifier)];
            UpdateValues(control);

            App.contextMenuVisible = true;
        }

        public static void UpdateValues(Control control)
        {
            var contextMenu = new jQuery("#contextMenu");
            for (int i = 2; i < contextMenu.Children().Length; i++)
            {
                if (contextMenu.Children().Get(i).ClassName != "divider")
                    contextMenu.Children().Eq(i).Hide();
            }

            // Its not necessary to add other properties because they are on the same div
            if (control.HasOwnProperty("MarginLeft"))
            {
                new jQuery("#marginPropertyDiv").Show();

                if (string.IsNullOrEmpty(control.MarginLeft))
                    new jQuery("#marginLeftProperty").Val(control.MarginLeft);
                else
                    new jQuery("#marginLeftProperty").Val("");
            }
            if (control.HasOwnProperty("PaddingLeft"))
            {
                new jQuery("#paddingPropertyDiv").Show();

                string prop = (string)control.GetType().GetProperty(nameof(ContainerControl.PaddingLeft), System.Reflection.BindingFlags.Public).GetValue(control, null);
                if (string.IsNullOrEmpty(prop))
                    new jQuery("#paddingLeftProperty").Val(prop);
                else
                    new jQuery("#paddingLeftProperty").Val("");
            }
            if (control.HasOwnProperty("Width"))
            {
                new jQuery("#positionPropertyDiv").Show();

                if (string.IsNullOrEmpty(control.Width))
                    new jQuery("#widthProperty").Val(control.Width);
                else
                    new jQuery("#widthProperty").Val("");
            }

            if (control.HasOwnProperty("Color"))
            {
                new jQuery("#colorPropertyDiv").Show();

                string prop = (string)control.GetType().GetProperty(nameof(ContainerControl.Color), System.Reflection.BindingFlags.Public).GetValue(control, null);
                if (string.IsNullOrEmpty(prop))
                    new jQuery("#colorProperty").Val(prop);
                else
                    new jQuery("#colorProperty").Val("");
            }
            if (control.HasOwnProperty("Elevation"))
            {
                new jQuery("#elevationPropertyDiv").Show();

                string prop = (string)control.GetType().GetProperty(nameof(ContainerControl.Elevation), System.Reflection.BindingFlags.Public).GetValue(control, null);
                if (string.IsNullOrEmpty(prop))
                    new jQuery("#elevationProperty").Val(prop);
                else
                    new jQuery("#elevationProperty").Val("");
            }
            if (control.HasOwnProperty("FontColor"))
            {
                new jQuery("#fontColorPropertyDiv").Show();

                string prop = (string)control.GetType().GetProperty(nameof(LabelControl.FontColor), System.Reflection.BindingFlags.Public).GetValue(control, null);
                if (string.IsNullOrEmpty(prop))
                    new jQuery("#fontColorProperty").Val(prop);
                else
                    new jQuery("#fontColorProperty").Val("");
            }
            if (control.HasOwnProperty("FontSize"))
            {
                new jQuery("#fontSizePropertyDiv").Show();

                string prop = (string)control.GetType().GetProperty(nameof(LabelControl.FontSize), System.Reflection.BindingFlags.Public).GetValue(control, null);
                if (string.IsNullOrEmpty(prop))
                    new jQuery("#fontSizeProperty").Val(prop);
                else
                    new jQuery("#fontSizeProperty").Val("");
            }
            if (control.HasOwnProperty("Height"))
            {
                new jQuery("#heightPropertyDiv").Show();

                if (string.IsNullOrEmpty(control.Height))
                    new jQuery("#heightProperty").Val(control.Height);
                else
                    new jQuery("#heightProperty").Val("");
            }
            if (control.HasOwnProperty("HorizontalAlignment"))
            {
                new jQuery("#horizontalAlignmentPropertyDiv").Show();

                string prop = (string)control.GetType().GetProperty(nameof(LabelControl.HorizontalAlignment), System.Reflection.BindingFlags.Public).GetValue(control, null);
                if (string.IsNullOrEmpty(prop))
                    new jQuery("#horizontalAlignmentProperty").Val(prop);
                else
                    new jQuery("#horizontalAlignmentProperty").Val("left");
            }
            if (control.HasOwnProperty("HorizontalTextAlignment"))
            {
                new jQuery("#horizontalTextAlignmentPropertyDiv").Show();

                string prop = (string)control.GetType().GetProperty(nameof(LabelControl.HorizontalTextAlignment), System.Reflection.BindingFlags.Public).GetValue(control, null);
                if (string.IsNullOrEmpty(prop))
                    new jQuery("#horizontalTextAlignmentProperty").Val(prop);
                else
                    new jQuery("#horizontalTextAlignmentProperty").Val("center");
            }
            if (control.HasOwnProperty("Inset"))
            {
                new jQuery("#insetPropertyDiv").Show();

                string prop = (string)control.GetType().GetProperty(nameof(ContainerControl.Inset), System.Reflection.BindingFlags.Public).GetValue(control, null);
                if (string.IsNullOrEmpty(prop))
                    new jQuery("#insetProperty").Val(prop);
                else
                    new jQuery("#insetProperty").Val("");
            }
            if (control.HasOwnProperty("Source"))
            {
                new jQuery("#sourcePropertyDiv").Show();

                string prop = (string)control.GetType().GetProperty(nameof(ImageControl.Source), System.Reflection.BindingFlags.Public).GetValue(control, null);
                if (string.IsNullOrEmpty(prop) && prop.Contains("C:\\"))
                    new jQuery("#fileSourceProperty").Val(prop);
                else
                    new jQuery("#fileSourceProperty").Val("");
            }
            if (control.HasOwnProperty("Text"))
            {
                new jQuery("#textPropertyDiv").Show();

                string prop = (string)control.GetType().GetProperty(nameof(LabelControl.Text), System.Reflection.BindingFlags.Public).GetValue(control, null);
                if (string.IsNullOrEmpty(prop))
                    new jQuery("#textProperty").Val(prop);
                else
                    new jQuery("#textProperty").Val("");
            }
            if (control.HasOwnProperty("VerticalAlignment"))
            {
                new jQuery("#verticalAlignmentPropertyDiv").Show();

                string prop = (string)control.GetType().GetProperty(nameof(LabelControl.VerticalAlignment), System.Reflection.BindingFlags.Public).GetValue(control, null);
                if (string.IsNullOrEmpty(prop))
                    new jQuery("#verticalAlignmentProperty").Val(prop);
                else
                    new jQuery("#verticalAlignmentProperty").Val("center");
            }
        }

        public static void SetBindings()
        {
            /********** Margin **********/
            new jQuery("#marginLeftProperty").On("input", () =>
            {
                new jQuery(App.ContextMenuControlRef).Css("margin-left", new jQuery("#marginLeftProperty").Val());
                App.Controls[new jQuery(App.ContextMenuControlRef).Data<string>(Constants.Identifier)].MarginLeft = new jQuery("#marginLeftProperty").Val();
            });
            new jQuery("#marginTopProperty").On("input", () =>
            {
                new jQuery(App.ContextMenuControlRef).Css("margin-top", new jQuery("#marginTopProperty").Val());
                App.Controls[new jQuery(App.ContextMenuControlRef).Data<string>(Constants.Identifier)].MarginTop = new jQuery("#marginTopProperty").Val();
            });
            new jQuery("#marginRightProperty").On("input", () =>
            {
                new jQuery(App.ContextMenuControlRef).Css("margin-right", new jQuery("#marginRightProperty").Val());
                App.Controls[new jQuery(App.ContextMenuControlRef).Data<string>(Constants.Identifier)].MarginRight = new jQuery("#marginRightProperty").Val();
            });
            new jQuery("#marginBottomProperty").On("input", () =>
            {
                new jQuery(App.ContextMenuControlRef).Css("margin-bottom", new jQuery("#marginBottomProperty").Val());
                App.Controls[new jQuery(App.ContextMenuControlRef).Data<string>(Constants.Identifier)].MarginBottom = new jQuery("#marginBottomProperty").Val();
            });
            /********** Margin end **********/

            /********** Padding **********/
            new jQuery("#paddingLeftProperty").On("input", () =>
            {
                new jQuery(App.ContextMenuControlRef).Css("padding-left", new jQuery("#paddingLeftProperty").Val());

                var control = App.Controls[new jQuery(App.ContextMenuControlRef).Data<string>(Constants.Identifier)];
                control.GetType().GetProperty(nameof(ContainerControl.PaddingLeft), System.Reflection.BindingFlags.Public).SetValue(control, new jQuery("#paddingLeftProperty").Val());
                CalculateWidth(App.ContextMenuControlRef);
            });
            new jQuery("#paddingTopProperty").On("input", () =>
            {
                new jQuery(App.ContextMenuControlRef).Css("padding-top", new jQuery("#paddingTopProperty").Val());

                var control = App.Controls[new jQuery(App.ContextMenuControlRef).Data<string>(Constants.Identifier)];
                control.GetType().GetProperty(nameof(ContainerControl.PaddingTop), System.Reflection.BindingFlags.Public).SetValue(control, new jQuery("#paddingTopProperty").Val());
            });
            new jQuery("#paddingRightProperty").On("input", () =>
            {
                new jQuery(App.ContextMenuControlRef).Css("padding-right", new jQuery("#paddingRightProperty").Val());

                var control = App.Controls[new jQuery(App.ContextMenuControlRef).Data<string>(Constants.Identifier)];
                control.GetType().GetProperty(nameof(ContainerControl.PaddingRight), System.Reflection.BindingFlags.Public).SetValue(control, new jQuery("#paddingRightProperty").Val());
                CalculateWidth(App.ContextMenuControlRef);
            });
            new jQuery("#paddingBottomProperty").On("input", () =>
            {
                new jQuery(App.ContextMenuControlRef).Css("padding-bottom", new jQuery("#paddingBottomProperty").Val());

                var control = App.Controls[new jQuery(App.ContextMenuControlRef).Data<string>(Constants.Identifier)];
                control.GetType().GetProperty(nameof(ContainerControl.PaddingBottom), System.Reflection.BindingFlags.Public).SetValue(control, new jQuery("#paddingBottomProperty").Val());
            });
            /********** Padding end **********/

            /********** PositiOn **********/
            new jQuery("#leftProperty").On("input", () =>
            {
                new jQuery(App.ContextMenuControlRef).Css("left", new jQuery("#leftProperty").Val());
                App.Controls[new jQuery(App.ContextMenuControlRef).Data<string>(Constants.Identifier)].Left = new jQuery("#leftProperty").Val();
                CalculateWidth(App.ContextMenuControlRef);
            });
            new jQuery("#topProperty").On("input", () =>
            {
                new jQuery(App.ContextMenuControlRef).Css("top", new jQuery("#topProperty").Val());
                App.Controls[new jQuery(App.ContextMenuControlRef).Data<string>(Constants.Identifier)].Top = new jQuery("#topProperty").Val();
            });
            new jQuery("#rightProperty").On("input", () =>
            {
                App.Controls[new jQuery(App.ContextMenuControlRef).Data<string>(Constants.Identifier)].Right = new jQuery("#rightProperty").Val();
                CalculateWidth(App.ContextMenuControlRef);
            });
            new jQuery("#bottomProperty").On("input", () =>
            {
                new jQuery(App.ContextMenuControlRef).Css("bottom", new jQuery("#bottomProperty").Val());
                App.Controls[new jQuery(App.ContextMenuControlRef).Data<string>(Constants.Identifier)].Bottom = new jQuery("#bottomProperty").Val();
            });
            new jQuery("#widthProperty").On("input", () =>
            {
                App.Controls[new jQuery(App.ContextMenuControlRef).Data<string>(Constants.Identifier)].Width = new jQuery("#widthProperty").Val();
                CalculateWidth(App.ContextMenuControlRef);
            });
            new jQuery("#heightProperty").On("input", () =>
            {
                new jQuery(App.ContextMenuControlRef).Css("height", new jQuery("#heightProperty").Val());
                App.Controls[new jQuery(App.ContextMenuControlRef).Data<string>(Constants.Identifier)].Height = new jQuery("#heightProperty").Val();
            });
            /********** Position end **********/
            
            new jQuery("#colorProperty").On("input", () =>
            {
                var Val = new jQuery("#colorProperty").Val();

                var control = App.Controls[new jQuery(App.ContextMenuControlRef).Data<string>(Constants.Identifier)];
                control.GetType().GetProperty(nameof(ContainerControl.Color), System.Reflection.BindingFlags.Public).SetValue(control, Val);

                if (Val.Contains("#"))
                    new jQuery(App.ContextMenuControlRef).Css("background-color", Val);
                else
                    App.Controls[new jQuery(App.ContextMenuControlRef).Data<string>(Constants.Identifier)].UpdateColors();
            });
            new jQuery("#elevationProperty").On("input", () =>
            {
                var isInset = (new jQuery("#div-insetProperty:checked").Val() == "on");
                var elevationValue = new jQuery("#elevatiOnProperty").Val();

                new jQuery(App.ContextMenuControlRef).Css("box-shadow",
                    "0 " + (!isInset ? elevationValue + "px " : "0 ") + (Convert.ToInt32(elevationValue) + Convert.ToInt32(elevationValue) * .5) + "px rgba(0,0,0, .4)" + (isInset ? " inset" : ""));

                new jQuery(App.ContextMenuControlRef).Css("z-index", new jQuery("#insetProperty:checked").Val() != "on" ? new jQuery("#elevationProperty").Val() : "0");

                var control = App.Controls[new jQuery(App.ContextMenuControlRef).Data<string>(Constants.Identifier)];
                control.GetType().GetProperty(nameof(ContainerControl.Elevation), System.Reflection.BindingFlags.Public).SetValue(control, new jQuery("#elevationProperty").Val());
            });
            new jQuery("#fileSourceProperty").On("change", () =>
            {
                if (string.IsNullOrEmpty(new jQuery("#fileSourceProperty").Val()))
                {
                    Global.ProcessFile(new jQuery(App.ContextMenuControlRef).Children().Get(0));

                    var control = App.Controls[new jQuery(App.ContextMenuControlRef).Data<string>(Constants.Identifier)];
                    control.GetType().GetProperty(nameof(ContainerControl.Color), System.Reflection.BindingFlags.Public).SetValue(control, new jQuery("#urlSourceProperty").Val());
                }
                else
                {
                    (new jQuery(App.ContextMenuControlRef).Children().Get(0) as HTMLInputElement).Src = "../images/default_image.jpg";

                    var control = App.Controls[new jQuery(App.ContextMenuControlRef).Data<string>(Constants.Identifier)];
                    control.GetType().GetProperty(nameof(ImageControl.Source), System.Reflection.BindingFlags.Public).SetValue(control, null);
                }
            });
            new jQuery("#fontColorProperty").On("input", () =>
            {
                var Val = new jQuery("#fontColorProperty").Val();

                var control = App.Controls[new jQuery(App.ContextMenuControlRef).Data<string>(Constants.Identifier)];
                control.GetType().GetProperty(nameof(LabelControl.FontColor), System.Reflection.BindingFlags.Public).SetValue(control, Val);

                if (Val.Contains("#"))
                    new jQuery(App.ContextMenuControlRef).Css("color", Val);
                else
                    App.Controls[new jQuery(App.ContextMenuControlRef).Data<string>(Constants.Identifier)].UpdateColors();
            });
            new jQuery("#fontSizeProperty").On("input", () =>
            {
                new jQuery(App.ContextMenuControlRef).Css("font-size", new jQuery("#fontSizeProperty").Val());
                App.Controls[new jQuery(App.ContextMenuControlRef).Data<string>(Constants.Identifier)].VerticalAlignment = new jQuery("#fontSizeProperty").Val();
            });
            new jQuery("#heightProperty").On("input", () =>
            {
                if (App.Controls[new jQuery(App.ContextMenuControlRef).Data<string>(Constants.Identifier)].Type != ControlType.Image)
                    new jQuery(App.ContextMenuControlRef).Css("height", new jQuery("#heightProperty").Val());
                else
                    new jQuery(App.ContextMenuControlRef).Children().Eq(0).Css("height", new jQuery("#heightProperty").Val());

                App.Controls[new jQuery(App.ContextMenuControlRef).Data<string>(Constants.Identifier)].Height = new jQuery("#heightProperty").Val();
            });
            new jQuery("#horizontalAlignmentProperty").On("input", () =>
            {
                new jQuery(App.ContextMenuControlRef).Css("float", new jQuery("#horizontalAlignmentProperty").Val());

                var control = App.Controls[new jQuery(App.ContextMenuControlRef).Data<string>(Constants.Identifier)];
                control.GetType().GetProperty(nameof(Control.HorizontalAlignment), System.Reflection.BindingFlags.Public).SetValue(control, new jQuery("#horizontalAlignmentProperty").Val());
            });
            new jQuery("#horizOntalTextAlignmentProperty").On("input", () =>
            {
                new jQuery(App.ContextMenuControlRef).Css("text-align", new jQuery("#horizOntalTextAlignmentProperty").Val());

                var control = App.Controls[new jQuery(App.ContextMenuControlRef).Data<string>(Constants.Identifier)];
                control.GetType().GetProperty(nameof(LabelControl.HorizontalTextAlignment), System.Reflection.BindingFlags.Public).SetValue(control, new jQuery("#horizOntalTextAlignmentProperty").Val());
            });
            new jQuery("#insetProperty").Change(() =>
            {
                var element = new jQuery("#insetProperty:checked");
                if (element.Val() == "on" && !new jQuery(App.ContextMenuControlRef).Css("box-shadow").Contains("inset"))
                {
                    new jQuery(App.ContextMenuControlRef).Css("box-shadow", new jQuery(App.ContextMenuControlRef).Css("box-shadow") + " inset");
                }

                if (element.Val() != "on" && new jQuery(App.ContextMenuControlRef).Css("box-shadow").Contains("inset"))
                {
                    new jQuery(App.ContextMenuControlRef).Css("box-shadow", new jQuery(App.ContextMenuControlRef).Css("box-shadow").Replace(" inset", ""));
                }

                new jQuery(App.ContextMenuControlRef).Css("z-index", element.Val() != "on" ? new jQuery("#elevatiOnProperty").Val() : "0");

                var control = App.Controls[new jQuery(App.ContextMenuControlRef).Data<string>(Constants.Identifier)];
                control.GetType().GetProperty(nameof(ContainerControl.Inset), System.Reflection.BindingFlags.Public).SetValue(control, new jQuery("#insetProperty").Val());
            });
            new jQuery("#textProperty").On("input", () =>
            {
                App.ContextMenuControlRef.InnerHTML = new jQuery("#textProperty").Val().Split("\n").Join("<br>");

                var control = App.Controls[new jQuery(App.ContextMenuControlRef).Data<string>(Constants.Identifier)];
                control.GetType().GetProperty(nameof(LabelControl.Text), System.Reflection.BindingFlags.Public).SetValue(control, new jQuery("#textProperty").Val().Split("\n").Join("<br>"));
            });
            new jQuery("#verticalAlignmentProperty").On("input", () =>
            {
                new jQuery(App.ContextMenuControlRef).Css("align-self", new jQuery("#verticalAlignmentProperty").Val());
                App.Controls[new jQuery(App.ContextMenuControlRef).Data<string>(Constants.Identifier)].VerticalAlignment = new jQuery("#verticalAlignmentProperty").Val();
            });
        }
    }
}