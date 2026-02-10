/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace GameLaunch
{
    public partial class UI_GameLaunchPanel : GComponent
    {
        public GTextField m_title;
        public Transition m_t0;
        public const string URL = "ui://p0t1sb4yblfn0";

        public static UI_GameLaunchPanel CreateInstance()
        {
            return (UI_GameLaunchPanel)UIPackage.CreateObject("GameLaunch", "GameLaunchPanel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_title = (GTextField)GetChildAt(1);
            m_t0 = GetTransitionAt(0);
        }
    }
}