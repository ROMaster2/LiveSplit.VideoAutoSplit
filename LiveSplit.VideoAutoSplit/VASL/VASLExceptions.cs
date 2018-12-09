using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LiveSplit.VAS.VASL
{
    // @TODO: Implement Exception Constructors
    // @TODO: Don't throw exceptions within exceptions
    public class VASLCompilerException : Exception
    {
        public VASLMethod Method { get; private set; }
        public CompilerErrorCollection CompilerErrors { get; private set; }

        public VASLCompilerException(VASLMethod method, CompilerErrorCollection errors)
            : base(GetMessage(method, errors))
        {
            Method = method;
            CompilerErrors = errors;
        }

        private static string GetMessage(VASLMethod method, CompilerErrorCollection errors)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            if (errors == null) throw new ArgumentNullException(nameof(errors));

            var sb = new StringBuilder($"'{method.Name ?? "(no name)"}' method compilation errors:");
            foreach (CompilerError error in errors)
            {
                error.Line += method.LineOffset;
                sb.Append("\nLine ").Append(error.Line).Append(", Col ").Append(error.Column).Append(": ").Append(error.IsWarning ? "warning" : "error").Append(" ").Append(error.ErrorNumber).Append(": ").Append(error.ErrorText);
            }
            return sb.ToString();
        }
    }

    // @TODO: Implement Exception Constructors
    // @TODO: Don't throw exceptions within exceptions
    public class VASLRuntimeException : Exception
    {
        public VASLRuntimeException(VASLMethod method, Exception innerException)
            : base(GetMessage(method, innerException), innerException)
        { }

        private static string GetMessage(VASLMethod method, Exception innerException)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            if (innerException == null) throw new ArgumentNullException(nameof(innerException));

            var stackTrace = new StackTrace(innerException, true);
            var stackTraceSb = new StringBuilder();
            foreach (var frame in stackTrace.GetFrames())
            {
                var frameMethod = frame.GetMethod();
                var frameModule = frameMethod.Module;

                var frameVASLMethod = method;
                if (method.ScriptMethods != null)
                {
                    frameVASLMethod = method.ScriptMethods.FirstOrDefault(m => frameModule == m.Module);
                    if (frameVASLMethod == null)
                    {
                        continue;
                    }
                }
                else if (frameModule != method.Module)
                {
                    continue;
                }

                var frameLine = frame.GetFileLineNumber();
                if (frameLine > 0)
                {
                    var line = frameLine + frameVASLMethod.LineOffset;
                    stackTraceSb.Append("\n   at VASL line ").Append(line).Append(" in '").Append(frameVASLMethod.Name).Append("'");
                }
            }

            var exceptionName = innerException.GetType().FullName;
            var methodName = method.Name ?? "(no name)";
            var exceptionMessage = innerException.Message;
            return $"Exception thrown: '{exceptionName}' in '{methodName}' method:\n{exceptionMessage}\n{stackTraceSb}";
        }
    }
}
