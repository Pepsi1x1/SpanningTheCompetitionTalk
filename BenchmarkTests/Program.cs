using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using System;
using System.Runtime.InteropServices;

namespace BenchmarkTests
{
    public static class TalkStringExtensions
    {
        public static string Mask(this string mobile)
        {
            const string SPACE_SINGLE = " ";
            const char ASTERISK = '*';
            const int MIN_MOBILE_NUMBER_LENGTH = 7;

            if (string.IsNullOrWhiteSpace(mobile) || mobile.Length < MIN_MOBILE_NUMBER_LENGTH)
            {
                return string.Empty;
            }

            string firstPart = mobile.Substring(0, 3);
            string secondPart = mobile.Substring(firstPart.Length);

            string result = string.Join(SPACE_SINGLE,
                                        firstPart.Substring(0, 3)
                                                 .PadRight(5, ASTERISK),
                                        secondPart.Substring(secondPart.Length - 3)
                                                 .PadLeft(6, ASTERISK));

            return result;
        }

        public static string MaskSpan(this ReadOnlySpan<char> mobile)
        {
            const char ASTERISK = '*';
            const int MIN_MOBILE_NUMBER_LENGTH = 7;

            if (mobile.Length < MIN_MOBILE_NUMBER_LENGTH || mobile.IsWhiteSpace())
            {
                return null;
            }

            ReadOnlySpan<char> firstPart = mobile.Slice(0, 3);
            ReadOnlySpan<char> secondPart = mobile.Slice(mobile.Length - 3);

            string result = $"{firstPart.ToString()}{new string(ASTERISK, mobile.Length - 6)}{secondPart.ToString()}";

            return result;
        }

        public static string MaskJames(this string mobile)
        {
            const int MIN_MOBILE_NUMBER_LENGTH = 7;
            const int MASK_PERCENT = 55;
            const char ASTERISK = '*';
            if (string.IsNullOrWhiteSpace(mobile))
            {
                return string.Empty;
            }

            int total = mobile.Length;
            if (total < MIN_MOBILE_NUMBER_LENGTH)
            {
                return string.Empty;
            }
            char[] chars = mobile.ToCharArray();

            int totalCharsToHide = (int)Math.Round(((double)(total * MASK_PERCENT)) / 100);
            int totalCharsToShow = total - totalCharsToHide;
            int offset = totalCharsToShow / 2;
            for (int i = 0; i < totalCharsToHide; i++)
            {
                chars[i + offset] = ASTERISK;
            }

            return new string(chars);
        }

        public static string MaskCombined(this ReadOnlySpan<char> mobile)
        {
            const int MIN_MOBILE_NUMBER_LENGTH = 7;
            const int VISIBLE_CHARS = 6;
            const char ASTERISK = '*';
            const int MaxStackSize = 128;

            if (mobile.Length < MIN_MOBILE_NUMBER_LENGTH || mobile.IsWhiteSpace())
            {
                return null;
            }

            int total = mobile.Length;

            Span<char> chars =
                total > MaxStackSize
                    ? new char[total]
                    : stackalloc char[total];

            mobile.CopyTo(chars);

            int halfVisibleChars = VISIBLE_CHARS / 2;
            int hideCharsEndIndex = total - halfVisibleChars;
            for (int i = halfVisibleChars; i < hideCharsEndIndex; i++)
            {
                chars[i] = ASTERISK;
            }

            return chars.ToString();
        }

        public static string MaskFinal(this ReadOnlySpan<char> mobile)
        {
            const int MIN_MOBILE_NUMBER_LENGTH = 7;
            const int VISIBLE_CHARS = 6;
            const char ASTERISK = '*';
            const int MaxStackSize = 128;

            if (mobile.Length < MIN_MOBILE_NUMBER_LENGTH || mobile.IsWhiteSpace())
            {
                return null;
            }

            int total = mobile.Length;

            Span<char> chars =
                total > MaxStackSize
                    ? new char[total]
                    : stackalloc char[total];

            mobile.CopyTo(chars);

            int totalCharsToHide = total - VISIBLE_CHARS;
            chars.Slice(VISIBLE_CHARS >> 1, totalCharsToHide).Fill(ASTERISK);

            return chars.ToString();
        }

