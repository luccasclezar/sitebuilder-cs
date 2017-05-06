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
    }
}
