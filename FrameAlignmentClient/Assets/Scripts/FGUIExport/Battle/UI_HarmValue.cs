/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Battle
{
    public partial class UI_HarmValue : GComponent
    {
        public GTextField m_value;
        public Transition m_t0;
        public const string URL = "ui://0fi67j7jhiwh3";

        public static UI_HarmValue CreateInstance()
        {
            return (UI_HarmValue)UIPackage.CreateObject("Battle", "HarmValue");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_value = (GTextField)GetChildAt(0);
            m_t0 = GetTransitionAt(0);
        }
    }
}