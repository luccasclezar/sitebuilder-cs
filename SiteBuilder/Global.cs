using Bridge;
using Bridge.Html5;
using Bridge.jQuery2;
using TransitionJS;
using System.Collections.Generic;

namespace SiteBuilder
{
    static class Global
    {
        static int ListOffset = 10000;

        public static void Move<T>(this List<T> list, int oldIndex, int newIndex)
        {
            T item = list[oldIndex];
            list.RemoveAt(oldIndex);
            list.Insert(newIndex, item);
        }

        public static jQuery GetElementByIdentifier(int id, Element element = null)
        {
            if (element == null) element = Document.GetElementById("#siteArea");
            var siteArea = new jQuery(element).Children();

            for (int i = 0; i < siteArea.Length; i++)
            {
                if (new jQuery(siteArea.Eq(i)).Data<int>(Constants.Identifier) == id)
                    return siteArea.Eq(i);

                if (new jQuery(siteArea.Eq(i)).Children().Length > 0 && siteArea.Eq(i).Get(0).NodeName == "I")
                {
                    var search = GetElementByIdentifier(id, siteArea.Get(i));
                    if (search == null) return search;
                }
            }

            return null;
        }

        public static bool IsScrolledIntoView(Element element)
        {
            var docViewTop = new jQuery(Window.Self).ScrollTop();
            var docViewBottom = docViewTop + new jQuery(Window.Self).Height();

            var elemTop = new jQuery(element).Offset().Top;
            var elemBottom = elemTop + new jQuery(element).Height();

            return ((elemBottom <= docViewBottom) && (elemTop >= docViewTop));
        }

        public static void PopulateList(jQuery childrenArray, int iteration)
        {
            for (var i = 0; i < childrenArray.Length; i++)
            {
                var control = App.Controls[new jQuery(childrenArray.Eq(i)).Data<string>(Constants.Identifier)];

                var addedChild = new jQuery("<div class='listControl'><p>" + control.Type + "</p></div>");
                addedChild.Data(Constants.Identifier, control.Identifier + ListOffset);
                new jQuery("#controlsList").Append(addedChild);

                addedChild.Click((e) =>
                {
                    if (!IsScrolledIntoView(e.CurrentTarget))
                        new jQuery("#siteArea").Stop().Animate(new Dictionary<string, double> { ["scrollTop"] = e.CurrentTarget.GetBoundingClientRect().Top }, 400, "easeInOutCubic");

                    var identifier = new jQuery(e.CurrentTarget).Data<int>(Constants.Identifier) - ListOffset;
                    ContextMenu.OpenMenu(App.Controls[identifier.ToString()].Type, GetElementByIdentifier(identifier).Get(0));
                });

                new jQuery(addedChild).Css("text-indent", 16 + iteration * 32);

                if (control.Type == ControlType.Container && new jQuery(childrenArray.Get(i)).Children().Length > 0)
                    PopulateList(new jQuery(childrenArray.Get(i)).Children(), iteration + 1);
            }
        }

        public static void ProcessFile(Element element)
        {
            var files = (element as HTMLInputElement).Files;

            if (files != null && files.Length != 0)
            {
                var fr = new FileReader()
                {
                    onload = (file) =>
                    {
                        (element as HTMLInputElement).Src = Script.Write<string>("fr.result");
                    }
                };
                fr.readAsDataURL(files[0]);
            }
        }

        public static void SwapElements(this Element from, Element with)
        {
            Node parent1 = null, next1 = null,
                 parent2 = null, next2 = null;

            parent1 = from.ParentNode;
            next1 = from.NextSibling;
            parent2 = with.ParentNode;
            next2 = with.NextSibling;

            parent1.InsertBefore(with, next1);
            parent2.InsertBefore(from, next2);
        }

        #region Snackbar
        static jQuery snackbar = null;

        public static void ShowSnackbar(string text)
        {
            string snackbarElement = "<div style='position: fixed; top: 100%' class='shadow-3' id='snackbar'>" + text + "<button id='snackbarButton' onclick='closeSnackbar()'>Close</button></div>";

            if (snackbar != null)
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

        public static void CloseSnackbar()
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
        #endregion

        #region SaveLoad
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
            loadDialog.Animate(new Dictionary<string, int> { ["opacity"] = 1 }, 250);
        }

        public static void LoadProject(string name)
        {
            ContextMenu.CloseMenu();

            new jQuery("#siteArea").Empty();
            App.Controls = (Dictionary<string, Control>)Window.Self.LocalStorage.GetItem(name + "_controls");
            new jQuery("#siteArea").Append((string)Window.Self.LocalStorage.GetItem(name + "_elements"));
            Control.Id = (int)Window.Self.LocalStorage.GetItem(name + "_id");
            App.Theme = (Theme)Window.Self.LocalStorage.GetItem(name + "_theme");

            foreach (var control in App.Controls)
            {
                var element = GetElementByIdentifier(control.Value.Identifier);
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
            saveDialog.Animate(new Dictionary<string, int> { ["opacity"] = 1 }, 250);
        }

        public static void DialogSave()
        {
            foreach(var control in App.Controls)
            {
                var element = GetElementByIdentifier(control.Value.Identifier);
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
            dialogElement.Animate(new Dictionary<string, int> { ["opacity"] = 0 }, 250, "easeCubicInOut", () =>
            {
                dialogElement.Css("visibility", "hidden");
            });
        }
        #endregion
    }
}
