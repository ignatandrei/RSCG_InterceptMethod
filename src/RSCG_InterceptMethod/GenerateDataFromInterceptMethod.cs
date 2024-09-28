using Microsoft.CodeAnalysis.Operations;
using System.Drawing;

namespace RSCG_InterceptMethod;
[DebuggerDisplay("{MethodInvocation}")]
internal class GenerateDataFromInterceptMethod
{
    private readonly IInvocationOperation inv;
    private Compilation compilation;
    private ILocalReferenceOperation local;
    private readonly IMethodSymbol methodSymbol;
    public DataForEachIntercept data;
    public string TypeOfClass;
    public string MethodName;
    public string TypeReturn;
    public string NameOfVariable;
    public GenerateDataFromInterceptMethod(Compilation compilation, IInvocationOperation inv, ILocalReferenceOperation local, IMethodSymbol type)
    {
        this.compilation = compilation;
        this.inv = inv;
        this.local = local;
        this.methodSymbol = type;
        
        
        this.NameOfVariable = local.Local.Name;
        this.MethodName = inv.TargetMethod.Name;
        INamedTypeSymbol namedTypeSymbol = methodSymbol.ContainingType;

        this.TypeOfClass = namedTypeSymbol.ToDisplayString();
        this.MethodInvocation = inv.Syntax.ToString();
        this.TypeReturn = inv.TargetMethod.ReturnType.ToDisplayString();
        //Console.WriteLine("MethodInvocation: " + MethodInvocation);
        data = new DataForEachIntercept(inv, compilation);
        //int startMethod;

        if (NameOfVariable.Length > 0)
        {
            data.StartMethod = data.code.IndexOf(NameOfVariable + "." + MethodName + "(", data.startLinePosition.Character) + 1;//dot
            data.StartMethod += (NameOfVariable + ".").Length;
        }
        else
            data.StartMethod = data.code.IndexOf(MethodName + "(", data.startLinePosition.Character);        
        
    }
    public bool IsVoid()
    {
        return TypeReturn == "void";
    }
    public string ThisArgument
    {
        get
        {
            string ret= $"this {this.TypeOfClass} {NameOfVariable}";
            if (ArgumentsWithWhichIsCalled.Length > 0)
                ret += ",";

            return ret;
        }
        
    }
    public string ArgumentsForCallMethod
    {
        get
        {
            var arguments = methodSymbol.Parameters;
            if (arguments.Length == 0) return "";
            //if is instance, has a this before
            string argumentsForCallMethod = "";
            foreach (var item in arguments)
            {
                argumentsForCallMethod +=item.Type.Name +" "+  item.Name.ToString() + ", ";
            }
            argumentsForCallMethod = argumentsForCallMethod.Substring(0, argumentsForCallMethod.Length - 2);
            return argumentsForCallMethod;
        }
    }
    private string ArgumentsWithWhichIsCalled
    {
        get
        {
            //var arguments = inv.Arguments;
            var arguments = methodSymbol.Parameters;
            if (arguments.Length == 0) return "";
            //if is instance, has a this before
            string argumentsForCallMethod = "";
            foreach (var item in arguments)
            {
                argumentsForCallMethod +=item.Name.ToString() + ", ";
            }
            argumentsForCallMethod = argumentsForCallMethod.Substring(0, argumentsForCallMethod.Length - 2);
            return argumentsForCallMethod;
        }
    }
    public string CallMethod { 
        get{
            var method = local.Syntax.ToString();
            method += ".";
            method += this.MethodName;
            method += "(";
            method += this.ArgumentsWithWhichIsCalled;
            method += ")";
            return method;
        }
    }
    public string MethodInvocation { get; set; }
    static int count = 0;
    internal void WriteFile(SourceProductionContext spc)
    {
        InterceptTimingInstance genInstance = new(this);
        var data = genInstance.Render();
        Interlocked.Increment(ref count);
        string nameFileToBeWritten = $"{this.TypeOfClass}_{this.MethodName}_{count}";

        spc.AddSource($"{nameFileToBeWritten}.cs", data);
    }
    public string MethodSignature
    {
        get
        {
            var nameOfVariable = this.NameOfVariable.Replace(".", "_");
            return $"Intercept_{nameOfVariable}_{MethodName}_{count}";
        }
    }
    public bool HasTaskReturnType
    {
        get
        {
            return TypeReturn.Contains("System.Threading.Tasks.Task");
        }
    }
}
