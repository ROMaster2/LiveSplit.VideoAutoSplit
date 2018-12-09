using System.Linq;
using Irony.Parsing;

namespace LiveSplit.VAS.VASL
{
    [Language("VASL", "0.1", "Video Auto Split Language grammar")]
    public class VASLGrammar : Grammar
    {
        public VASLGrammar() : base(true)
        {
            var code       = new CustomTerminal("code", MatchCodeTerminal);
            var stringLit = TerminalFactory.CreateCSharpString("string");
            var identifier = TerminalFactory.CreateCSharpIdentifier("identifier");
            var number     = TerminalFactory.CreateCSharpNumber("number");
            number.Options |= NumberOptions.AllowSign;

            var singleLineComment = new CommentTerminal("SingleLineComment", "//", "\r", "\n", "\u2085", "\u2028", "\u2029");
            var delimitedComment  = new CommentTerminal("DelimitedComment", "/*", "*/");
            NonGrammarTerminals.Add(singleLineComment);
            NonGrammarTerminals.Add(delimitedComment);

            // Todo: Aliases

            var init      = new KeyTerm("init",      "init");
            var exit      = new KeyTerm("exit",      "exit");
            var update    = new KeyTerm("update",    "update");
            var start     = new KeyTerm("start",     "start");
            var split     = new KeyTerm("split",     "split");
            var reset     = new KeyTerm("reset",     "reset");
            var startup   = new KeyTerm("startup",   "startup");
            var shutdown  = new KeyTerm("shutdown",  "shutdown");
            var undoSplit = new KeyTerm("undoSplit", "undoSplit");
            var isLoading = new KeyTerm("isLoading", "isLoading");
            var gameTime  = new KeyTerm("gameTime",  "gameTime");
            var comma     = ToTerm(",", "comma");
            var semi      = ToTerm(";", "semi");

            var root       = new NonTerminal("root");
            var version    = new NonTerminal("version");
            var methodList = new NonTerminal("methodList");
            var varList    = new NonTerminal("varList");
            var var        = new NonTerminal("var");
            var method     = new NonTerminal("method");
            var methodType = new NonTerminal("methodType");

            root.Rule        = methodList;
            version.Rule     = (comma + stringLit) | Empty;
            methodList.Rule = MakeStarRule(methodList, method);
            varList.Rule    = MakeStarRule(varList, semi, var);
            method.Rule      = (methodType + "{" + code + "}") | Empty;
            methodType.Rule = init | exit | update | start | split | isLoading | gameTime | reset | startup | shutdown | undoSplit;

            Root = root;

            MarkTransient(varList, methodList, methodType);

            LanguageFlags = LanguageFlags.NewLineBeforeEOF;
        }

        private static Token MatchCodeTerminal(Terminal terminal, ParsingContext context, ISourceStream source)
        {
            var remaining = source.Text.Substring(source.Location.Position);
            var stack = 1;
            var token = "";

            while (stack > 0)
            {
                var index = remaining.IndexOf('}') + 1;
                var cut = remaining.Substring(0, index);

                token += cut;
                remaining = remaining.Substring(index);
                stack += cut.Count(x => x == '{');
                stack--;
            }

            token = token.Substring(0, token.Length - 1);
            source.PreviewPosition += token.Length;

            return source.CreateToken(terminal.OutputTerminal);
        }
    }
}
