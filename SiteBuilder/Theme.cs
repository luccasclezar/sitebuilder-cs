using Bridge;
using Bridge.Html5;
using Bridge.jQuery2;

namespace SiteBuilder
{
    public class Theme
    {
        public string Primary { get; set; } = "#009688";
        public string PrimaryDark { get; set; } = "#00796B";
        public string Accent { get; set; } = "#FF5722";

        public string Background { get; set; } = "#EEE";
        public string BackgroundDark { get; set; } = "#E0E0E0";
        public string Card { get; set; } = "#FFF";

        public string TextPrimary { get; set; } = "rgba(0,0,0, .87)";
        public string TextSecondary { get; set; } = "rgba(0,0,0, .54)";
        public string TextDisabled { get; set; } = "rgba(0,0,0, .38)";

        public string TextPrimaryBlack { get; set; } = "rgba(0,0,0, .87)";
        public string TextSecondaryBlack { get; set; } = "rgba(0,0,0, .54)";
        public string TextDisabledBlack { get; set; } = "rgba(0,0,0, .38)";

        public string TextPrimaryWhite { get; set; } = "rgba(255,255,255, 1)";
        public string TextSecondaryWhite { get; set; } = "rgba(255,255,255, .7)";
        public string TextDisabledWhite { get; set; } = "rgba(255,255,255, .5)";

        public static void UpdateThemePanel()
        {
            new jQuery("#theme-Primary").Val(App.Theme.Primary.Replace("#", ""));
            new jQuery("#theme-PrimaryDark").Val(App.Theme.PrimaryDark.Replace("#", ""));
            new jQuery("#theme-Accent").Val(App.Theme.Accent.Replace("#", ""));
            new jQuery("#theme-Theme").Val(App.Theme.Background.Contains("EEE") ? "light" : "dark");
        }

        public static void SetThemeBindings()
        {
            new jQuery("#theme-Theme").On("change", () =>
            {
                if (new jQuery("#theme-Theme").Val() == "light")
                {
                    App.Theme.Background = "#EEE";
                    App.Theme.BackgroundDark = "#E0E0E0";
                    App.Theme.Card = "#FFF";

                    App.Theme.TextPrimary = "rgba(0,0,0, .87)";
                    App.Theme.TextSecondary = "rgba(0,0,0, .54)";
                    App.Theme.TextDisabled = "rgba(0,0,0, .38)";
                }
                else
                {
                    App.Theme.Background = "#303030";
                    App.Theme.BackgroundDark = "#212121";
                    App.Theme.Card = "#424242";

                    App.Theme.TextPrimary = "rgba(255,255,255, 1)";
                    App.Theme.TextSecondary = "rgba(255,255,255, .7)";
                    App.Theme.TextDisabled = "rgba(255,255,255, .5)";
                }
                UpdateAllColors();
            });
            new jQuery("#theme-Primary").On("input", () =>
            {
                App.Theme.Primary = new jQuery("#theme-Primary").Val();
                UpdateAllColors();
            });
            new jQuery("#theme-PrimaryDark").On("input", () =>
            {
                App.Theme.PrimaryDark = new jQuery("#theme-PrimaryDark").Val();
                UpdateAllColors();
            });
            new jQuery("#theme-Accent").On("input", () =>
            {
                App.Theme.Accent = new jQuery("#theme-Accent").Val();
                UpdateAllColors();
            });
        }

        public static void UpdateAllColors()
        {
            foreach(var control in App.Controls)
            {
                control.Value.UpdateColors();
            }
        }
    }
}
