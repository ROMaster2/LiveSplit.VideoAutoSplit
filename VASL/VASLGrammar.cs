using System.Linq;
using Irony.Parsing;

namespace LiveSplit.VAS.VASL
{
    [Language("VASL", "0.1", "Video Auto Split Language grammar")]
    public class VASLGrammar : Grammar
    {
        public VASLGrammar()
            : base(true)
        {
            var code       = new CustomTerminal("code", MatchCodeTerminal);
            var string_lit = TerminalFactory.CreateCSharpString("string");
            var identifier = TerminalFactory.CreateCSharpIdentifier("identifier");
            var number     = TerminalFactory.CreateCSharpNumber("number");
            number.Options |= NumberOptions.AllowSign;

            var single_line_comment = new CommentTerminal("SingleLineComment", "//", "\r", "\n", "\u2085", "\u2028", "\u2029");
            var delimited_comment   = new CommentTerminal("DelimitedComment", "/*", "*/");
            NonGrammarTerminals.Add(single_line_comment);
            NonGrammarTerminals.Add(delimited_comment);

            // Todo: Aliases

            var init      = new KeyTerm("init",      "init");
            var exit      = new KeyTerm("exit",      "exit");
            var update    = new KeyTerm("update",    "update");
            var start     = new KeyTerm("start",     "start");
            var split     = new KeyTerm("split",     "split");
            var reset     = new KeyTerm("reset",     "reset");
            var startup   = new KeyTerm("startup",   "startup");
            var shutdown  = new KeyTerm("shutdown",  "shutdown");
            var isLoading = new KeyTerm("isLoading", "isLoading");
            var gameTime  = new KeyTerm("gameTime",  "gameTime");
            var comma     = ToTerm(",", "comma");
            var semi      = ToTerm(";", "semi");

            var root        = new NonTerminal("root");
            var version     = new NonTerminal("version");
            var method_list = new NonTerminal("methodList");
            var var_list    = new NonTerminal("varList");
            var var         = new NonTerminal("var");
            var method      = new NonTerminal("method");
            var method_type = new NonTerminal("methodType");

            root.Rule        = method_list;
            version.Rule     = (comma + string_lit) | Empty;
            method_list.Rule = MakeStarRule(method_list, method);
            var_list.Rule    = MakeStarRule(var_list, semi, var);
            method.Rule      = (method_type + "{" + code + "}") | Empty;
            method_type.Rule = init | exit | update | start | split | isLoading | gameTime | reset | startup | shutdown;

            Root = root;

            MarkTransient(var_list, method_list, method_type);

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
