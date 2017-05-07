using Bridge;
using Bridge.Html5;

namespace SiteBuilder
{
    public static class Waves
    {
        public static void Attach(string selector)
        {
            Script.Write("Waves.attach(selector)");
        }

        public static void Attach(string selector, string[] styles)
        {
            Script.Write("Waves.attach(selector, styles)");
        }
    }
}
