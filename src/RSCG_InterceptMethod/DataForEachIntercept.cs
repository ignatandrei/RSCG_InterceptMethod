using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Text;

namespace RSCG_InterceptMethod;
class DataForEachIntercept
{
    private IInvocationOperation inv;
    private Compilation compilation;
    public string filePath { get; private set; }
    public LinePosition startLinePosition { get; private set; }
    public DataForEachIntercept(IInvocationOperation inv, Compilation compilation)
    {
        this.inv = inv;
        this.compilation = compilation;
        var tree = inv.Syntax.SyntaxTree;
        filePath = compilation.Options.SourceReferenceResolver?.NormalizePath(tree.FilePath, baseFilePath: null) ?? tree.FilePath;
        var location = tree.GetLocation(inv.Syntax.Span);
        var lineSpan = location.GetLineSpan();
        startLinePosition = lineSpan.StartLinePosition;
        SourceText sourceText = location.SourceTree!.GetText();
        var line = sourceText.Lines[startLinePosition.Line];
        this.code = line.ToString();
        this.Path = lineSpan.Path;
        this.Line= startLinePosition.Line + 1;
        //todo
        
    }

    public string CodeNumbered
    {
        get
        {
            int numberCode = 0;
            string codeNumbered = "";
            while (numberCode < code.Length)
            {
                numberCode++;
                var nr1 = numberCode % 10;
                if (nr1 == 0)
                {
                    codeNumbered += "!";
                }
                else
                {
                    codeNumbered += (nr1).ToString();
                }

            }
            return codeNumbered;
        }
    }
    public string code { get; set; } = "";

    public string Path { get; set; } = "";
    public int Line { get; internal set; }
    public int StartMethod { get; internal set; }
    public string DataToBeWriten
    {
        get
        {
            var content = "\r\n";
            content += $@"//replace code: {code}";
            content += "\r\n";
            content += $@"//replace code: {CodeNumbered}";
            content += "\r\n";
            content += $$""" 
[System.Runtime.CompilerServices.InterceptsLocation(@"{{Path}}", {{Line}}, {{StartMethod}})]                
""";
            return content;
        }
    }
}
