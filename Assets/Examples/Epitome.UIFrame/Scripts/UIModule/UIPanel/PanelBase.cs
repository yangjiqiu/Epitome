using Epitome.UIFrame;

public abstract class PanelBase : UIBase
{
    public override void Display()
    {
        gameObject.SetActive(true);

        base.Display();
    }

    public override void Freeze()
    {
        base.Freeze();
    }

    public override void Redisplay()
    {
        base.Redisplay();
    }

    public override void Hiding()
    {
        gameObject.SetActive(false);

        base.Hiding();
    }
}