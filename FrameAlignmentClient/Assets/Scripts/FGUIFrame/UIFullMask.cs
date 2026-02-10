namespace FGUIFrame
{
    /// <summary>
    /// 全屏遮罩基类
    /// </summary>
    public class UIFullMask : UIPanel
    {
        public int visibleCount = 0;

        public void SetMaskVisible(bool visible)
        {
            if (visible)
            {
                this.visibleCount++;
            }
            else
            {
                this.visibleCount--;
            }
            if (this.visibleCount > 0)
            {
                this.SetShow();
            }
            else
            {
                this.SetHide();
            }
        }

        protected void SetShow()
        {
            if (this.m_ui.visible == true) return;
            this.m_ui.visible = true;
        }

        protected void SetHide()
        {
            if (this.m_ui.visible == false) return;
            this.m_ui.visible = false;
        }
    }
}

