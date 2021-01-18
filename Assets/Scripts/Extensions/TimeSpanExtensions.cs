// Copyright © 2020 Bogdan Nikolayev <bodix321@gmail.com>
// All Rights Reserved

using System;

namespace Extensions
{
    // Too infrequently used extension methods for a frequently used types.
    public static class TimeSpanExtensions
    {
        public static TimeSpan Milliseconds(this sbyte value) => TimeSpan.FromMilliseconds(value);
        public static TimeSpan Milliseconds(this byte value) => TimeSpan.FromMilliseconds(value);
        public static TimeSpan Milliseconds(this short value) => TimeSpan.FromMilliseconds(value);
        public static TimeSpan Milliseconds(this ushort value) => TimeSpan.FromMilliseconds(value);
        public static TimeSpan Milliseconds(this int value) => TimeSpan.FromMilliseconds(value);
        public static TimeSpan Milliseconds(this uint value) => TimeSpan.FromMilliseconds(value);
        public static TimeSpan Milliseconds(this long value) => TimeSpan.FromMilliseconds(value);
        public static TimeSpan Milliseconds(this ulong value) => TimeSpan.FromMilliseconds(value);
        public static TimeSpan Milliseconds(this float value) => TimeSpan.FromMilliseconds(value);
        public static TimeSpan Milliseconds(this double value) => TimeSpan.FromMilliseconds(value);

        public static TimeSpan Seconds(this sbyte value) => TimeSpan.FromSeconds(value);
        public static TimeSpan Seconds(this byte value) => TimeSpan.FromSeconds(value);
        public static TimeSpan Seconds(this short value) => TimeSpan.FromSeconds(value);
        public static TimeSpan Seconds(this ushort value) => TimeSpan.FromSeconds(value);
        public static TimeSpan Seconds(this int value) => TimeSpan.FromSeconds(value);
        public static TimeSpan Seconds(this uint value) => TimeSpan.FromSeconds(value);
        public static TimeSpan Seconds(this long value) => TimeSpan.FromSeconds(value);
        public static TimeSpan Seconds(this ulong value) => TimeSpan.FromSeconds(value);
        public static TimeSpan Seconds(this float value) => TimeSpan.FromSeconds(value);
        public static TimeSpan Seconds(this double value) => TimeSpan.FromSeconds(value);

        public static TimeSpan Minutes(this sbyte value) => TimeSpan.FromMinutes(value);
        public static TimeSpan Minutes(this byte value) => TimeSpan.FromMinutes(value);
        public static TimeSpan Minutes(this short value) => TimeSpan.FromMinutes(value);
        public static TimeSpan Minutes(this ushort value) => TimeSpan.FromMinutes(value);
        public static TimeSpan Minutes(this int value) => TimeSpan.FromMinutes(value);
        public static TimeSpan Minutes(this uint value) => TimeSpan.FromMinutes(value);
        public static TimeSpan Minutes(this long value) => TimeSpan.FromMinutes(value);
        public static TimeSpan Minutes(this ulong value) => TimeSpan.FromMinutes(value);
        public static TimeSpan Minutes(this float value) => TimeSpan.FromMinutes(value);
        public static TimeSpan Minutes(this double value) => TimeSpan.FromMinutes(value);

        public static TimeSpan Hours(this sbyte value) => TimeSpan.FromHours(value);
        public static TimeSpan Hours(this byte value) => TimeSpan.FromHours(value);
        public static TimeSpan Hours(this short value) => TimeSpan.FromHours(value);
        public static TimeSpan Hours(this ushort value) => TimeSpan.FromHours(value);
        public static TimeSpan Hours(this int value) => TimeSpan.FromHours(value);
        public static TimeSpan Hours(this uint value) => TimeSpan.FromHours(value);
        public static TimeSpan Hours(this long value) => TimeSpan.FromHours(value);
        public static TimeSpan Hours(this ulong value) => TimeSpan.FromHours(value);
        public static TimeSpan Hours(this float value) => TimeSpan.FromHours(value);
        public static TimeSpan Hours(this double value) => TimeSpan.FromHours(value);

        public static TimeSpan Days(this sbyte value) => TimeSpan.FromDays(value);
        public static TimeSpan Days(this byte value) => TimeSpan.FromDays(value);
        public static TimeSpan Days(this short value) => TimeSpan.FromDays(value);
        public static TimeSpan Days(this ushort value) => TimeSpan.FromDays(value);
        public static TimeSpan Days(this int value) => TimeSpan.FromDays(value);
        public static TimeSpan Days(this uint value) => TimeSpan.FromDays(value);
        public static TimeSpan Days(this long value) => TimeSpan.FromDays(value);
        public static TimeSpan Days(this ulong value) => TimeSpan.FromDays(value);
        public static TimeSpan Days(this float value) => TimeSpan.FromDays(value);
        public static TimeSpan Days(this double value) => TimeSpan.FromDays(value);
    }
}