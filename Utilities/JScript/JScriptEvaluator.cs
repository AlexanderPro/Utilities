using System;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.JScript;

namespace Utilities.JScript
{
    public static class JScriptEvaluator
    {
        private static Object _evaluator = null;
        private static Type _evaluatorType = null;
        private static readonly String _jscriptSource =

            @"package Evaluator
            {
               class Evaluator
               {
                  public function Eval(expr : String) : Object
                  { 
                     return eval(expr); 
                  }
               }
            }";

        static JScriptEvaluator()
        {
            var provider = new JScriptCodeProvider();
            var parameters = new CompilerParameters()
            {
                GenerateInMemory = true
            };

            CompilerResults results = provider.CompileAssemblyFromSource(parameters, _jscriptSource);
            Assembly assembly = results.CompiledAssembly;
            
            _evaluatorType = assembly.GetType("Evaluator.Evaluator");
            _evaluator = Activator.CreateInstance(_evaluatorType);
        }

        public static Object Eval(String statement)
        {
            return _evaluatorType.InvokeMember("Eval", BindingFlags.InvokeMethod, null, _evaluator, new Object[] { statement } );
        }
    }
}