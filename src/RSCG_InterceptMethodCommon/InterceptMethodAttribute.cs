namespace RSCG_InterceptMethodCommon;
public enum WhatToIntercept
{
    None,
    Timing,
}

[AttributeUsage(AttributeTargets.Method,AllowMultiple =false)]
public class InterceptMethodAttribute : Attribute
{
    private readonly WhatToIntercept whatToIntercept;

    public InterceptMethodAttribute(WhatToIntercept whatToIntercept)
    {
        this.whatToIntercept = whatToIntercept;
    }
}
