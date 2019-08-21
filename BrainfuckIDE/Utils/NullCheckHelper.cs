#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckIDE.Utils
{
    static class NullCheckHelper
    {
        static public TResult ApllyTo<TArg, TResult>(this TArg arg, Func<TArg, TResult> func)
        {
            if (func is null) throw new ArgumentNullException(nameof(func));
            return func(arg);
        }
        static public void ApllyTo<TArg>(this TArg arg, Action<TArg> func)
        {
            if (func is null) throw new ArgumentNullException(nameof(func));
            func(arg);
        }
    }
}
