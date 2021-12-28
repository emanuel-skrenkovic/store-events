using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Store.Core.Domain;

public static class Ensure
{
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T NotNull<T>(T arg, [CallerArgumentExpression("arg")] string argName = null) 
        => arg ?? throw new ArgumentNullException(argName?.TrimStart('@'));
        
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NotNullOrEmpty(string arg, string argName)
    {
        if (string.IsNullOrWhiteSpace(arg))
        {
            throw new ArgumentException(CommonMessages.NullOrEmpty(argName));
        }
    }
        
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NotNullOrEmpty<T>(IEnumerable<T> arg, [CallerArgumentExpression("arg")] string argName = null)
    {
        if (arg?.Any() != true)
        {
            throw new ArgumentException(CommonMessages.NullOrEmpty(argName?.TrimStart('@')));
        }
    }
        
    private static class CommonMessages
    {
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string NullOrEmpty(string argName) => $"{argName} is null or empty.";
    }
}