
namespace RSCG_InterceptMethod;

internal class DataFromExposeMethod
{
    public IMethodSymbol type;
    public int HowToIntercept;
    public DataFromExposeMethod(IMethodSymbol type)
    {
        this.type = type;
        var attr = type.GetAttributes();
        foreach (var item in attr)
        {
            if (item.AttributeClass?.ToDisplayString() == "RSCG_InterceptMethodCommon.InterceptMethodAttribute")
            {
                HowToIntercept =int.Parse( item.ConstructorArguments[0].Value?.ToString() ?? "");

            }
        }
    }
}