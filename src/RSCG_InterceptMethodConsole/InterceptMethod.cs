namespace RSCG_InterceptMethodCommon;
internal class InterceptMethodCall
{
    public static void InterceptMethod(DataFromInterceptTiming dataFromInterceptTiming)
    {
        Console.WriteLine("method called: " + dataFromInterceptTiming.ClassName+"."+dataFromInterceptTiming.MethodName );
        Console.WriteLine("start time: " + dataFromInterceptTiming.StartTime);
        Console.WriteLine("end time: " + dataFromInterceptTiming.EndTime);
    }
}
