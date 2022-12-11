
namespace MipsEmu.Assembler;

using System.Text;

public struct Label {
    private string Name {get;}
    private int Line {get;}

    public Label(string name, int line) {
        Name = name;
        Line = line;
    }

    public override string ToString() {
        return Name + ":" + Line;
    }
}

public struct ParseResult {
    public List<Token> directiveTokens, instructionTokens;
    public List<Label> directiveLabels, instructionLabels;
    public List<string> globals;

    public ParseResult() {
        directiveTokens = new List<Token>();
        instructionTokens = new List<Token>();
        directiveLabels = new List<Label>();
        instructionLabels = new List<Label>();
        globals = new List<string>();
    }

    public override string ToString() {
        var builder = new StringBuilder();
        builder.AppendLine("Instruction Tokens:");
        foreach(var token in instructionTokens)
            builder.AppendLine("\t" + token);
        builder.AppendLine("\nDirective Tokens:");
        foreach(var token in directiveTokens)
            builder.AppendLine("\t" + token);

        builder.AppendLine("\nInstruction Labels:");
        foreach(var token in instructionLabels)
            builder.AppendLine("\t" + token);
        builder.AppendLine("\nDirective Labels:");
        foreach(var token in directiveLabels)
            builder.AppendLine("\t" + token);

        builder.AppendLine("\nGlobal Labels:");
        foreach(var label in globals)
            builder.AppendLine("\t" + label);
        return builder.ToString();
    }

}

public class ParseTreeNode {
    public ParseTreeNode? Parent {get;}
    public Token? Data {get;}
    public List<ParseTreeNode> Children {get;}

    public ParseTreeNode(Token? value, ParseTreeNode? parent) {
        Data = value;
        Children = new List<ParseTreeNode>();
        Parent = parent;
    }

    public ParseTreeNode(Token? value) : this(value, null) { }

    public ParseTreeNode AddChild(Token childValue) {
        var child = new ParseTreeNode(childValue, this);
        Children.Add(child);
        return child;
    }
}
public class SyntaxAnalyzer {
    private TokenFactory factory;

    public SyntaxAnalyzer() {
        factory = new TokenFactory();
    }

    public ParseResult BuildProgram(Symbol[] symbols) {
        ParseTreeNode root = BuildTree(symbols);
        List<Token> tokens = FindSolution(root);
        return SeparateTokens(tokens);
    } 

    public ParseTreeNode BuildTree(Symbol[] symbols) {
        ParseTreeNode root = new ParseTreeNode(null);
        BuildTree(root, symbols, 0);
        return root;
    }

    private void BuildTree(ParseTreeNode parent, Symbol[] symbols, int startIndex) {
        if (startIndex < symbols.Length) {
            var result = new List<Token>();
            var matches = factory.FindMatches(symbols, startIndex);
            foreach (var match in matches) {
                Token token = factory.Generate(symbols, match, startIndex);
                var child = parent.AddChild(token);
                BuildTree(child, symbols, startIndex + match.Item2);
            }
        }
    }

    public List<Token> FindSolution(ParseTreeNode root) {
        var nodes = new Stack<Tuple<int, ParseTreeNode>>();
        var max = new Tuple<int, ParseTreeNode>(0, root);
        nodes.Push(max);
        while (nodes.Count > 0) {
            var current = nodes.Pop();
            if (current.Item2.Children.Count > 0) {
                int depth = current.Item1 + 1;
                current.Item2.Children.ForEach((n) => nodes.Push(new Tuple<int, ParseTreeNode>(depth, n)));
            } else if (current.Item1 > max.Item1) { // no deeper
                max = current;
            }
        }
        return BuildLeafSolution(max.Item2);
    }

    private List<Token> BuildLeafSolution(ParseTreeNode leaf) {
        var solution = new LinkedList<Token>();
        for (ParseTreeNode? current = leaf; current != null && current.Data != null; current = current.Parent) {
            solution.AddLast(current.Data);
        }
        return solution.ToList<Token>();
    }


    /// <summary>Split tokens into Data and Text. Create symbol table.
    public ParseResult SeparateTokens(List<Token> tokens) {
        var result = new ParseResult();
        bool inText = true;
        for(int t = 0; t < tokens.Count; t++) {
            Token token = tokens[t];
            switch(token.GetTokenType()) {
                case TokenType.INSTRUCTION:
                    ProcessInstructionToken(token, result);
                    break;
                case TokenType.DIRECTIVE:
                    ProcessDirectiveToken(token, ref inText, result);
                    break;
                case TokenType.LABEL:
                    CreateLabel(result, inText, token);
                    break;
            }
        }
        return result;
    }

    public void CreateLabel(ParseResult result, bool inText, Token token) {
        string labelName = token.GetSymbol(0, true).value;
        if (inText) {
            result.instructionLabels.Add(new Label(labelName, result.instructionLabels.Count));
        } else {
            result.directiveLabels.Add(new Label(labelName, result.directiveTokens.Count));
        }
    }

    public void ProcessInstructionToken(Token token, ParseResult result) {
        result.instructionTokens.Add(token);
    }

    public void ProcessDirectiveToken(Token token, ref bool inText, ParseResult result) {
        result.directiveTokens.Add(token);
        token.UpdateAssemblerState(ref inText, result); // add globls and change data/text
    }

    private void PrintRemaining(Symbol[] symbols, int start) {
        var builder = new StringBuilder();
        for (int i = start; i < symbols.Length; i++)
            builder.Append(symbols[i] + " ~~ ");
        Console.WriteLine(builder.ToString());
    }

    public void AddTokenForm(ITokenForm form, Func<Symbol[], Token> generator) {
        factory.AddTokenForm(form, generator);
    }

}

