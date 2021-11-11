using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Store.Core.Domain
{
    public static class Ensure
    {
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNull(object arg, string message)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(message);
            }
        }
        
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T NotNull<T>(T arg, string message) => arg ?? throw new ArgumentNullException(message);
        
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNullOrEmpty(string arg, string message)
        {
            if (string.IsNullOrWhiteSpace(arg))
            {
                throw new ArgumentException(message);
            }
        }
        
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNullOrEmpty<T>(IEnumerable<T> arg, string message)
        {
            if (arg?.Any() != true)
            {
                throw new ArgumentException(message);
            }
        }
        
        public static class CommonMessages
        {
            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string NullOrEmpty(string argName) => $"{argName} is null or empty.";
        }
    }
}