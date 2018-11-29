using LiveSplit.Model;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using LiveSplit.VAS.Models;

namespace LiveSplit.VAS.VASL
{
    public class VASLMethod
    {
        public VASLScript.Methods ScriptMethods { get; set; }

        public string Name { get; }

        public bool IsEmpty { get; }

        public int LineOffset { get; }

        public Module Module { get; }

        private dynamic _compiled_code;

        public VASLMethod(string code, string name = null, int script_line = 0)
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
            // Todo: Remove memory and modules?.
            // Refresh rate can stay, kind of, idk. Version is to be added to models eventually.
            using (var provider = new Microsoft.CSharp.CSharpCodeProvider(options))
            {
                var user_code_start_marker = "// USER_CODE_START";
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
    public dynamic Execute(LiveSplitState timer, dynamic old, dynamic current, dynamic vars, dynamic features, dynamic settings)
    {{
        { user_code_start_marker }
	    { code }
	    return null;
    }}
}}";

                if (script_line > 0)
                {
                    var user_code_index = source.IndexOf(user_code_start_marker);
                    var compiled_code_line = source.Take(user_code_index).Count(c => c == '\n') + 2;
                    LineOffset = script_line - compiled_code_line;
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
                _compiled_code = Activator.CreateInstance(type);
            }
        }

        public dynamic Call(LiveSplitState timer, ExpandoObject vars, ref string version,
            dynamic settings, ExpandoObject old = null, ExpandoObject current = null, DeltaManager features = null)
        {
            // dynamic args can't be ref or out, this is a workaround
            _compiled_code.version = version;
            dynamic ret;
            try
            {
                ret = _compiled_code.Execute(timer, old, current, vars, features, settings);
            }
            catch (NullReferenceException ex)
            {
                if (features.OriginalIndex >= DeltaManager.HistorySize)
                    throw new VASLRuntimeException(this, ex);
                else
                    ret = null;
            }
            catch (Exception ex)
            {
                throw new VASLRuntimeException(this, ex);
            }
            version = _compiled_code.version;
            return ret;
        }
    }
}
