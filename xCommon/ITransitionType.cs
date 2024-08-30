namespace xCollection
{
    public interface ITransitionType
    {
        void onTimer(int iTime, out double dPercentage, out bool bCompleted);
    }
}