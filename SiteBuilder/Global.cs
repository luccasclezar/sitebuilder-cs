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
            if (element == null) element = Document.GetElementById("siteArea");
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

        public static string LowerFirst(this string str)
        {
            str = str.CharAt(0).ToLowerCase() + str.Substr(1);
            return str;
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

        public static string UpperFirst(this string str)
        {
            str = str.CharAt(0).ToUpperCase() + str.Substr(1);
            return str;
        }
    }
}
