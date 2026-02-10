/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Load
{
    public partial class UI_LoadPanel : GComponent
    {
        public GProgressBar m_bar;
        public const string URL = "ui://hletxy2kq7gw0";

        public static UI_LoadPanel CreateInstance()
        {
            return (UI_LoadPanel)UIPackage.CreateObject("Load", "LoadPanel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_bar = (GProgressBar)GetChildAt(2);
        }
    }
}