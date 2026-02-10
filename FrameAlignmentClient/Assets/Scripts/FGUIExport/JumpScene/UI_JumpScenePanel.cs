/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace JumpScene
{
    public partial class UI_JumpScenePanel : GComponent
    {
        public GGraph m_bg;
        public const string URL = "ui://p0xuzafwblfn0";

        public static UI_JumpScenePanel CreateInstance()
        {
            return (UI_JumpScenePanel)UIPackage.CreateObject("JumpScene", "JumpScenePanel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_bg = (GGraph)GetChildAt(0);
        }
    }
}