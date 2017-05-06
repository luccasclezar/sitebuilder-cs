using Bridge;
using Bridge.Html5;
using System;

namespace SiteBuilder
{
    ///<summary>
    ///Stub for the js FileReader
    ///</summary>
    [External]
    public class FileReader
    {
        public Action<Event<HTMLInputElement>> onload;
        public Action<Event<HTMLInputElement>> onloadend;

        public extern void readAsDataURL(File f);
        public extern void readAsBinaryString(Blob blob);
        public extern void readAsArrayBuffer(Blob blob);
        public extern void readAsText(Blob blob);
    };
}