        public static string MaskNoBounds(this ReadOnlySpan<char> mobile)
        {
            const int MIN_MOBILE_NUMBER_LENGTH = 7;
            const int VISIBLE_CHARS = 6;
            const char ASTERISK = '*';
            const int MaxStackSize = 128;

            if (mobile.Length < MIN_MOBILE_NUMBER_LENGTH || mobile.IsWhiteSpace())
            {
                return null;
            }

            int total = mobile.Length;

            Span<char> chars =
                total > MaxStackSize
                    ? new char[total]
                    : stackalloc char[total];

            mobile.CopyTo(chars);

            int halfVisibleChars = VISIBLE_CHARS >> 1;
            int hideCharsEndIndex = total - halfVisibleChars;
            ref char arrayMemoryRegion = ref chars[0];
            for (int i = halfVisibleChars; i < hideCharsEndIndex; i++)
            {
                System.Runtime.CompilerServices.Unsafe.Add(ref arrayMemoryRegion, i) = ASTERISK;
            }

            return chars.ToString();
        }

        public static void MaskFastest(string mobile)
        {
            const int MIN_MOBILE_NUMBER_LENGTH = 7;
            const int VISIBLE_CHARS = 6;
            const char ASTERISK = '*';

            if (string.IsNullOrWhiteSpace(mobile) || mobile.Length < MIN_MOBILE_NUMBER_LENGTH)
            {
                return;
            }

            var chars = MemoryMarshal.AsMemory(mobile.AsMemory()).Span;

            int halfVisibleChars = VISIBLE_CHARS >> 1;
            int hideCharsEndIndex = mobile.Length - halfVisibleChars;
            ref char arrayMemoryRegion = ref chars[0];
            for (int i = halfVisibleChars; i < hideCharsEndIndex; i++)
            {
                System.Runtime.CompilerServices.Unsafe.Add(ref arrayMemoryRegion, i) = ASTERISK;
            }
        }

        public static unsafe void MaskFastestPtr(string mobile)
        {
            const int MIN_MOBILE_NUMBER_LENGTH = 7;
            const int VISIBLE_CHARS = 6;
            const char ASTERISK = '*';

            if (string.IsNullOrWhiteSpace(mobile) || mobile.Length < MIN_MOBILE_NUMBER_LENGTH)
            {
                return;
            }

            int halfVisibleChars = VISIBLE_CHARS >> 1;
            int hideCharsEndIndex = mobile.Length - halfVisibleChars;
            fixed (char* mobilePtr = mobile)
            {
                char* writeablePtr = mobilePtr;
                writeablePtr += halfVisibleChars;
                for (int i = halfVisibleChars; i < hideCharsEndIndex; i++, writeablePtr++)
                {
                    *writeablePtr = ASTERISK;
                }
            }
        }

        public static unsafe void MaskFastestPtrB(ReadOnlySpan<char> mobile)
        {
            const int MIN_MOBILE_NUMBER_LENGTH = 7;
            const int HALF_VISIBLE_CHARS = 3;
            const char ASTERISK = '*';

            //Spans aren't null, just empty
            if (MIN_MOBILE_NUMBER_LENGTH > mobile.Length || mobile.IsWhiteSpace())
            {
                return;
            }


            fixed (char* mobilePtr = mobile)
            {
                int hideCharsEndIndex = mobile.Length - HALF_VISIBLE_CHARS;

                char* writeablePtr = mobilePtr + HALF_VISIBLE_CHARS;
                for (int i = 0; i < hideCharsEndIndex; i++, writeablePtr++)
                {
                    *writeablePtr = ASTERISK;
                }
            }
        }

        public static void MaskFastestB(string mobile)
        {
            const int MIN_MOBILE_NUMBER_LENGTH = 7;
            const int VISIBLE_CHARS = 6;
            const char ASTERISK = '*';

            if (string.IsNullOrWhiteSpace(mobile) || mobile.Length < MIN_MOBILE_NUMBER_LENGTH)
            {
                /* do nothing */
            }
            else
            {
                var chars = MemoryMarshal.AsMemory(mobile.AsMemory()).Span;

                int halfVisibleChars = VISIBLE_CHARS >> 1;
                int hideCharsEndIndex = mobile.Length - halfVisibleChars;
                ref char arrayMemoryRegion = ref chars[0];
                for (int i = halfVisibleChars; i < hideCharsEndIndex; i++)
                {
                    System.Runtime.CompilerServices.Unsafe.Add(ref arrayMemoryRegion, i) = ASTERISK;
                }
            }
        }

