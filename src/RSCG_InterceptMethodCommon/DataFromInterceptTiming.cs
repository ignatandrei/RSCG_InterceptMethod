﻿namespace RSCG_InterceptMethodCommon;
public struct DataFromInterceptTiming
{
    public string MethodName="";
    public string ClassName="";
    
    public DateTime StartTime;
    public DateTime EndTime;

    public DataFromInterceptTiming(string methodName,string className)
    {
        StartTime = DateTime.UtcNow;
        EndTime = DateTime.UtcNow;
        MethodName = methodName;
        ClassName = className;
    }
}