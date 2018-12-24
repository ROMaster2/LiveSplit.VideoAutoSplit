using LiveSplit.Model;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using LiveSplit.VAS.Models.Delta;

namespace LiveSplit.VAS.VASL
{
    public class VASLMethod
    {
        public VASLScript.MethodList ScriptMethods { get; set; }

        public string Name { get; }

        public bool IsEmpty { get; }

        public int LineOffset { get; }

        public Module Module { get; }

        private dynamic CompiledCode;

        public VASLMethod(string code, string name = null, int scriptLine = 0)
        {
            if (code == null)
                throw new ArgumentNullException(nameof(code));

            Name = name;
            IsEmpty = string.IsNullOrWhiteSpace(code);
            code = code.Replace("return;", "return null;"); // hack
            code = Regex.Replace(code, @"\.old[^\w\(]", ".old()", RegexOptions.IgnoreCase); // Testing

            var options = new Dictionary<string, string> {
                { "CompilerVersion", "v4.0" }
            };

            using (var provider = new Microsoft.CSharp.CSharpCodeProvider(options))
            {
                var userCodeStartMarker = "// USER_CODE_START";
                string source = $@"
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using LiveSplit.ComponentUtil;
using LiveSplit.Model;
using LiveSplit.Options;
public class CompiledScript
{{
    public string version;
    public double refreshRate;
    void print(string s)
    {{
        Log.Info(s);
    }}
    public dynamic Execute(LiveSplitState timer, dynamic vars, dynamic features, dynamic settings)
    {{
        { userCodeStartMarker }
	    { code }
	    return null;
    }}
}}";

                if (scriptLine > 0)
                {
                    var userCodeIndex = source.IndexOf(userCodeStartMarker);
                    var compiledCodeLine = source.Take(userCodeIndex).Count(c => c == '\n') + 2;
                    LineOffset = scriptLine - compiledCodeLine;
                }

                var parameters = new CompilerParameters() {
                    GenerateInMemory = true,
                    CompilerOptions = "/optimize /d:TRACE /debug:pdbonly",
                };
                parameters.ReferencedAssemblies.Add("System.dll");
                parameters.ReferencedAssemblies.Add("System.Core.dll");
                parameters.ReferencedAssemblies.Add("System.Data.dll");
                parameters.ReferencedAssemblies.Add("System.Data.DataSetExtensions.dll");
                parameters.ReferencedAssemblies.Add("System.Drawing.dll");
                parameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");
                parameters.ReferencedAssemblies.Add("System.Xml.dll");
                parameters.ReferencedAssemblies.Add("System.Xml.Linq.dll");
                parameters.ReferencedAssemblies.Add("Microsoft.CSharp.dll");
                parameters.ReferencedAssemblies.Add("LiveSplit.Core.dll");

                var res = provider.CompileAssemblyFromSource(parameters, source);
                if (res.Errors.HasErrors)
                    throw new VASLCompilerException(this, res.Errors);

                Module = res.CompiledAssembly.ManifestModule;
                var type = res.CompiledAssembly.GetType("CompiledScript");
                CompiledCode = Activator.CreateInstance(type);
            }
        }

        public dynamic Call(LiveSplitState timer, ExpandoObject vars, string gameVersion, dynamic settings, DeltaOutput d)
        {
            dynamic ret = null;
            try
            {
                ret = CompiledCode.Execute(timer, vars, d, settings);
            }
            catch (Exception ex)
            {
                // Ignore NullReferenceExceptions until the history is filled.
                if (ex is NullReferenceException && d.OriginalIndex <= d.HistorySize)
                {
                    Log.Verbose("NullReferenceException found in script before history is filled, ignoring.");
                }
                else
                {
                    Log.Error(new VASLRuntimeException(this, ex), "VASL Script Error");
                }
            }
            return ret;
        }
    }
}
