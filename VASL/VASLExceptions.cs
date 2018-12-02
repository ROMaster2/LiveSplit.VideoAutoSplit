using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LiveSplit.VAS.VASL
{
    public class VASLCompilerException : Exception
    {
        public VASLMethod Method { get; }

        public CompilerErrorCollection CompilerErrors { get; }

        public VASLCompilerException(VASLMethod method, CompilerErrorCollection errors)
            : base(GetMessage(method, errors))
        {
            Method = method;
            CompilerErrors = errors;
        }

        static string GetMessage(VASLMethod method, CompilerErrorCollection errors)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            if (errors == null)
                throw new ArgumentNullException(nameof(errors));

            var sb = new StringBuilder($"'{method.Name ?? "(no name)"}' method compilation errors:");
            foreach (CompilerError error in errors)
            {
                error.Line = error.Line + method.LineOffset;
                sb.Append($"\nLine {error.Line}, Col {error.Column}: {(error.IsWarning ? "warning" : "error")} {error.ErrorNumber}: {error.ErrorText}");
            }
            return sb.ToString();
        }
    }

    public class VASLRuntimeException : Exception
    {
        public VASLRuntimeException(VASLMethod method, Exception innerException)
            : base(GetMessage(method, innerException), innerException)
        { }

        static string GetMessage(VASLMethod method, Exception innerException)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            if (innerException == null)
                throw new ArgumentNullException(nameof(innerException));

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
                        continue;
                }
                else if (frameModule != method.Module)
                    continue;

                var frameLine = frame.GetFileLineNumber();
                if (frameLine > 0)
                {
                    var line = frameLine + frameVASLMethod.LineOffset;
                    stackTraceSb.Append($"\n   at VASL line {line} in '{frameVASLMethod.Name}'");
                }
            }

            var exceptionName = innerException.GetType().FullName;
            var methodName = method.Name ?? "(no name)";
            var exceptionMessage = innerException.Message;
            return $"Exception thrown: '{exceptionName}' in '{methodName}' method:\n{exceptionMessage}\n{stackTraceSb.ToString()}";
        }
    }
}
