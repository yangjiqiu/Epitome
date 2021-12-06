namespace Epitome.UIFrame
{
    public abstract class PopUpPanelBase : PanelBase
    {
        protected UIMaskType UIMaskType = UIMaskType.Lucency;

        public override void Display()
        {
            UIMaskManager.Instance.SetMaskWindow(this.gameObject, UIMaskType);
            base.Display();
        }

        public override void Freeze()
        {
            base.Freeze();
        }

        public override void Redisplay()
        {
            UIMaskManager.Instance.SetMaskWindow(this.gameObject, UIMaskType);
            base.Redisplay();
        }

        public override void Hiding()
        {
            UIMaskManager.Instance.CancelMaskWindow();
            base.Hiding();
        }
    }
}