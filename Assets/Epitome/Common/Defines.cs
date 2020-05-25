namespace Epitome
{
    public delegate void StateChangedEvent(object sender, ObjectState newState, ObjectState oldState);

    public enum ObjectState
    {
        None,

        Initial,

        Loading,

        Ready,

        Disabled,

        Closing
    }
}