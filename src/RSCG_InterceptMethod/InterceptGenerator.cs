using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Immutable;

namespace RSCG_InterceptMethod;
[Generator]
public class InterceptGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
            var methods = context.SyntaxProvider.ForAttributeWithMetadataName(
        "RSCG_InterceptMethodCommon.InterceptMethodAttribute",
        IsAppliedOnMethod,
        FindAttributeDataExposeClass
        )
         .Collect()
        .SelectMany((data, _) => data.Distinct())
        .Collect();
        var methodsToIntercept = context.SyntaxProvider.CreateSyntaxProvider(
                predicate: (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (context, token) =>
                {
                    var operation = context.SemanticModel.GetOperation(context.Node, token);
                    return operation;
                })
            .Where(static m => m is not null)!
            ;
        var compilationAndDataStatic = context.CompilationProvider
            .Combine(methodsToIntercept.Collect())
            .Combine(methods);
        context.RegisterSourceOutput(compilationAndDataStatic,
   (spc, data) =>
   ExecuteGenStatic(spc, data!));


    }

    private void ExecuteGenStatic(SourceProductionContext spc, ((Compilation Left, ImmutableArray<IOperation> Right) Left, ImmutableArray<DataFromExposeMethod> Right) value)
    {
        DataFromExposeMethod[] data = value.Right.ToArray();
        Compilation compilation = value.Left.Left;
        IOperation[] methods = value.Left.Right.ToArray();
        GenerateData(data, methods, compilation, spc);
    }

    private void GenerateData(DataFromExposeMethod[] data, IOperation[] methods, Compilation compilation, SourceProductionContext spc)
    {
        string x = "1";
        foreach (var expose in data)
        {
            switch(expose.HowToIntercept)
            {
                case 0:                    
                    break;
                case 1:
                    DoTiming(expose.type, methods, compilation, spc);
                    break;
                default:
                    throw new NotImplementedException("unknown " + expose.HowToIntercept);
            }
        }
        Console.WriteLine(x);
    }

    private void DoTiming(IMethodSymbol type, IOperation[] methods, Compilation compilation, SourceProductionContext spc)
    {
        var fullName = type.ToDisplayString();
        var className = type.ContainingType.Name;
        //var method = methods.FirstOrDefault(m => m.Syntax.GetLocation().GetLineSpan().StartLinePosition.Line == type.Locations[0].GetLineSpan().StartLinePosition.Line);
        foreach (var item in methods)
        {
            var inv = item as IInvocationOperation;
            if (inv == null)
                continue;
            if (inv.Type is not ITypeSymbol localType)
                continue;

            var inst = inv.Instance;

            //bool IsStaticMethod = true;
            ILocalReferenceOperation? local = null;
            if (inst is ILocalReferenceOperation local1)
            {
                local = local1;
                //IsStaticMethod = false;
            }

            var fullMethod = inv.TargetMethod.ToDisplayString();
            if (fullMethod != fullName)
                continue;
            ////Console.WriteLine("isstatic"+IsStaticMethod);
            //if (!SymbolEqualityComparer.Default.Equals(localType, type.ContainingType))
            //    continue;

            GenerateForInstanceMethods(type, compilation, spc, inv, local);

            
            
            
        }
    }

    private static void GenerateForInstanceMethods(IMethodSymbol type, Compilation compilation, SourceProductionContext spc, IInvocationOperation inv,  ILocalReferenceOperation? local)
    {
        

        var gen = new GenerateDataFromInterceptMethod(compilation, inv, local, type);
        gen.WriteFile(spc);
    }

    private bool IsSyntaxTargetForGeneration(SyntaxNode s)
    {
        if (!TryGetMapMethodName(s, out var method))
            return false;
        return true;

    }

    private static bool IsAppliedOnMethod(
    SyntaxNode syntaxNode,
    CancellationToken cancellationToken)
    {
        var isMethod = syntaxNode is MethodDeclarationSyntax;
        return isMethod;

    }
    private static DataFromExposeMethod FindAttributeDataExposeClass(
    GeneratorAttributeSyntaxContext context,
    CancellationToken cancellationToken)
    {
        var type = (IMethodSymbol)context.TargetSymbol;
        var dataInfo = new DataFromExposeMethod(type);
        return dataInfo;
    }

    public static bool TryGetMapMethodName(SyntaxNode node, out string? methodName)
    {
        methodName = default;
        if (node is not InvocationExpressionSyntax inv)
        {
            return false;
        }
        if (inv is InvocationExpressionSyntax { Expression: IdentifierNameSyntax { Identifier: { ValueText: var methodValue } } })
        {
            methodName = methodValue;
        }
        if (inv is InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax { Name: { Identifier: { ValueText: var method } } } })
        {
            methodName = method;
        }
        if (string.IsNullOrWhiteSpace(methodName))
            return false;
        
        //do not intercept the method that is calling the intercept method
        if (methodName== "InterceptMethod")
            return false;

        return true;
    }

}