        public static void MaskFastestC(string mobile)
        {
            const int MIN_MOBILE_NUMBER_LENGTH = 7;
            const int VISIBLE_CHARS = 6;
            const char ASTERISK = '*';

            if (string.IsNullOrWhiteSpace(mobile))
            {
                /* do nothing */
            }
            else
            {
                int mobileLength = mobile.Length;
                if (mobileLength < MIN_MOBILE_NUMBER_LENGTH)
                {
                    /* do nothing */
                }
                else
                {
                    var chars = MemoryMarshal.AsMemory(mobile.AsMemory()).Span;

                    int halfVisibleChars = VISIBLE_CHARS >> 1;
                    int hideCharsEndIndex = mobileLength - halfVisibleChars;
                    ref char arrayMemoryRegion = ref chars[0];
                    for (int i = halfVisibleChars; i < hideCharsEndIndex; i++)
                    {
                        System.Runtime.CompilerServices.Unsafe.Add(ref arrayMemoryRegion, i) = ASTERISK;
                    }
                }
            }
        }

        public static unsafe void MaskFixedPtr(string mobile)
        {
            const int MIN_MOBILE_NUMBER_LENGTH = 7;
            const int VISIBLE_CHARS = 6;
            const char ASTERISK = '*';

            if (string.IsNullOrWhiteSpace(mobile))
            {
                //do nothing
            }
            else
            {
                int l = mobile.Length;
                if (l < MIN_MOBILE_NUMBER_LENGTH)
                {
                    //do nothing
                }
                else
                {
                    const int halfVisibleChars = VISIBLE_CHARS >> 1;
                    int hideCharsEndIndex = l - halfVisibleChars;
                    fixed (char* mobilePtr = mobile)
                    {
                        for (int i = halfVisibleChars; i < hideCharsEndIndex; i++)
                        {
                            mobilePtr[i] = ASTERISK;
                        }
                    }
                }
            }
        }

        public static void ReturnStatement(string mobile)
        {
            if(string.IsNullOrWhiteSpace(mobile))
            {
                return;
            }

            if(mobile.Length < 7)
            {
                return;
            }

            var _ = mobile;
        }

        public static void BranchNoReturn(string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile))
            {
                // return
            }
            else
            {
                if (mobile.Length < 7)
                {
                    //return
                }
                else
                {
                    var _ = mobile;
                }
            }
        }

        public static void CallNotVirtual(ReadOnlySpan<char> mobile)
        {
            if (mobile.IsWhiteSpace())
            {
                // return
            }
            else
            {
                if (mobile.Length < 7)
                {
                    //return
                }
                else
                {
                    var _ = mobile;
                }
            }
        }

    }

    [BenchmarkDotNet.Attributes.MemoryDiagnoser]
    public class TalkBenchmarks
    {
        [BenchmarkDotNet.Attributes.Params("07111222333", "+447111222333", "239874982379847923749823798472398749823798472398749873897492837")]
        public string Mobile { get; set; }

        [BenchmarkDotNet.Attributes.Benchmark(Baseline = true)]
        public void String() => TalkStringExtensions.Mask(Mobile);

        [BenchmarkDotNet.Attributes.Benchmark()]
        public void Span() => TalkStringExtensions.MaskSpan(Mobile);
    }

    public class Program
    {
        static Guid UltimatePowerPlanClone = Guid.Parse("c23b9214-79ed-4e5b-9df4-61f5f87ef144");

        static void Main(string[] args)
        {
            
            var config = DefaultConfig.Instance
            .AddJob(Job
                .MediumRun.WithPowerPlan(UltimatePowerPlanClone)
                .WithOutlierMode(Perfolizer.Mathematics.OutlierDetection.OutlierMode.DontRemove)
                .WithWarmupCount(5)
                .WithIterationCount(5)
                .WithLaunchCount(1)
                .WithRuntime(CoreRuntime.Core70));

            BenchmarkDotNet.Running.BenchmarkRunner.Run<TalkBenchmarks>(config);
        }
    }
}