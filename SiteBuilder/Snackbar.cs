using Bridge;
using Bridge.Html5;
using Bridge.jQuery2;
using TransitionJS;

namespace SiteBuilder
{
    public static class Snackbar
    {
        static jQuery snackbar = null;

        public static void Show(string text)
        {
            string snackbarElement = "<div style='position: fixed; top: 100%' class='shadow-3' id='snackbar'>" + text + "<button id='snackbarButton' onclick='SiteBuilder.Snackbar.close()'>Close</button></div>";

            if (snackbar == null)
            {
                new jQuery(Document.Body).Append(snackbarElement);
                Waves.Attach("#snackbarButton");
                snackbar = new jQuery("#snackbar");
                snackbar.Transition(new TransitionObject { Y = "-64px" }, 300);
            }
            else
            {
                snackbar.Transition(new TransitionObject { Y = "64px" }, 300, callback: () =>
                {
                    snackbar.Remove();

                    new jQuery(Document.Body).Append(snackbarElement);
                    Waves.Attach("#snackbarButton");
                    snackbar = new jQuery("#snackbar");
                    snackbar.Transition(new TransitionObject { Y = "-64px" }, 300);
                });
            }
        }

        public static void Close()
        {
            if (snackbar != null)
            {
                snackbar.Transition(new TransitionObject { Y = "64px" }, 300, callback: () =>
                {
                    snackbar.Remove();
                    snackbar = null;
                });
            }
        }
    }
}
