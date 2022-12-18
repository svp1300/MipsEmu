namespace MipsEmu.Assembler;

using MipsEmu.Assembler.Tokens;

using System.Diagnostics.CodeAnalysis;
using System.Text;

/// <summary>Marker for an assembly label.</summary>
public struct Label {
    public string Name {get;}
    public long Address {get; set;}

    public Label(string name, long address) {
        Name = name;
        Address = address;
    }

    public override string ToString() {
        return Name + ":" + Address;
    }

    public override bool Equals([NotNullWhen(true)] object? obj) {
        if (obj is Label) {
            return ((Label) obj).Name.Equals(Name);
        } else {
            return false;
        }
    }

    public override int GetHashCode() => Name.GetHashCode();
    
    public static Label? FindLabel(string name, List<Label> labels) {
        foreach(var l in labels) {
            if (l.Name.Equals(name))
                return l;
        }
        return null;
    }

    public void AddAddressOffset(long amount) {
        Address += amount;
    }

}

/// <summary>Contains the global names and both tokens and labels organized by whether they are for instructions or dot directives.</summary>
public class SyntaxParseResult {
    
    public List<Token> directiveTokens, instructionTokens;
    public List<Label> directiveLabels, instructionLabels;
    public List<string> globals;
    public long dataLength, textLength;

    public SyntaxParseResult() {
        directiveTokens = new List<Token>();
        instructionTokens = new List<Token>();
        directiveLabels = new List<Label>();
        instructionLabels = new List<Label>();
        globals = new List<string>();
        dataLength = -1;
        textLength = -1;
    }

    public Label? GetLabel(string name, bool text) {
        var searchedLabels = text ? instructionLabels : directiveLabels;
        foreach (var label in searchedLabels) {
            if (label.Name.Equals(name))
                return label;
        }
        return null;
    }
    
    public List<Label> GetExternalDataReferences() => GetExternalReferences(directiveLabels);
    public List<Label> GetExternalTextReferences() => GetExternalReferences(instructionLabels);

    private List<Label> GetExternalReferences(List<Label> labels) {
        var external = new List<Label>();
        foreach(var labelName in globals) {
            Label? matchedLabel = Label.FindLabel(labelName, labels);
            if (matchedLabel != null)
                external.Add(matchedLabel.Value);
        }
        return external;
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

public class AnalyzerState {
    public long DataAddress {get; set;}
    public long TextAddress {get; set;}
    public int Alignment {get; set;}
    public bool InText {get; set;}

    public AnalyzerState() {
        DataAddress = 0;
        TextAddress = 0;
        Alignment = 0;
        InText = true;
    }
}

/// <summary>Data structure used in parsing when finding and presenting the solution.</summary>
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
/// <summary>Tool that when given the language definitions of assembly code and a program, will tokenize it and split it into directives, instructions, and labels.</summary>
public class SyntaxAnalyzer {
    private TokenFactory factory;

    public SyntaxAnalyzer() {
        factory = new TokenFactory();
    }

    /// <summary>Runs the process of syntax analysis from the parse tree to token meaning.</summary>
    /// <param name="symbols">The symbols found during a program's lexical analysis.</param>
    /// <returns>A struct containing the results of the analysis.</returns>
    public SyntaxParseResult BuildProgram(Symbol[] symbols) {
        ParseTreeNode root = BuildTree(symbols);
        List<Token> tokens = FindSolution(root);
        return SeparateTokens(tokens);
    } 

    /// <summary>Takes the given symbols and creates a parse tree for use in analyzing the program.</summary>
    /// <param name="symbols">The symbols found during a program's lexical analysis.</param>
    /// <returns>The parse tree created by analyzing symbols.</returns>
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

    /// <summary>Finds and returns the path from the root to the deepest leaf in the parse tree.</summary>
    /// <param name="root">Root of the program's parse tree.</param>
    /// <returns>Longest path in the parse tree.</returns>
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
            } else if (current.Item1 == max.Item1) {
                var maxData = max.Item2.Data;
                var currentData = max.Item2.Data;
                if (maxData != null && currentData != null && maxData.GetSymbolCount(true) < currentData.GetSymbolCount(true)) {
                    max =  current;
                }
                        
            }
        }
        return BuildLeafSolution(max.Item2);
    }

    private List<Token> BuildLeafSolution(ParseTreeNode leaf) {
        var solution = new LinkedList<Token>();
        for (ParseTreeNode? current = leaf; current != null && current.Data != null; current = current.Parent) {
            solution.AddFirst(current.Data);
        }
        return solution.ToList<Token>();
    }


    /// <summary>Split tokens into Data, text, and labels. Applies effects of some dot directives.</summary>
    /// <param name="tokens">A list of tokens from the parse tree's solution.</param>
    /// <returns>A struct containing the results of the analysis.<returns>
    public SyntaxParseResult SeparateTokens(List<Token> tokens) {
        var result = new SyntaxParseResult();
        var state = new AnalyzerState();

        for(int t = 0; t < tokens.Count; t++) {
            Token token = tokens[t];
            Console.WriteLine(token);
            switch(token.GetTokenType()) {
                case TokenType.INSTRUCTION:
                    ProcessInstructionToken(token, state, result);
                    break;
                case TokenType.DIRECTIVE:
                    ProcessDirectiveToken(token, state, result);
                    break;
                case TokenType.LABEL:
                    CreateLabel(result, state, token);
                    break;
            }
        }
        result.dataLength = state.DataAddress;
        result.textLength = state.TextAddress;
        return result;
    }

    public void CreateLabel(SyntaxParseResult result, AnalyzerState state, Token token) {
        string labelName = token.GetSymbol(0, true).value;
        if (state.InText) {
            result.instructionLabels.Add(new Label(labelName, state.TextAddress));
        } else {
            result.directiveLabels.Add(new Label(labelName, state.DataAddress));
        }
    }

    public void ProcessInstructionToken(Token token, AnalyzerState state, SyntaxParseResult result) {
        result.instructionTokens.Add(token);
        state.TextAddress += 32;
    }

    public void ProcessDirectiveToken(Token token, AnalyzerState state, SyntaxParseResult result) {
        result.directiveTokens.Add(token);
        token.UpdateAssemblerState(state, result); // add globls and change data/text
        state.DataAddress += token.GetBitLength(state.Alignment);
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


