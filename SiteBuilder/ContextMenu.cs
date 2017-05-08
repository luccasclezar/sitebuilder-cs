using Bridge;
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

            var control = App.Controls.First(item => item.Key == jElement.Data<int>(Constants.Identifier).ToString()).Value;
            var left = control.Left ?? "0px";
            var right = control.Right ?? "0px";
            var width = control.Width ?? "100%";
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

            var control = App.Controls[new jQuery(App.ContextMenuControlRef).Data<int>(Constants.Identifier).ToString()];
            Script.Write<string>("control['Source'] = null");
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
            var element = new jQuery(App.ContextMenuControlRef);

            App.Controls.Remove(element.Data<int>(Constants.Identifier).ToString());
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

            var control = App.Controls[new jQuery(App.ContextMenuControlRef).Data<int>(Constants.Identifier).ToString()];
            UpdateValues(control);

            App.contextMenuVisible = true;
        }

        public static void UpdateValues<T>(T control) where T : Control
        {
            var contextMenu = new jQuery("#contextMenu");
            for (int i = 2; i < contextMenu.Children().Length; i++)
            {
                if (contextMenu.Children().Get(i).ClassName != "divider")
                    contextMenu.Children().Eq(i).Hide();
            }

            Script.Write("for(var key in control) {");
            Script.Write("if (key.includes('$') || (typeof control[key] !== 'string' && typeof control[key] !== 'object')) continue;");

            var Key = Script.Write<string>("key");

            if (!string.IsNullOrEmpty(Script.Write<string>("control[key]")))
                new jQuery("#" + Key.LowerFirst() + "Property").Val(Script.Write<string>("control[key]"));
            else
                new jQuery("#" + Key.LowerFirst() + "Property").Val("");

            if (Key.Contains("Left") || Key.Contains("Top") || Key.Contains("Right") || Key.Contains("Bottom"))
                Key = Key.Replace(new Bridge.Text.RegularExpressions.Regex("Left|Right|Top|Bottom"), "");
            new jQuery("#" + Key.LowerFirst() + "PropertyDiv").Show();

            Script.Write("}");

            // Its not necessary to add other properties because they are on the same div
            /*if (Script.Write<bool>("'MarginLeft' in control"))
            {
                new jQuery("#marginPropertyDiv").Show();

                if (!string.IsNullOrEmpty(control.MarginLeft))
                    new jQuery("#marginLeftProperty").Val(control.MarginLeft);
                else
                    new jQuery("#marginLeftProperty").Val("");

                if (string.IsNullOrEmpty(control.MarginTop))
                    new jQuery("#marginTopProperty").Val(control.MarginTop);
                else
                    new jQuery("#marginTopProperty").Val("");

                if (string.IsNullOrEmpty(control.MarginRight))
                    new jQuery("#marginRightProperty").Val(control.MarginRight);
                else
                    new jQuery("#marginRightProperty").Val("");

                if (string.IsNullOrEmpty(control.MarginBottom))
                    new jQuery("#marginBottomProperty").Val(control.MarginBottom);
                else
                    new jQuery("#marginBottomProperty").Val("");
            }
            if (Script.Write<bool>("'PaddingLeft' in control"))
            {
                new jQuery("#paddingPropertyDiv").Show();

                string prop = Script.Write<string>("control['PaddingLeft']");
                if (string.IsNullOrEmpty(prop))
                    new jQuery("#paddingLeftProperty").Val(prop);
                else
                    new jQuery("#paddingLeftProperty").Val("");

                prop = Script.Write<string>("control['PaddingTop']");
                if (string.IsNullOrEmpty(prop))
                    new jQuery("#paddingTopProperty").Val(prop);
                else
                    new jQuery("#paddingTopProperty").Val("");

                prop = Script.Write<string>("control['PaddingRight']");
                if (string.IsNullOrEmpty(prop))
                    new jQuery("#paddingRightProperty").Val(prop);
                else
                    new jQuery("#paddingRightProperty").Val("");

                prop = Script.Write<string>("control['PaddingBottom']");
                if (string.IsNullOrEmpty(prop))
                    new jQuery("#paddingBottomProperty").Val(prop);
                else
                    new jQuery("#paddingBottomProperty").Val("");
            }
            if (Script.Write<bool>("'Width' in control"))
            {
                new jQuery("#positionPropertyDiv").Show();

                if (string.IsNullOrEmpty(control.Width))
                    new jQuery("#widthProperty").Val(control.Width);
                else
                    new jQuery("#widthProperty").Val("");

                if (string.IsNullOrEmpty(control.Height))
                    new jQuery("#heightProperty").Val(control.Height);
                else
                    new jQuery("#heightProperty").Val("");

                if (string.IsNullOrEmpty(control.Left))
                    new jQuery("#widthProperty").Val(control.Left);
                else
                    new jQuery("#widthProperty").Val("");

                if (string.IsNullOrEmpty(control.Top))
                    new jQuery("#widthProperty").Val(control.Top);
                else
                    new jQuery("#widthProperty").Val("");

                if (string.IsNullOrEmpty(control.Right))
                    new jQuery("#widthProperty").Val(control.Right);
                else
                    new jQuery("#widthProperty").Val("");

                if (string.IsNullOrEmpty(control.Bottom))
                    new jQuery("#widthProperty").Val(control.Bottom);
                else
                    new jQuery("#widthProperty").Val("");
            }

            if (Script.Write<bool>("'Color' in control"))
            {
                new jQuery("#colorPropertyDiv").Show();

                string prop = Script.Write<string>("control['Color']");
                if (string.IsNullOrEmpty(prop))
                    new jQuery("#colorProperty").Val(prop);
                else
                    new jQuery("#colorProperty").Val("");
            }
            if (Script.Write<bool>("'Elevation' in control"))
            {
                new jQuery("#elevationPropertyDiv").Show();

                string prop = Script.Write<string>("control['Elevation']");
                if (string.IsNullOrEmpty(prop))
                    new jQuery("#elevationProperty").Val(prop);
                else
                    new jQuery("#elevationProperty").Val("");
            }
            if (Script.Write<bool>("'FontColor' in control"))
            {
                new jQuery("#fontColorPropertyDiv").Show();

                string prop = Script.Write<string>("control['FontColor']");
                if (string.IsNullOrEmpty(prop))
                    new jQuery("#fontColorProperty").Val(prop);
                else
                    new jQuery("#fontColorProperty").Val("");
            }
            if (Script.Write<bool>("'FontSize' in control"))
            {
                new jQuery("#fontSizePropertyDiv").Show();

                string prop = Script.Write<string>("control['FontSize']");
                if (string.IsNullOrEmpty(prop))
                    new jQuery("#fontSizeProperty").Val(prop);
                else
                    new jQuery("#fontSizeProperty").Val("");
            }
            if (Script.Write<bool>("'Height' in control"))
            {
                new jQuery("#heightPropertyDiv").Show();

                if (string.IsNullOrEmpty(control.Height))
                    new jQuery("#heightProperty").Val(control.Height);
                else
                    new jQuery("#heightProperty").Val("");
            }
            if (Script.Write<bool>("'HorizontalAlignment' in control"))
            {
                new jQuery("#horizontalAlignmentPropertyDiv").Show();

                string prop = Script.Write<string>("control['HorizontalAlignment']");
                if (string.IsNullOrEmpty(prop))
                    new jQuery("#horizontalAlignmentProperty").Val(prop);
                else
                    new jQuery("#horizontalAlignmentProperty").Val("left");
            }
            if (Script.Write<bool>("'HorizontalTextAlignment' in control"))
            {
                new jQuery("#horizontalTextAlignmentPropertyDiv").Show();

                string prop = Script.Write<string>("control['HorizontalTextAlignment']");
                if (string.IsNullOrEmpty(prop))
                    new jQuery("#horizontalTextAlignmentProperty").Val(prop);
                else
                    new jQuery("#horizontalTextAlignmentProperty").Val("center");
            }
            if (Script.Write<bool>("'Inset' in control"))
            {
                new jQuery("#insetPropertyDiv").Show();

                string prop = Script.Write<string>("control['Inset']");
                if (string.IsNullOrEmpty(prop))
                    new jQuery("#insetProperty").Val(prop);
                else
                    new jQuery("#insetProperty").Val("");
            }
            if (Script.Write<bool>("'Source' in control"))
            {
                new jQuery("#sourcePropertyDiv").Show();

                string prop = Script.Write<string>("control['Source']");
                if (string.IsNullOrEmpty(prop) && prop.Contains("C:\\"))
                    new jQuery("#fileSourceProperty").Val(prop);
                else
                    new jQuery("#fileSourceProperty").Val("");
            }
            if (Script.Write<bool>("'Text' in control"))
            {
                new jQuery("#textPropertyDiv").Show();

                string prop = Script.Write<string>("control['Text']");
                if (string.IsNullOrEmpty(prop))
                    new jQuery("#textProperty").Val(prop);
                else
                    new jQuery("#textProperty").Val("");
            }
            if (Script.Write<bool>("'VerticalAlignment' in control"))
            {
                new jQuery("#verticalAlignmentPropertyDiv").Show();

                string prop = Script.Write<string>("control['VerticalAlignment']");
                if (string.IsNullOrEmpty(prop))
                    new jQuery("#verticalAlignmentProperty").Val(prop);
                else
                    new jQuery("#verticalAlignmentProperty").Val("center");
            }*/
        }

        public static void SetBindings()
        {
            /********** Margin **********/
            new jQuery("#marginLeftProperty").On("input", () =>
            {
                new jQuery(App.ContextMenuControlRef).Css("margin-left", new jQuery("#marginLeftProperty").Val());
                App.Controls[new jQuery(App.ContextMenuControlRef).Data<int>(Constants.Identifier).ToString()].MarginLeft = new jQuery("#marginLeftProperty").Val();
            });
            new jQuery("#marginTopProperty").On("input", () =>
            {
                new jQuery(App.ContextMenuControlRef).Css("margin-top", new jQuery("#marginTopProperty").Val());
                App.Controls[new jQuery(App.ContextMenuControlRef).Data<int>(Constants.Identifier).ToString()].MarginTop = new jQuery("#marginTopProperty").Val();
            });
            new jQuery("#marginRightProperty").On("input", () =>
            {
                new jQuery(App.ContextMenuControlRef).Css("margin-right", new jQuery("#marginRightProperty").Val());
                App.Controls[new jQuery(App.ContextMenuControlRef).Data<int>(Constants.Identifier).ToString()].MarginRight = new jQuery("#marginRightProperty").Val();
            });
            new jQuery("#marginBottomProperty").On("input", () =>
            {
                new jQuery(App.ContextMenuControlRef).Css("margin-bottom", new jQuery("#marginBottomProperty").Val());
                App.Controls[new jQuery(App.ContextMenuControlRef).Data<int>(Constants.Identifier).ToString()].MarginBottom = new jQuery("#marginBottomProperty").Val();
            });
            /********** Margin end **********/

            /********** Padding **********/
            new jQuery("#paddingLeftProperty").On("input", () =>
            {
                new jQuery(App.ContextMenuControlRef).Css("padding-left", new jQuery("#paddingLeftProperty").Val());

                var control = App.Controls[new jQuery(App.ContextMenuControlRef).Data<int>(Constants.Identifier).ToString()];
                Script.Write("control['PaddingLeft'] = $('#paddingLeftProperty').val()");
                CalculateWidth(App.ContextMenuControlRef);
            });
            new jQuery("#paddingTopProperty").On("input", () =>
            {
                new jQuery(App.ContextMenuControlRef).Css("padding-top", new jQuery("#paddingTopProperty").Val());

                var control = App.Controls[new jQuery(App.ContextMenuControlRef).Data<int>(Constants.Identifier).ToString()];
                Script.Write<string>("control['PaddingTop'] = $('#paddingTopProperty').val()");
            });
            new jQuery("#paddingRightProperty").On("input", () =>
            {
                new jQuery(App.ContextMenuControlRef).Css("padding-right", new jQuery("#paddingRightProperty").Val());

                var control = App.Controls[new jQuery(App.ContextMenuControlRef).Data<int>(Constants.Identifier).ToString()];
                Script.Write<string>("control['PaddingRight'] = $('#paddingRightProperty').val()");
                CalculateWidth(App.ContextMenuControlRef);
            });
            new jQuery("#paddingBottomProperty").On("input", () =>
            {
                new jQuery(App.ContextMenuControlRef).Css("padding-bottom", new jQuery("#paddingBottomProperty").Val());

                var control = App.Controls[new jQuery(App.ContextMenuControlRef).Data<int>(Constants.Identifier).ToString()];
                Script.Write<string>("control['PaddingBottom'] = $('#paddingBottomProperty').val()");
            });
            /********** Padding end **********/

            /********** PositiOn **********/
            new jQuery("#leftProperty").On("input", () =>
            {
                new jQuery(App.ContextMenuControlRef).Css("left", new jQuery("#leftProperty").Val());
                App.Controls[new jQuery(App.ContextMenuControlRef).Data<int>(Constants.Identifier).ToString()].Left = new jQuery("#leftProperty").Val();
                CalculateWidth(App.ContextMenuControlRef);
            });
            new jQuery("#topProperty").On("input", () =>
            {
                new jQuery(App.ContextMenuControlRef).Css("top", new jQuery("#topProperty").Val());
                App.Controls[new jQuery(App.ContextMenuControlRef).Data<int>(Constants.Identifier).ToString()].Top = new jQuery("#topProperty").Val();
            });
            new jQuery("#rightProperty").On("input", () =>
            {
                App.Controls[new jQuery(App.ContextMenuControlRef).Data<int>(Constants.Identifier).ToString()].Right = new jQuery("#rightProperty").Val();
                CalculateWidth(App.ContextMenuControlRef);
            });
            new jQuery("#bottomProperty").On("input", () =>
            {
                new jQuery(App.ContextMenuControlRef).Css("bottom", new jQuery("#bottomProperty").Val());
                App.Controls[new jQuery(App.ContextMenuControlRef).Data<int>(Constants.Identifier).ToString()].Bottom = new jQuery("#bottomProperty").Val();
            });
            new jQuery("#widthProperty").On("input", () =>
            {
                App.Controls[new jQuery(App.ContextMenuControlRef).Data<int>(Constants.Identifier).ToString()].Width = new jQuery("#widthProperty").Val();
                CalculateWidth(App.ContextMenuControlRef);
            });
            new jQuery("#heightProperty").On("input", () =>
            {
                new jQuery(App.ContextMenuControlRef).Css("height", new jQuery("#heightProperty").Val());
                App.Controls[new jQuery(App.ContextMenuControlRef).Data<int>(Constants.Identifier).ToString()].Height = new jQuery("#heightProperty").Val();
            });
            /********** Position end **********/

            new jQuery("#colorProperty").On("input", () =>
            {
                var val = new jQuery("#colorProperty").Val();

                var control = App.Controls[new jQuery(App.ContextMenuControlRef).Data<int>(Constants.Identifier).ToString()];
                Script.Write<string>("control['Color'] = val");

                if (val.Contains("#"))
                    new jQuery(App.ContextMenuControlRef).Css("background-color", val);
                else
                    App.Controls[new jQuery(App.ContextMenuControlRef).Data<int>(Constants.Identifier).ToString()].UpdateColors();
            });
            new jQuery("#elevationProperty").On("input", () =>
            {
                var isInset = (new jQuery("#div-insetProperty:checked").Val() == "on");
                var elevationValue = new jQuery("#elevationProperty").Val();

                new jQuery(App.ContextMenuControlRef).Css("box-shadow",
                    "0 " + (!isInset ? elevationValue + "px " : "0 ") + (Convert.ToInt32(elevationValue) + Convert.ToInt32(elevationValue) * .5) + "px rgba(0,0,0, .4)" + (isInset ? " inset" : ""));

                new jQuery(App.ContextMenuControlRef).Css("z-index", new jQuery("#insetProperty:checked").Val() != "on" ? new jQuery("#elevationProperty").Val() : "0");

                var control = App.Controls[new jQuery(App.ContextMenuControlRef).Data<int>(Constants.Identifier).ToString()];
                Script.Write<string>("control['Elevation'] = $('#elevationProperty').val()");
            });
            new jQuery("#fileSourceProperty").On("change", () =>
            {
                if (string.IsNullOrEmpty(new jQuery("#fileSourceProperty").Val()))
                {
                    Global.ProcessFile(new jQuery(App.ContextMenuControlRef).Children().Get(0));

                    var control = App.Controls[new jQuery(App.ContextMenuControlRef).Data<int>(Constants.Identifier).ToString()];
                    Script.Write<string>("control['Color'] = $('#urlSourceProperty').val()");
                }
                else
                {
                    (new jQuery(App.ContextMenuControlRef).Children().Get(0) as HTMLInputElement).Src = "../images/default_image.jpg";

                    var control = App.Controls[new jQuery(App.ContextMenuControlRef).Data<int>(Constants.Identifier).ToString()];
                    Script.Write<string>("control['Source'] = null");
                }
            });
            new jQuery("#fontColorProperty").On("input", () =>
            {
                var val = new jQuery("#fontColorProperty").Val();

                var control = App.Controls[new jQuery(App.ContextMenuControlRef).Data<int>(Constants.Identifier).ToString()];
                Script.Write<string>("control['FontColor'] = val");

                if (val.Contains("#"))
                    new jQuery(App.ContextMenuControlRef).Css("color", val);
                else
                    App.Controls[new jQuery(App.ContextMenuControlRef).Data<int>(Constants.Identifier).ToString()].UpdateColors();
            });
            new jQuery("#fontSizeProperty").On("input", () =>
            {
                new jQuery(App.ContextMenuControlRef).Css("font-size", new jQuery("#fontSizeProperty").Val());
                App.Controls[new jQuery(App.ContextMenuControlRef).Data<int>(Constants.Identifier).ToString()].VerticalAlignment = new jQuery("#fontSizeProperty").Val();
            });
            new jQuery("#heightProperty").On("input", () =>
            {
                if (App.Controls[new jQuery(App.ContextMenuControlRef).Data<int>(Constants.Identifier).ToString()].Type != ControlType.Image)
                    new jQuery(App.ContextMenuControlRef).Css("height", new jQuery("#heightProperty").Val());
                else
                    new jQuery(App.ContextMenuControlRef).Children().Eq(0).Css("height", new jQuery("#heightProperty").Val());

                App.Controls[new jQuery(App.ContextMenuControlRef).Data<int>(Constants.Identifier).ToString()].Height = new jQuery("#heightProperty").Val();
            });
            new jQuery("#horizontalAlignmentProperty").On("input", () =>
            {
                new jQuery(App.ContextMenuControlRef).Css("float", new jQuery("#horizontalAlignmentProperty").Val());

                var control = App.Controls[new jQuery(App.ContextMenuControlRef).Data<int>(Constants.Identifier).ToString()];
                Script.Write<string>("control['HorizontalAlignment'] = $('#horizontalAlignmentProperty').val()");
            });
            new jQuery("#horizontalTextAlignmentProperty").On("input", () =>
            {
                new jQuery(App.ContextMenuControlRef).Css("text-align", new jQuery("#horizontalTextAlignmentProperty").Val());

                var control = App.Controls[new jQuery(App.ContextMenuControlRef).Data<int>(Constants.Identifier).ToString()];
                Script.Write<string>("control['HorizontalTextAlignment'] = $('#horizontalTextAlignmentProperty').val()");
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

                new jQuery(App.ContextMenuControlRef).Css("z-index", element.Val() != "on" ? new jQuery("#elevationProperty").Val() : "0");

                var control = App.Controls[new jQuery(App.ContextMenuControlRef).Data<int>(Constants.Identifier).ToString()];
                Script.Write<string>("control['Inset'] = $('#insetProperty').val()");
            });
            new jQuery("#textProperty").On("input", () =>
            {
                App.ContextMenuControlRef.InnerHTML = new jQuery("#textProperty").Val().Split("\n").Join("<br>");

                var control = App.Controls[new jQuery(App.ContextMenuControlRef).Data<int>(Constants.Identifier).ToString()];
                Script.Write("control['Text'] = $('#textProperty').val().split('\\n').join('<br>')");
            });
            new jQuery("#verticalAlignmentProperty").On("input", () =>
            {
                new jQuery(App.ContextMenuControlRef).Css("align-self", new jQuery("#verticalAlignmentProperty").Val());
                App.Controls[new jQuery(App.ContextMenuControlRef).Data<int>(Constants.Identifier).ToString()].VerticalAlignment = new jQuery("#verticalAlignmentProperty").Val();
            });
        }
    }
}
