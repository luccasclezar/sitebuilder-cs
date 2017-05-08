/**
 * @version 1.0.0.0
 * @copyright Copyright ©  2017
 * @compiler Bridge.NET 15.7.0
 */
Bridge.assembly("SiteBuilder", function ($asm, globals) {
    "use strict";

    Bridge.define("SiteBuilder.App", {
        statics: {
            contextMenuVisible: false,
            contextMenuControlRef: null,
            controls: null,
            theme: null,
            config: {
                init: function () {
                    this.controls = new (System.Collections.Generic.Dictionary$2(String,SiteBuilder.Control))();
                    this.theme = new SiteBuilder.Theme();
                    Bridge.ready(this.main);
                }
            },
            getHasContainerSelected: function () {
                return SiteBuilder.App.contextMenuControlRef != null && SiteBuilder.App.controls.get($(SiteBuilder.App.contextMenuControlRef).data(SiteBuilder.Constants.identifier).toString()).getType() === SiteBuilder.ControlType.Container;
            },
            main: function () {
                SiteBuilder.ContextMenu.setBindings();
                SiteBuilder.Theme.setThemeBindings();

                $(document).keydown($asm.$.SiteBuilder.App.f1);
            },
            themeClicked: function () {
                SiteBuilder.Theme.updateThemePanel();

                if (document.getElementById("theme").getBoundingClientRect().left >= $("body").innerWidth()) {
                    $("#theme").transition({ x: "-512px" }, 300);
                } else {
                    $("#theme").transition({ x: "0" }, 300);
                }
            },
            toggleControlsList: function () {
                if (document.getElementById("controlsList").getBoundingClientRect().left >= $("body").innerWidth()) {
                    SiteBuilder.App.updateList();
                    $("#controlsList").transition({ x: "-512px" }, 300);
                } else {
                    $("#controlsList").transition({ x: "0" }, 300);
                }
            },
            updateList: function () {
                var $t;
                $t = Bridge.getEnumerator(SiteBuilder.App.controls);
                while ($t.moveNext()) {
                    var control = $t.getCurrent();
                    if (control.value.getType() === SiteBuilder.ControlType.Container) {
                        for (var i = 0; i < (Bridge.as(control.value, SiteBuilder.ContainerControl)).getChildren().getCount(); i = (i + 1) | 0) {
                            try {
                                var child = SiteBuilder.App.controls.get((Bridge.as(control.value, SiteBuilder.ContainerControl)).getChildren().getItem(i).toString());
                            }
                            catch ($e1) {
                                $e1 = System.Exception.create($e1);
                                if (Bridge.is($e1, System.Collections.Generic.KeyNotFoundException)) {
                                    (Bridge.as(control.value, SiteBuilder.ContainerControl)).getChildren().splice(i, 1);
                                    i = (i - 1) | 0;
                                } else {
                                    throw $e1;
                                }
                            }
                        }
                    }
                }

                $("#controlList").empty();
                SiteBuilder.Global.populateList($("#siteArea").children(), 0);
                SiteBuilder.Waves.attach(".listControl");
            },
            buttonClicked: function () {
                if (SiteBuilder.App.getHasContainerSelected()) {
                    var className = "floatingMaterialButton";

                    var addedElement = Bridge.merge(document.createElement('button'), {
                        className: className,
                        textContent: SiteBuilder.Strings.getDefault()
                    } );
                    $(SiteBuilder.App.contextMenuControlRef).append(addedElement);

                    var control = new SiteBuilder.ButtonControl();

                    SiteBuilder.App.generalControlAdd(SiteBuilder.ButtonControl, addedElement, control, className);
                } else {
                    SiteBuilder.Snackbar.show(SiteBuilder.Strings.getSelectContainer());
                }
            },
            containerClicked: function () {
                var addedElement = null;
                var className = "";
                var control = new SiteBuilder.ContainerControl();

                if (SiteBuilder.App.getHasContainerSelected()) {
                    className = "childDiv";
                    addedElement = Bridge.merge(document.createElement('div'), {
                        className: className
                    } );

                    $(SiteBuilder.App.contextMenuControlRef).append(addedElement);

                    control.setColor("card");
                } else {
                    className = "defaultDiv";
                    addedElement = Bridge.merge(document.createElement('div'), {
                        className: className
                    } );

                    $("#siteArea").append(addedElement);
                }

                SiteBuilder.App.generalControlAdd(SiteBuilder.ContainerControl, addedElement, control, className);
            },
            imageClicked: function () {
                var addedElement = null;
                var className = "";
                var control = new SiteBuilder.ImageControl();

                if (SiteBuilder.App.getHasContainerSelected()) {
                    className = "defaultImage";
                    addedElement = Bridge.merge(new Image(), {
                        className: className,
                        src: SiteBuilder.ImageControl.defaultImage
                    } );

                    $(SiteBuilder.App.contextMenuControlRef).append(addedElement);
                } else {
                    className = "fullImage";
                    addedElement = Bridge.merge(new Image(), {
                        className: className,
                        src: SiteBuilder.ImageControl.defaultImage
                    } );

                    $("#siteArea").append(addedElement);
                }

                SiteBuilder.App.generalControlAdd(SiteBuilder.ImageControl, addedElement, control, className);
            },
            labelClicked: function () {
                var addedElement = null;
                var className = "defaultLabel";
                var control = new SiteBuilder.LabelControl();

                if (SiteBuilder.App.getHasContainerSelected()) {
                    addedElement = Bridge.merge(document.createElement('p'), {
                        className: className,
                        textContent: SiteBuilder.Strings.getDefault()
                    } );
                    $(SiteBuilder.App.contextMenuControlRef).append(addedElement);

                    SiteBuilder.App.generalControlAdd(SiteBuilder.LabelControl, addedElement, control, className);
                } else {
                    SiteBuilder.Snackbar.show(SiteBuilder.Strings.getSelectContainer());
                }
            },
            generalControlAdd: function (T, element, control, className) {
                var addedElement = $(element);
                addedElement.data(SiteBuilder.Constants.identifier, control.getIdentifier());
                SiteBuilder.ControlExtensions.updateColors(T, control);
                SiteBuilder.App.controls.add(control.getIdentifier().toString(), control);

                SiteBuilder.Waves.attach$1(System.String.concat(".", className), System.Array.init(["no-pointer"], String));

                addedElement.click(function (e) {
                    SiteBuilder.ContextMenu.openMenu(control.getType(), e.currentTarget);
                    e.stopPropagation();
                });
            }
        },
        $entryPoint: true
    });

    Bridge.ns("SiteBuilder.App", $asm.$);

    Bridge.apply($asm.$.SiteBuilder.App, {
        f1: function (e) {
            if (e.which === 46) {
                SiteBuilder.ContextMenu.deleteControl();
            }
        }
    });

    Bridge.define("SiteBuilder.Control", {
        statics: {
            id: 0
        },
        config: {
            properties: {
                Identifier: 0,
                Type: 0,
                HorizontalAlignment: null,
                VerticalAlignment: null,
                MarginLeft: null,
                MarginTop: null,
                MarginRight: null,
                MarginBottom: null,
                Left: null,
                Top: null,
                Right: null,
                Bottom: null,
                Width: null,
                Height: null
            }
        }
    });

    Bridge.define("SiteBuilder.Constants", {
        statics: {
            identifier: "identifier"
        }
    });

    Bridge.define("SiteBuilder.ContextMenu", {
        statics: {
            calculateWidth: function (element) {
                var $t, $t1, $t2, $t3, $t4;
                var jElement = $(element);

                var control = System.Linq.Enumerable.from(SiteBuilder.App.controls).first(function (item) {
                        return Bridge.referenceEquals(item.key, jElement.data(SiteBuilder.Constants.identifier).toString());
                    }).value;
                var left = ($t = control.getLeft(), $t != null ? $t : "0px");
                var right = ($t1 = control.getRight(), $t1 != null ? $t1 : "0px");
                var width = ($t2 = control.getWidth(), $t2 != null ? $t2 : "100%");
                var paddingLeft = "0";
                var paddingRight = "0";

                try {
                    paddingLeft = ($t3 = (Bridge.as(control, SiteBuilder.ContainerControl)).getPaddingLeft(), $t3 != null ? $t3 : "0px");
                    paddingRight = ($t4 = (Bridge.as(control, SiteBuilder.ContainerControl)).getPaddingRight(), $t4 != null ? $t4 : "0px");
                }
                catch ($e1) {
                    $e1 = System.Exception.create($e1);
                    if (Bridge.is($e1, System.InvalidCastException)) {
                    } else {
                        throw $e1;
                    }
                }

                if (!System.String.contains(left,"px") && !System.String.contains(left,"%")) {
                    left = System.String.concat(left, "px");
                }

                if (!System.String.contains(right,"px") && !System.String.contains(right,"%")) {
                    right = System.String.concat(right, "px");
                }

                if (!System.String.contains(paddingLeft,"px") && !System.String.contains(paddingLeft,"%")) {
                    paddingLeft = System.String.concat(paddingLeft, "px");
                }

                if (!System.String.contains(paddingRight,"px") && !System.String.contains(paddingRight,"%")) {
                    paddingRight = System.String.concat(paddingRight, "px");
                }

                if (!System.String.contains(width,"px") && !System.String.contains(width,"%")) {
                    width = System.String.concat(width, "px");
                }

                $(SiteBuilder.App.contextMenuControlRef).css("width", System.String.concat("calc(", width, " - (", left, " + ", right, " + ", paddingLeft, " + ", paddingRight, "))"));
            },
            clearImageClicked: function () {
                (Bridge.as($(SiteBuilder.App.contextMenuControlRef).children().get(0), HTMLInputElement)).src = "../images/default_image.jpg";

                var control = SiteBuilder.App.controls.get($(SiteBuilder.App.contextMenuControlRef).data(SiteBuilder.Constants.identifier).toString());
                control['Source'] = null;
            },
            closeMenu: function () {
                $(SiteBuilder.App.contextMenuControlRef).css("border", "none");
                $("#contextMenu").css("visibility", "hidden");
                SiteBuilder.App.contextMenuControlRef = null;
                SiteBuilder.App.contextMenuVisible = false;
            },
            deleteControl: function () {
                var element = $(SiteBuilder.App.contextMenuControlRef);

                SiteBuilder.App.controls.remove(element.data(SiteBuilder.Constants.identifier).toString());
                element.remove();
                SiteBuilder.ContextMenu.closeMenu();
            },
            moveDown: function () {
                var jElement = $(SiteBuilder.App.contextMenuControlRef);
                var currentIndex = jElement.index();

                if (currentIndex !== ((jElement.parent().children().length - 1) | 0)) {
                    SiteBuilder.Global.swapElements(SiteBuilder.App.contextMenuControlRef, $(SiteBuilder.App.contextMenuControlRef).parent().children().get(((currentIndex + 1) | 0)));
                }
            },
            moveUp: function () {
                var currentIndex = $(SiteBuilder.App.contextMenuControlRef).index();

                if (currentIndex !== 0) {
                    SiteBuilder.Global.swapElements(SiteBuilder.App.contextMenuControlRef, $(SiteBuilder.App.contextMenuControlRef).parent().children().get(((currentIndex - 1) | 0)));
                }
            },
            openMenu: function (type, element) {
                $("#controlName").text(System.Enum.getName(SiteBuilder.ControlType, type));

                var offsetBottom = ((($(element).offset().top + $(element).height()) | 0) + 8) | 0;

                var contextMenu = $("#contextMenu");
                contextMenu.css("top", offsetBottom.toString());
                contextMenu.css("visibility", "visible");

                $(SiteBuilder.App.contextMenuControlRef).css("border", "none");
                SiteBuilder.App.contextMenuControlRef = element;
                $(SiteBuilder.App.contextMenuControlRef).css("border", "1px solid #FF3D00");

                var control = SiteBuilder.App.controls.get($(SiteBuilder.App.contextMenuControlRef).data(SiteBuilder.Constants.identifier).toString());
                SiteBuilder.ContextMenu.updateValues(SiteBuilder.Control, control);

                SiteBuilder.App.contextMenuVisible = true;
            },
            updateValues: function (T, control) {
                var contextMenu = $("#contextMenu");
                for (var i = 2; i < contextMenu.children().length; i = (i + 1) | 0) {
                    if (!Bridge.referenceEquals(contextMenu.children().get(i).className, "divider")) {
                        contextMenu.children().eq(i).hide();
                    }
                }

                for(var key in control) {;
                if (key.includes('$') || (typeof control[key] !== 'string' && typeof control[key] !== 'object')) continue;

                var Key = key;

                if (!System.String.isNullOrEmpty(control[key])) {
                    $(System.String.concat("#", SiteBuilder.Global.lowerFirst(Key), "Property")).val(control[key]);
                } else {
                    $(System.String.concat("#", SiteBuilder.Global.lowerFirst(Key), "Property")).val("");
                }

                if (System.String.contains(Key,"Left") || System.String.contains(Key,"Top") || System.String.contains(Key,"Right") || System.String.contains(Key,"Bottom")) {
                    Key = Key.replace(new RegExp("Left|Right|Top|Bottom"), "");
                }
                $(System.String.concat("#", SiteBuilder.Global.lowerFirst(Key), "PropertyDiv")).show();

                };

                // Its not necessary to add other properties because they are on the same div
                /* if (Script.Write<bool>("'MarginLeft' in control"))
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
            },
            setBindings: function () {
                /* ********* Margin **********/
                $("#marginLeftProperty").on("input", $asm.$.SiteBuilder.ContextMenu.f1);
                $("#marginTopProperty").on("input", $asm.$.SiteBuilder.ContextMenu.f2);
                $("#marginRightProperty").on("input", $asm.$.SiteBuilder.ContextMenu.f3);
                $("#marginBottomProperty").on("input", $asm.$.SiteBuilder.ContextMenu.f4);
                /* ********* Margin end **********/

                /* ********* Padding **********/
                $("#paddingLeftProperty").on("input", $asm.$.SiteBuilder.ContextMenu.f5);
                $("#paddingTopProperty").on("input", $asm.$.SiteBuilder.ContextMenu.f6);
                $("#paddingRightProperty").on("input", $asm.$.SiteBuilder.ContextMenu.f7);
                $("#paddingBottomProperty").on("input", $asm.$.SiteBuilder.ContextMenu.f8);
                /* ********* Padding end **********/

                /* ********* PositiOn **********/
                $("#leftProperty").on("input", $asm.$.SiteBuilder.ContextMenu.f9);
                $("#topProperty").on("input", $asm.$.SiteBuilder.ContextMenu.f10);
                $("#rightProperty").on("input", $asm.$.SiteBuilder.ContextMenu.f11);
                $("#bottomProperty").on("input", $asm.$.SiteBuilder.ContextMenu.f12);
                $("#widthProperty").on("input", $asm.$.SiteBuilder.ContextMenu.f13);
                $("#heightProperty").on("input", $asm.$.SiteBuilder.ContextMenu.f14);
                /* ********* Position end **********/

                $("#colorProperty").on("input", $asm.$.SiteBuilder.ContextMenu.f15);
                $("#elevationProperty").on("input", $asm.$.SiteBuilder.ContextMenu.f16);
                $("#fileSourceProperty").on("change", $asm.$.SiteBuilder.ContextMenu.f17);
                $("#fontColorProperty").on("input", $asm.$.SiteBuilder.ContextMenu.f18);
                $("#fontSizeProperty").on("input", $asm.$.SiteBuilder.ContextMenu.f19);
                $("#heightProperty").on("input", $asm.$.SiteBuilder.ContextMenu.f20);
                $("#horizontalAlignmentProperty").on("input", $asm.$.SiteBuilder.ContextMenu.f21);
                $("#horizontalTextAlignmentProperty").on("input", $asm.$.SiteBuilder.ContextMenu.f22);
                $("#insetProperty").change($asm.$.SiteBuilder.ContextMenu.f23);
                $("#textProperty").on("input", $asm.$.SiteBuilder.ContextMenu.f24);
                $("#verticalAlignmentProperty").on("input", $asm.$.SiteBuilder.ContextMenu.f25);
            }
        }
    });

    Bridge.ns("SiteBuilder.ContextMenu", $asm.$);

    Bridge.apply($asm.$.SiteBuilder.ContextMenu, {
        f1: function () {
            $(SiteBuilder.App.contextMenuControlRef).css("margin-left", $("#marginLeftProperty").val());
            SiteBuilder.App.controls.get($(SiteBuilder.App.contextMenuControlRef).data(SiteBuilder.Constants.identifier).toString()).setMarginLeft($("#marginLeftProperty").val());
        },
        f2: function () {
            $(SiteBuilder.App.contextMenuControlRef).css("margin-top", $("#marginTopProperty").val());
            SiteBuilder.App.controls.get($(SiteBuilder.App.contextMenuControlRef).data(SiteBuilder.Constants.identifier).toString()).setMarginTop($("#marginTopProperty").val());
        },
        f3: function () {
            $(SiteBuilder.App.contextMenuControlRef).css("margin-right", $("#marginRightProperty").val());
            SiteBuilder.App.controls.get($(SiteBuilder.App.contextMenuControlRef).data(SiteBuilder.Constants.identifier).toString()).setMarginRight($("#marginRightProperty").val());
        },
        f4: function () {
            $(SiteBuilder.App.contextMenuControlRef).css("margin-bottom", $("#marginBottomProperty").val());
            SiteBuilder.App.controls.get($(SiteBuilder.App.contextMenuControlRef).data(SiteBuilder.Constants.identifier).toString()).setMarginBottom($("#marginBottomProperty").val());
        },
        f5: function () {
            $(SiteBuilder.App.contextMenuControlRef).css("padding-left", $("#paddingLeftProperty").val());

            var control = SiteBuilder.App.controls.get($(SiteBuilder.App.contextMenuControlRef).data(SiteBuilder.Constants.identifier).toString());
            control['PaddingLeft'] = $('#paddingLeftProperty').val();
            SiteBuilder.ContextMenu.calculateWidth(SiteBuilder.App.contextMenuControlRef);
        },
        f6: function () {
            $(SiteBuilder.App.contextMenuControlRef).css("padding-top", $("#paddingTopProperty").val());

            var control = SiteBuilder.App.controls.get($(SiteBuilder.App.contextMenuControlRef).data(SiteBuilder.Constants.identifier).toString());
            control['PaddingTop'] = $('#paddingTopProperty').val();
        },
        f7: function () {
            $(SiteBuilder.App.contextMenuControlRef).css("padding-right", $("#paddingRightProperty").val());

            var control = SiteBuilder.App.controls.get($(SiteBuilder.App.contextMenuControlRef).data(SiteBuilder.Constants.identifier).toString());
            control['PaddingRight'] = $('#paddingRightProperty').val();
            SiteBuilder.ContextMenu.calculateWidth(SiteBuilder.App.contextMenuControlRef);
        },
        f8: function () {
            $(SiteBuilder.App.contextMenuControlRef).css("padding-bottom", $("#paddingBottomProperty").val());

            var control = SiteBuilder.App.controls.get($(SiteBuilder.App.contextMenuControlRef).data(SiteBuilder.Constants.identifier).toString());
            control['PaddingBottom'] = $('#paddingBottomProperty').val();
        },
        f9: function () {
            $(SiteBuilder.App.contextMenuControlRef).css("left", $("#leftProperty").val());
            SiteBuilder.App.controls.get($(SiteBuilder.App.contextMenuControlRef).data(SiteBuilder.Constants.identifier).toString()).setLeft($("#leftProperty").val());
            SiteBuilder.ContextMenu.calculateWidth(SiteBuilder.App.contextMenuControlRef);
        },
        f10: function () {
            $(SiteBuilder.App.contextMenuControlRef).css("top", $("#topProperty").val());
            SiteBuilder.App.controls.get($(SiteBuilder.App.contextMenuControlRef).data(SiteBuilder.Constants.identifier).toString()).setTop($("#topProperty").val());
        },
        f11: function () {
            SiteBuilder.App.controls.get($(SiteBuilder.App.contextMenuControlRef).data(SiteBuilder.Constants.identifier).toString()).setRight($("#rightProperty").val());
            SiteBuilder.ContextMenu.calculateWidth(SiteBuilder.App.contextMenuControlRef);
        },
        f12: function () {
            $(SiteBuilder.App.contextMenuControlRef).css("bottom", $("#bottomProperty").val());
            SiteBuilder.App.controls.get($(SiteBuilder.App.contextMenuControlRef).data(SiteBuilder.Constants.identifier).toString()).setBottom($("#bottomProperty").val());
        },
        f13: function () {
            SiteBuilder.App.controls.get($(SiteBuilder.App.contextMenuControlRef).data(SiteBuilder.Constants.identifier).toString()).setWidth($("#widthProperty").val());
            SiteBuilder.ContextMenu.calculateWidth(SiteBuilder.App.contextMenuControlRef);
        },
        f14: function () {
            $(SiteBuilder.App.contextMenuControlRef).css("height", $("#heightProperty").val());
            SiteBuilder.App.controls.get($(SiteBuilder.App.contextMenuControlRef).data(SiteBuilder.Constants.identifier).toString()).setHeight($("#heightProperty").val());
        },
        f15: function () {
            var val = $("#colorProperty").val();

            var control = SiteBuilder.App.controls.get($(SiteBuilder.App.contextMenuControlRef).data(SiteBuilder.Constants.identifier).toString());
            control['Color'] = val;

            if (System.String.contains(val,"#")) {
                $(SiteBuilder.App.contextMenuControlRef).css("background-color", val);
            } else {
                SiteBuilder.ControlExtensions.updateColors(SiteBuilder.Control, SiteBuilder.App.controls.get($(SiteBuilder.App.contextMenuControlRef).data(SiteBuilder.Constants.identifier).toString()));
            }
        },
        f16: function () {
            var isInset = (Bridge.referenceEquals($("#div-insetProperty:checked").val(), "on"));
            var elevationValue = $("#elevationProperty").val();

            $(SiteBuilder.App.contextMenuControlRef).css("box-shadow", System.String.concat("0 ", (!isInset ? System.String.concat(elevationValue, "px ") : "0 "), System.Double.format((System.Convert.toInt32(elevationValue) + System.Convert.toInt32(elevationValue) * 0.5), 'G'), "px rgba(0,0,0, .4)", (isInset ? " inset" : "")));

            $(SiteBuilder.App.contextMenuControlRef).css("z-index", !Bridge.referenceEquals($("#insetProperty:checked").val(), "on") ? $("#elevationProperty").val() : "0");

            var control = SiteBuilder.App.controls.get($(SiteBuilder.App.contextMenuControlRef).data(SiteBuilder.Constants.identifier).toString());
            control['Elevation'] = $('#elevationProperty').val();
        },
        f17: function () {
            if (System.String.isNullOrEmpty($("#fileSourceProperty").val())) {
                SiteBuilder.Global.processFile($(SiteBuilder.App.contextMenuControlRef).children().get(0));

                var control = SiteBuilder.App.controls.get($(SiteBuilder.App.contextMenuControlRef).data(SiteBuilder.Constants.identifier).toString());
                control['Color'] = $('#urlSourceProperty').val();
            } else {
                (Bridge.as($(SiteBuilder.App.contextMenuControlRef).children().get(0), HTMLInputElement)).src = "../images/default_image.jpg";

                var control1 = SiteBuilder.App.controls.get($(SiteBuilder.App.contextMenuControlRef).data(SiteBuilder.Constants.identifier).toString());
                control['Source'] = null;
            }
        },
        f18: function () {
            var val = $("#fontColorProperty").val();

            var control = SiteBuilder.App.controls.get($(SiteBuilder.App.contextMenuControlRef).data(SiteBuilder.Constants.identifier).toString());
            control['FontColor'] = val;

            if (System.String.contains(val,"#")) {
                $(SiteBuilder.App.contextMenuControlRef).css("color", val);
            } else {
                SiteBuilder.ControlExtensions.updateColors(SiteBuilder.Control, SiteBuilder.App.controls.get($(SiteBuilder.App.contextMenuControlRef).data(SiteBuilder.Constants.identifier).toString()));
            }
        },
        f19: function () {
            $(SiteBuilder.App.contextMenuControlRef).css("font-size", $("#fontSizeProperty").val());
            SiteBuilder.App.controls.get($(SiteBuilder.App.contextMenuControlRef).data(SiteBuilder.Constants.identifier).toString()).setVerticalAlignment($("#fontSizeProperty").val());
        },
        f20: function () {
            if (SiteBuilder.App.controls.get($(SiteBuilder.App.contextMenuControlRef).data(SiteBuilder.Constants.identifier).toString()).getType() !== SiteBuilder.ControlType.Image) {
                $(SiteBuilder.App.contextMenuControlRef).css("height", $("#heightProperty").val());
            } else {
                $(SiteBuilder.App.contextMenuControlRef).children().eq(0).css("height", $("#heightProperty").val());
            }

            SiteBuilder.App.controls.get($(SiteBuilder.App.contextMenuControlRef).data(SiteBuilder.Constants.identifier).toString()).setHeight($("#heightProperty").val());
        },
        f21: function () {
            $(SiteBuilder.App.contextMenuControlRef).css("float", $("#horizontalAlignmentProperty").val());

            var control = SiteBuilder.App.controls.get($(SiteBuilder.App.contextMenuControlRef).data(SiteBuilder.Constants.identifier).toString());
            control['HorizontalAlignment'] = $('#horizontalAlignmentProperty').val();
        },
        f22: function () {
            $(SiteBuilder.App.contextMenuControlRef).css("text-align", $("#horizontalTextAlignmentProperty").val());

            var control = SiteBuilder.App.controls.get($(SiteBuilder.App.contextMenuControlRef).data(SiteBuilder.Constants.identifier).toString());
            control['HorizontalTextAlignment'] = $('#horizontalTextAlignmentProperty').val();
        },
        f23: function () {
            var element = $("#insetProperty:checked");
            if (Bridge.referenceEquals(element.val(), "on") && !System.String.contains($(SiteBuilder.App.contextMenuControlRef).css("box-shadow"),"inset")) {
                $(SiteBuilder.App.contextMenuControlRef).css("box-shadow", System.String.concat($(SiteBuilder.App.contextMenuControlRef).css("box-shadow"), " inset"));
            }

            if (!Bridge.referenceEquals(element.val(), "on") && System.String.contains($(SiteBuilder.App.contextMenuControlRef).css("box-shadow"),"inset")) {
                $(SiteBuilder.App.contextMenuControlRef).css("box-shadow", System.String.replaceAll($(SiteBuilder.App.contextMenuControlRef).css("box-shadow"), " inset", ""));
            }

            $(SiteBuilder.App.contextMenuControlRef).css("z-index", !Bridge.referenceEquals(element.val(), "on") ? $("#elevationProperty").val() : "0");

            var control = SiteBuilder.App.controls.get($(SiteBuilder.App.contextMenuControlRef).data(SiteBuilder.Constants.identifier).toString());
            control['Inset'] = $('#insetProperty').val();
        },
        f24: function () {
            SiteBuilder.App.contextMenuControlRef.innerHTML = $("#textProperty").val().split("\n").join("<br>");

            var control = SiteBuilder.App.controls.get($(SiteBuilder.App.contextMenuControlRef).data(SiteBuilder.Constants.identifier).toString());
            control['Text'] = $('#textProperty').val().split('\n').join('<br>');
        },
        f25: function () {
            $(SiteBuilder.App.contextMenuControlRef).css("align-self", $("#verticalAlignmentProperty").val());
            SiteBuilder.App.controls.get($(SiteBuilder.App.contextMenuControlRef).data(SiteBuilder.Constants.identifier).toString()).setVerticalAlignment($("#verticalAlignmentProperty").val());
        }
    });

    Bridge.define("SiteBuilder.ControlExtensions", {
        statics: {
            updateColors: function (T, control) {
                
            for (var key in control) {
                if (!key.includes('get') && !key.includes('set') && control[key] && key.toLowerCase().includes('color')) {
                    var val = control[key].charAt(0).toLowerCase() + control[key].slice(1);

                if (System.Array.contains(System.Array.init(["primary", "primaryDark", "accent", "background", "backgroundDark", "card", "textPrimary", "textSecondary", "textDisabled", "textPrimaryBlack", "textSecondaryBlack", "textDisabledBlack", "textPrimaryWhite", "textSecondaryWhite", "textDisabledWhite"], String), val, String)) {
                    var cssAttr = "";
                    switch (SiteBuilder.Global.lowerFirst(key)) {
                        case "color": 
                            cssAttr = "background-color";
                            break;
                        case "fontColor": 
                            cssAttr = "color";
                            break;
                    }

                    var themeProp = (Bridge.as(SiteBuilder.App.theme[SiteBuilder.Global.upperFirst(val)], String));
                    var hashtag = !System.String.contains(themeProp,"r") && !System.String.contains(themeProp,"#") ? "#" : "";
                    $(SiteBuilder.Global.getElementByIdentifier(control.getIdentifier())).css(cssAttr, System.String.concat(hashtag, themeProp));

                    }
};
                }
            }
        }
    });

    Bridge.define("SiteBuilder.ControlType", {
        $kind: "enum",
        statics: {
            Button: 0,
            Container: 1,
            Image: 2,
            Label: 3
        }
    });

    Bridge.define("SiteBuilder.Global", {
        statics: {
            listOffset: 10000,
            move: function (T, list, oldIndex, newIndex) {
                var item = list.getItem(oldIndex);
                list.removeAt(oldIndex);
                list.insert(newIndex, item);
            },
            getElementByIdentifier: function (id, element) {
                if (element === void 0) { element = null; }
                if (element == null) {
                    element = document.getElementById("siteArea");
                }
                var siteArea = $(element).children();

                for (var i = 0; i < siteArea.length; i = (i + 1) | 0) {
                    if ($(siteArea.eq(i)).data(SiteBuilder.Constants.identifier) === id) {
                        return siteArea.eq(i);
                    }

                    if ($(siteArea.eq(i)).children().length > 0 && Bridge.referenceEquals(siteArea.eq(i).get(0).nodeName, "I")) {
                        var search = SiteBuilder.Global.getElementByIdentifier(id, siteArea.get(i));
                        if (search == null) {
                            return search;
                        }
                    }
                }

                return null;
            },
            isScrolledIntoView: function (element) {
                var docViewTop = $(window.self).scrollTop();
                var docViewBottom = (docViewTop + $(window.self).height()) | 0;

                var elemTop = $(element).offset().top;
                var elemBottom = (elemTop + $(element).height()) | 0;

                return ((elemBottom <= docViewBottom) && (elemTop >= docViewTop));
            },
            lowerFirst: function (str) {
                return System.String.concat(str.charAt(0).toLowerCase(), str.substr(1));
            },
            populateList: function (childrenArray, iteration) {
                for (var i = 0; i < childrenArray.length; i = (i + 1) | 0) {
                    (function () {
                        var control = SiteBuilder.App.controls.get($(childrenArray.eq(i)).data(SiteBuilder.Constants.identifier));

                        var addedChild = $("<div class='listControl'><p>" + System.Enum.toString(SiteBuilder.ControlType, control.getType()) + "</p></div>");
                        addedChild.data(SiteBuilder.Constants.identifier, ((control.getIdentifier() + SiteBuilder.Global.listOffset) | 0));
                        $("#controlsList").append(addedChild);

                        addedChild.click($asm.$.SiteBuilder.Global.f1);

                        $(addedChild).css("text-indent", ((16 + ((iteration * 32) | 0)) | 0));

                        if (control.getType() === SiteBuilder.ControlType.Container && $(childrenArray.get(i)).children().length > 0) {
                            SiteBuilder.Global.populateList($(childrenArray.get(i)).children(), ((iteration + 1) | 0));
                        }
                    }).call(this);
                }
            },
            processFile: function (element) {
                var files = (Bridge.as(element, HTMLInputElement)).files;

                if (files != null && files.length !== 0) {
                    var fr = Bridge.merge(new SiteBuilder.FileReader(), {
                        onload: function (file) {
                            (Bridge.as(element, HTMLInputElement)).src = fr.result;
                        }
                    } );
                    fr.readAsDataURL(files[0]);
                }
            },
            swapElements: function (from, $with) {
                var parent1 = null, next1 = null, parent2 = null, next2 = null;

                parent1 = from.parentNode;
                next1 = from.nextSibling;
                parent2 = $with.parentNode;
                next2 = $with.nextSibling;

                parent1.insertBefore($with, next1);
                parent2.insertBefore(from, next2);
            },
            upperFirst: function (str) {
                str = System.String.concat(str.charAt(0).toUpperCase(), str.substr(1));
                return str;
            }
        }
    });

    Bridge.ns("SiteBuilder.Global", $asm.$);

    Bridge.apply($asm.$.SiteBuilder.Global, {
        f1: function (e) {
            if (!SiteBuilder.Global.isScrolledIntoView(e.currentTarget)) {
                $("#siteArea").stop().animate(function (_o1) {
                        _o1.set("scrollTop", e.currentTarget.getBoundingClientRect().top);
                        return _o1;
                    }(new (System.Collections.Generic.Dictionary$2(String,System.Double))()), 400, "easeInOutCubic");
            }

            var identifier = ($(e.currentTarget).data(SiteBuilder.Constants.identifier) - SiteBuilder.Global.listOffset) | 0;
            SiteBuilder.ContextMenu.openMenu(SiteBuilder.App.controls.get(identifier.toString()).getType(), SiteBuilder.Global.getElementByIdentifier(identifier).get(0));
        }
    });

    Bridge.define("SiteBuilder.SaveLoad", {
        statics: {
            supportsStorage: function () {
                return window.localStorage != null;
            },
            load: function () {
                var $t;
                $("#savesList").empty();

                var savedKeys = Storages.localStorage.keys();
                $t = Bridge.getEnumerator(savedKeys);
                while ($t.moveNext()) {
                    var key = $t.getCurrent();
                    if (Bridge.referenceEquals(key, "move")) {
                        break;
                    }

                    if (System.String.contains(key,"controls")) {
                        var saveName = key.split("_")[0];

                        var entry = $(System.String.concat("<p class='saveEntry'>", saveName, "</p>"));
                        $("#savesList").append(entry);

                        entry.click($asm.$.SiteBuilder.SaveLoad.f1);
                    }
                }

                SiteBuilder.Waves.attach(".saveEntry");

                var loadDialog = $("#loadDialog");
                loadDialog.css("visibility", "visible");
                loadDialog.animate({ opacity: 1 }, 250);
            },
            loadProject: function (name) {
                var $t;
                SiteBuilder.ContextMenu.closeMenu();

                $("#siteArea").empty();
                SiteBuilder.App.controls = Bridge.merge(Bridge.createInstance(System.Collections.Generic.Dictionary$2(String,SiteBuilder.Control)), JSON.parse(Bridge.cast(window.self.localStorage.getItem(System.String.concat(name, "_controls")), String)));
                $("#siteArea").append(Bridge.cast(window.self.localStorage.getItem(System.String.concat(name, "_elements")), String));
                SiteBuilder.Control.id = Bridge.merge(Bridge.createInstance(System.Int32), JSON.parse(Bridge.cast(window.self.localStorage.getItem(System.String.concat(name, "_id")), String)));
                SiteBuilder.App.theme = Bridge.merge(Bridge.createInstance(SiteBuilder.Theme), JSON.parse(Bridge.cast(window.self.localStorage.getItem(System.String.concat(name, "_theme")), String)));

                $t = Bridge.getEnumerator(SiteBuilder.App.controls);
                while ($t.moveNext()) {
                    var control = $t.getCurrent();
                    var element = SiteBuilder.Global.getElementByIdentifier(control.value.getIdentifier());
                    element.click($asm.$.SiteBuilder.SaveLoad.f2);
                }

                SiteBuilder.SaveLoad.dialogCancel("loadDialog");
            },
            save: function () {
                var saveDialog = $("#saveDialog");
                saveDialog.css("visibility", "visible");
                saveDialog.animate({ opacity: 1 }, 250);
            },
            dialogSave: function () {
                var $t;
                $t = Bridge.getEnumerator(SiteBuilder.App.controls);
                while ($t.moveNext()) {
                    var control = $t.getCurrent();
                    var element = SiteBuilder.Global.getElementByIdentifier(control.value.getIdentifier());
                    element.attr("data-identifier", control.value.getIdentifier());
                }

                var saveName = $("#saveName").val();
                window.self.localStorage.setItem(System.String.concat(saveName, "_controls"), SiteBuilder.App.controls);
                window.self.localStorage.setItem(System.String.concat(saveName, "_elements"), $("#siteArea").html());
                window.self.localStorage.setItem(System.String.concat(saveName, "_id"), SiteBuilder.Control.id);
                window.self.localStorage.setItem(System.String.concat(saveName, "_theme"), SiteBuilder.App.theme);
                SiteBuilder.SaveLoad.dialogCancel("saveDialog");
            },
            dialogCancel: function (dialog) {
                var dialogElement = $(System.String.concat("#", dialog));
                dialogElement.animate({ opacity: 0 }, 250, "linear", function () {
                    dialogElement.css("visibility", "hidden");
                });
            }
        }
    });

    Bridge.ns("SiteBuilder.SaveLoad", $asm.$);

    Bridge.apply($asm.$.SiteBuilder.SaveLoad, {
        f1: function (e) {
            SiteBuilder.SaveLoad.loadProject($(e.currentTarget).text());
        },
        f2: function (e) {
            SiteBuilder.ContextMenu.openMenu(SiteBuilder.App.controls.get($(e.currentTarget).data(SiteBuilder.Constants.identifier)).getType(), e.currentTarget);
            e.stopPropagation();
        }
    });

    Bridge.define("SiteBuilder.Snackbar", {
        statics: {
            snackbar: null,
            show: function (text) {
                var snackbarElement = System.String.concat("<div style='position: fixed; top: 100%' class='shadow-3' id='snackbar'>", text, "<button id='snackbarButton' onclick='SiteBuilder.Snackbar.close()'>Close</button></div>");

                if (SiteBuilder.Snackbar.snackbar == null) {
                    $(document.body).append(snackbarElement);
                    SiteBuilder.Waves.attach("#snackbarButton");
                    SiteBuilder.Snackbar.snackbar = $("#snackbar");
                    SiteBuilder.Snackbar.snackbar.transition({ y: "-64px" }, 300);
                } else {
                    SiteBuilder.Snackbar.snackbar.transition({ y: "64px" }, 300, "easeInOutCubic", function () {
                        SiteBuilder.Snackbar.snackbar.remove();

                        $(document.body).append(snackbarElement);
                        SiteBuilder.Waves.attach("#snackbarButton");
                        SiteBuilder.Snackbar.snackbar = $("#snackbar");
                        SiteBuilder.Snackbar.snackbar.transition({ y: "-64px" }, 300);
                    });
                }
            },
            close: function () {
                if (SiteBuilder.Snackbar.snackbar != null) {
                    SiteBuilder.Snackbar.snackbar.transition({ y: "64px" }, 300, "easeInOutCubic", $asm.$.SiteBuilder.Snackbar.f1);
                }
            }
        }
    });

    Bridge.ns("SiteBuilder.Snackbar", $asm.$);

    Bridge.apply($asm.$.SiteBuilder.Snackbar, {
        f1: function () {
            SiteBuilder.Snackbar.snackbar.remove();
            SiteBuilder.Snackbar.snackbar = null;
        }
    });

    Bridge.define("SiteBuilder.Strings", {
        statics: {
            getDefault: function () {
                return "Default";
            },
            getSelectContainer: function () {
                return "Select a container";
            }
        }
    });

    Bridge.define("SiteBuilder.Theme", {
        statics: {
            updateThemePanel: function () {
                $("#theme-Primary").val(System.String.replaceAll(SiteBuilder.App.theme.getPrimary(), "#", ""));
                $("#theme-PrimaryDark").val(System.String.replaceAll(SiteBuilder.App.theme.getPrimaryDark(), "#", ""));
                $("#theme-Accent").val(System.String.replaceAll(SiteBuilder.App.theme.getAccent(), "#", ""));
                $("#theme-Theme").val(System.String.contains(SiteBuilder.App.theme.getBackground(),"EEE") ? "light" : "dark");
            },
            setThemeBindings: function () {
                $("#theme-Theme").on("change", $asm.$.SiteBuilder.Theme.f1);
                $("#theme-Primary").on("input", $asm.$.SiteBuilder.Theme.f2);
                $("#theme-PrimaryDark").on("input", $asm.$.SiteBuilder.Theme.f3);
                $("#theme-Accent").on("input", $asm.$.SiteBuilder.Theme.f4);
            },
            updateAllColors: function () {
                var $t;
                $t = Bridge.getEnumerator(SiteBuilder.App.controls);
                while ($t.moveNext()) {
                    var control = $t.getCurrent();
                    SiteBuilder.ControlExtensions.updateColors(SiteBuilder.Control, control.value);
                }
            }
        },
        config: {
            properties: {
                Primary: "#009688",
                PrimaryDark: "#00796B",
                Accent: "#FF5722",
                Background: "#EEE",
                BackgroundDark: "#E0E0E0",
                Card: "#FFF",
                TextPrimary: "rgba(0,0,0, .87)",
                TextSecondary: "rgba(0,0,0, .54)",
                TextDisabled: "rgba(0,0,0, .38)",
                TextPrimaryBlack: "rgba(0,0,0, .87)",
                TextSecondaryBlack: "rgba(0,0,0, .54)",
                TextDisabledBlack: "rgba(0,0,0, .38)",
                TextPrimaryWhite: "rgba(255,255,255, 1)",
                TextSecondaryWhite: "rgba(255,255,255, .7)",
                TextDisabledWhite: "rgba(255,255,255, .5)"
            }
        }
    });

    Bridge.ns("SiteBuilder.Theme", $asm.$);

    Bridge.apply($asm.$.SiteBuilder.Theme, {
        f1: function () {
            if (Bridge.referenceEquals($("#theme-Theme").val(), "light")) {
                SiteBuilder.App.theme.setBackground("#EEE");
                SiteBuilder.App.theme.setBackgroundDark("#E0E0E0");
                SiteBuilder.App.theme.setCard("#FFF");

                SiteBuilder.App.theme.setTextPrimary("rgba(0,0,0, .87)");
                SiteBuilder.App.theme.setTextSecondary("rgba(0,0,0, .54)");
                SiteBuilder.App.theme.setTextDisabled("rgba(0,0,0, .38)");
            } else {
                SiteBuilder.App.theme.setBackground("#303030");
                SiteBuilder.App.theme.setBackgroundDark("#212121");
                SiteBuilder.App.theme.setCard("#424242");

                SiteBuilder.App.theme.setTextPrimary("rgba(255,255,255, 1)");
                SiteBuilder.App.theme.setTextSecondary("rgba(255,255,255, .7)");
                SiteBuilder.App.theme.setTextDisabled("rgba(255,255,255, .5)");
            }
            SiteBuilder.Theme.updateAllColors();
        },
        f2: function () {
            SiteBuilder.App.theme.setPrimary($("#theme-Primary").val());
            SiteBuilder.Theme.updateAllColors();
        },
        f3: function () {
            SiteBuilder.App.theme.setPrimaryDark($("#theme-PrimaryDark").val());
            SiteBuilder.Theme.updateAllColors();
        },
        f4: function () {
            SiteBuilder.App.theme.setAccent($("#theme-Accent").val());
            SiteBuilder.Theme.updateAllColors();
        }
    });

    Bridge.define("SiteBuilder.Waves", {
        statics: {
            attach: function (selector) {
                Waves.attach(selector);
            },
            attach$1: function (selector, styles) {
                Waves.attach(selector, styles);
            }
        }
    });

    Bridge.define("SiteBuilder.ButtonControl", {
        inherits: [SiteBuilder.Control],
        config: {
            properties: {
                Color: null,
                FontColor: null,
                FontSize: null,
                Text: null
            }
        },
        ctor: function () {
            this.$initialize();
            SiteBuilder.Control.ctor.call(this);
            var $t;
            this.setType(SiteBuilder.ControlType.Button);
            this.setIdentifier(Bridge.identity(SiteBuilder.Control.id, ($t = (SiteBuilder.Control.id + 1) | 0, SiteBuilder.Control.id = $t, $t)));
    }
    });

    Bridge.define("SiteBuilder.ContainerControl", {
        inherits: [SiteBuilder.Control],
        config: {
            properties: {
                Color: null,
                Elevation: null,
                Inset: false,
                PaddingLeft: null,
                PaddingTop: null,
                PaddingRight: null,
                PaddingBottom: null,
                Children: null
            }
        },
        ctor: function () {
            this.$initialize();
            SiteBuilder.Control.ctor.call(this);
            var $t;
            this.setType(SiteBuilder.ControlType.Container);
            this.setIdentifier(Bridge.identity(SiteBuilder.Control.id, ($t = (SiteBuilder.Control.id + 1) | 0, SiteBuilder.Control.id = $t, $t)));
    }
    });

    Bridge.define("SiteBuilder.ImageControl", {
        inherits: [SiteBuilder.Control],
        statics: {
            defaultImage: "../images/default_image.jpg"
        },
        config: {
            properties: {
                Aspect: null,
                Elevation: null,
                Source: null
            }
        },
        ctor: function () {
            this.$initialize();
            SiteBuilder.Control.ctor.call(this);
            var $t;
            this.setType(SiteBuilder.ControlType.Image);
            this.setIdentifier(Bridge.identity(SiteBuilder.Control.id, ($t = (SiteBuilder.Control.id + 1) | 0, SiteBuilder.Control.id = $t, $t)));
    }
    });

    Bridge.define("SiteBuilder.LabelControl", {
        inherits: [SiteBuilder.Control],
        config: {
            properties: {
                FontColor: null,
                FontSize: null,
                HorizontalTextAlignment: null,
                Text: null
            }
        },
        ctor: function () {
            this.$initialize();
            SiteBuilder.Control.ctor.call(this);
            var $t;
            this.setType(SiteBuilder.ControlType.Label);
            this.setIdentifier(Bridge.identity(SiteBuilder.Control.id, ($t = (SiteBuilder.Control.id + 1) | 0, SiteBuilder.Control.id = $t, $t)));
    }
    });
});
