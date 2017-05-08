using Bridge;
using Bridge.Html5;
using Bridge.jQuery2;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SiteBuilder
{
    public class Control
    {
        public static int Id = 0;

        public int Identifier { get; set; }
        public ControlType Type { get; set; }

        public string HorizontalAlignment { get; set; }
        public string VerticalAlignment { get; set; }

        public string MarginLeft { get; set; }
        public string MarginTop { get; set; }
        public string MarginRight { get; set; }
        public string MarginBottom { get; set; }

        public string Left { get; set; }
        public string Top { get; set; }
        public string Right { get; set; }
        public string Bottom { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
    }

    public class ButtonControl : Control
    {
        public string Color { get; set; }
        public string FontColor { get; set; }
        public string FontSize { get; set; }
        public string Text { get; set; }

        public ButtonControl()
        {
            Type = ControlType.Button;
            Identifier = Id++;
        }
    }

    public class ContainerControl : Control
    {
        public string Color { get; set; }
        public string Elevation { get; set; }
        public bool Inset { get; set; }

        public string PaddingLeft { get; set; }
        public string PaddingTop { get; set; }
        public string PaddingRight { get; set; }
        public string PaddingBottom { get; set; }

        public List<int> Children { get; set; }

        public ContainerControl()
        {
            Type = ControlType.Container;
            Identifier = Id++;
        }
    }

    public class ImageControl : Control
    {
        public static string DefaultImage = "../images/default_image.jpg";

        public string Aspect { get; set; }
        public string Elevation { get; set; }
        public string Source { get; set; }

        public ImageControl()
        {
            Type = ControlType.Image;
            Identifier = Id++;
        }
    }

    public class LabelControl : Control
    {
        public string FontColor { get; set; }
        public string FontSize { get; set; }
        public string HorizontalTextAlignment { get; set; }
        public string Text { get; set; }

        public LabelControl()
        {
            Type = ControlType.Label;
            Identifier = Id++;
        }
    }

    public enum ControlType
    {
        Button,
        Container,
        Image,
        Label
    }

    public static class ControlExtensions
    {
        public static void UpdateColors<T>(this T control) where T : Control
        {
            Script.Write(@"
            for (var key in control) {
                if (!key.includes('get') && !key.includes('set') && control[key] && key.toLowerCase().includes('color')) {
                    var val = control[key].charAt(0).toLowerCase() + control[key].slice(1);");
            
            if (new[] { nameof(Theme.Primary), nameof(Theme.PrimaryDark), nameof(Theme.Accent),
                        nameof(Theme.Background), nameof(Theme.BackgroundDark), nameof(Theme.Card),
                        nameof(Theme.TextPrimary), nameof(Theme.TextSecondary), nameof(Theme.TextDisabled),
                        nameof(Theme.TextPrimaryBlack), nameof(Theme.TextSecondaryBlack), nameof(Theme.TextDisabledBlack),
                        nameof(Theme.TextPrimaryWhite), nameof(Theme.TextSecondaryWhite), nameof(Theme.TextDisabledWhite), }.Contains(Script.Write<string>("val")))
            {
                var cssAttr = "";
                switch (Script.Write<string>("key").LowerFirst())
                {
                    case nameof(ContainerControl.Color):
                        cssAttr = "background-color";
                        break;
                    case nameof(LabelControl.FontColor):
                        cssAttr = "color";
                        break;
                }

                var themeProp = (App.Theme[Script.Write<string>("val").UpperFirst()] as string);
                var hashtag = !themeProp.Contains("r") && !themeProp.Contains("#") ? "#" : "";
                new jQuery(Global.GetElementByIdentifier(control.Identifier)).Css(cssAttr, hashtag + themeProp);

                Script.Write("}\n}");
            }
        }
    }
}