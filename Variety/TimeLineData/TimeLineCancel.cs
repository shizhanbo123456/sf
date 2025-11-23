public class TimeLineCancel
{
    public bool Cancelled { get;private set; }
    private TimeLineWork TimeLineWork;

    public TimeLineCancel(Target targrt)
    {
        TimeLineWork = targrt.TimeLineWork;
        Cancelled = false;
    }
    public void Reset()
    {
        Cancelled = false;
    }
    public void Cancel()
    {
        Cancelled = true;
        TimeLineWork.CancelTrigged();
    }
}