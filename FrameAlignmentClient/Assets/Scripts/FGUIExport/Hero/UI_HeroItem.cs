/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Hero
{
    public partial class UI_HeroItem : GButton
    {
        public GGraph m_root;
        public GTextField m_name;
        public const string URL = "ui://5vfukh8qthaz1";

        public static UI_HeroItem CreateInstance()
        {
            return (UI_HeroItem)UIPackage.CreateObject("Hero", "HeroItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_root = (GGraph)GetChildAt(1);
            m_name = (GTextField)GetChildAt(3);
        }
    }
}