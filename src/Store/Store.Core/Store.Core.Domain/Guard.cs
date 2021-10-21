using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Store.Core.Domain
{
    public static class Guard
    {
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotNull(object arg, string message)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(message);
            }
        }
        
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T IsNotNull<T>(T arg, string message) => arg ?? throw new ArgumentNullException(message);
        
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotNullOrEmpty(string arg, string message)
        {
            if (string.IsNullOrWhiteSpace(arg))
            {
                throw new ArgumentException(message);
            }
        }
        
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotNullOrEmpty<T>(IEnumerable<T> arg, string message)
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