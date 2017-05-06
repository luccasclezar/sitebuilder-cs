/**
 * @version 1.0.0.0
 * @copyright Copyright ©  2017
 * @compiler Bridge.NET 15.7.0
 */
Bridge.assembly("SiteBuilder", function ($asm, globals) {
    "use strict";

    Bridge.define("SiteBuilder.App", {
        $main: function () {
            // Create a new Button
            var button = Bridge.merge(document.createElement('button'), {
                innerHTML: "Click Me",
                onclick: $asm.$.SiteBuilder.App.f1
            } );

            // Add the Button to the page
            document.body.appendChild(button);

            // To confirm Bridge.NET is working: 
            // 1. Build this project (Ctrl + Shift + B)
            // 2. Browse to file /Bridge/www/demo.html
            // 3. Right-click on file and select "View in Browser" (Ctrl + Shift + W)
            // 4. File should open in a browser, click the "Submit" button
            // 5. Success!
        }
    });

    Bridge.ns("SiteBuilder.App", $asm.$);

    Bridge.apply($asm.$.SiteBuilder.App, {
        f1: function (ev) {
            // When Button is clicked, 
            // the Bridge Console should open.
            Bridge.Console.log("Success!");
        }
    });

    Bridge.define("SiteBuilder.Control", {
        config: {
            properties: {
                Identifier: 0,
                Type: 0,
                HorizontalAlignment: 0,
                VerticalAlignment: 0,
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

    Bridge.define("SiteBuilder.ControlType", {
        $kind: "enum",
        statics: {
            Button: 0,
            Container: 1,
            Image: 2,
            Label: 3
        }
    });

    Bridge.define("SiteBuilder.HorizontalAlignment", {
        $kind: "enum",
        statics: {
            Left: 0,
            Center: 1,
            Right: 2
        }
    });

    Bridge.define("SiteBuilder.VerticalAlignment", {
        $kind: "enum",
        statics: {
            Top: 0,
            Center: 1,
            Bottom: 2
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
            this.setType(SiteBuilder.ControlType.Button);
        }
    });

    Bridge.define("SiteBuilder.ContainerControl", {
        inherits: [SiteBuilder.Control],
        config: {
            properties: {
                Color: null,
                Elevation: 0,
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
            this.setType(SiteBuilder.ControlType.Container);
        }
    });

    Bridge.define("SiteBuilder.ImageControl", {
        inherits: [SiteBuilder.Control],
        config: {
            properties: {
                Aspect: null,
                Elevation: 0,
                Source: null
            }
        },
        ctor: function () {
            this.$initialize();
            SiteBuilder.Control.ctor.call(this);
            this.setType(SiteBuilder.ControlType.Image);
        }
    });

    Bridge.define("SiteBuilder.LabelControl", {
        inherits: [SiteBuilder.Control],
        config: {
            properties: {
                FontColor: null,
                FontSize: null,
                HorizontalTextAlignment: 0,
                Text: null
            }
        },
        ctor: function () {
            this.$initialize();
            SiteBuilder.Control.ctor.call(this);
            this.setType(SiteBuilder.ControlType.Label);
        }
    });
});
