using Bridge;
using Bridge.Html5;
using Bridge.jQuery2;
using System.Collections.Generic;

namespace SiteBuilder
{
    class SaveLoad
    {
        public static bool SupportsStorage()
        {
            return Window.LocalStorage != null;
        }

        public static void Load()
        {
            new jQuery("#savesList").Empty();

            var savedKeys = Script.Write<string[]>("Storages.localStorage.keys();");
            foreach (var key in savedKeys)
            {
                if (key == "move") break;

                if (key.Contains("controls"))
                {
                    var saveName = key.Split("_")[0];

                    var entry = new jQuery("<p class='saveEntry'>" + saveName + "</p>");
                    new jQuery("#savesList").Append(entry);

                    entry.Click(e =>
                    {
                        LoadProject(new jQuery(e.CurrentTarget).Text());
                    });
                }
            }

            Waves.Attach(".saveEntry");

            var loadDialog = new jQuery("#loadDialog");
            loadDialog.Css("visibility", "visible");
            loadDialog.Animate(new ObjectLiteral(new { opacity = 1 }), 250);
        }

        public static void LoadProject(string name)
        {
            ContextMenu.CloseMenu();

            new jQuery("#siteArea").Empty();
            App.Controls = JSON.Parse<Dictionary<string, Control>>((string)Window.Self.LocalStorage.GetItem(name + "_controls"));
            new jQuery("#siteArea").Append((string)Window.Self.LocalStorage.GetItem(name + "_elements"));
            Control.Id = JSON.Parse<int>((string)Window.Self.LocalStorage.GetItem(name + "_id"));
            App.Theme = JSON.Parse<Theme>((string)Window.Self.LocalStorage.GetItem(name + "_theme"));

            foreach (var control in App.Controls)
            {
                var element = Global.GetElementByIdentifier(control.Value.Identifier);
                element.Click(e =>
                {
                    ContextMenu.OpenMenu(App.Controls[new jQuery(e.CurrentTarget).Data<string>(Constants.Identifier)].Type, e.CurrentTarget);
                    e.StopPropagation();
                });
            }

            DialogCancel("loadDialog");
        }

        public static void Save()
        {
            var saveDialog = new jQuery("#saveDialog");
            saveDialog.Css("visibility", "visible");
            saveDialog.Animate(new ObjectLiteral(new { opacity = 1 }), 250);
        }

        public static void DialogSave()
        {
            foreach (var control in App.Controls)
            {
                var element = Global.GetElementByIdentifier(control.Value.Identifier);
                element.Attr("data-identifier", control.Value.Identifier);
            }

            var saveName = new jQuery("#saveName").Val();
            Window.Self.LocalStorage.SetItem(saveName + "_controls", App.Controls);
            Window.Self.LocalStorage.SetItem(saveName + "_elements", new jQuery("#siteArea").Html());
            Window.Self.LocalStorage.SetItem(saveName + "_id", Control.Id);
            Window.Self.LocalStorage.SetItem(saveName + "_theme", App.Theme);
            DialogCancel("saveDialog");
        }

        public static void DialogCancel(string dialog)
        {
            var dialogElement = new jQuery("#" + dialog);
            dialogElement.Animate(new ObjectLiteral(new { opacity = 0 }), 250, "linear", () =>
            {
                dialogElement.Css("visibility", "hidden");
            });
        }
    }
}
