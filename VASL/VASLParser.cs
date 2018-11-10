using Irony.Parsing;
using LiveSplit.ComponentUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveSplit.VAS.Models;

namespace LiveSplit.VAS.VASL
{
    public class VASLParser
    {
        public static VASLScript Parse(string code)
        {
            var grammar = new VASLGrammar();
            var parser = new Parser(grammar);
            var tree = parser.Parse(code);

            if (tree.HasErrors())
            {
                var error_msg = new StringBuilder("VASL parse error(s):");
                foreach (var msg in parser.Context.CurrentParseTree.ParserMessages)
                {
                    var loc = msg.Location;
                    error_msg.Append($"\nat Line {loc.Line + 1}, Col {loc.Column + 1}: {msg.Message}");
                }

                throw new Exception(error_msg.ToString());
            }

            var methods_node = tree.Root.ChildNodes.First(x => x.Term.Name == "methodList");

            // Todo: Aliasing

            var methods = new VASLScript.Methods();

            foreach (var method in methods_node.ChildNodes[0].ChildNodes)
            {
                var body = (string)method.ChildNodes[2].Token.Value;
                var method_name = (string)method.ChildNodes[0].Token.Value;
                var line = method.ChildNodes[2].Token.Location.Line + 1;
                var script = new VASLMethod(body, method_name, line)
                {
                    ScriptMethods = methods
                };
                switch (method_name)
                {
                    case "init":      methods.init      = script; break;
                    case "exit":      methods.exit      = script; break;
                    case "update":    methods.update    = script; break;
                    case "start":     methods.start     = script; break;
                    case "split":     methods.split     = script; break;
                    case "isLoading": methods.isLoading = script; break;
                    case "gameTime":  methods.gameTime  = script; break;
                    case "reset":     methods.reset     = script; break;
                    case "startup":   methods.startup   = script; break;
                    case "shutdown":  methods.shutdown  = script; break;
                }
            }

            return new VASLScript(methods);
        }
    }
}
