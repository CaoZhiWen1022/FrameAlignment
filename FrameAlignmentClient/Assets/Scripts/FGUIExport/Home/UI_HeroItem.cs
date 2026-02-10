/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Home
{
    public partial class UI_HeroItem : GButton
    {
        public GGraph m_root;
        public const string URL = "ui://hmul0fbzthaz1";

        public static UI_HeroItem CreateInstance()
        {
            return (UI_HeroItem)UIPackage.CreateObject("Home", "HeroItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_root = (GGraph)GetChildAt(0);
        }
    }
}