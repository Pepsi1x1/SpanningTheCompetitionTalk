using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;
using System;
using System.Runtime.InteropServices;

namespace BenchmarkTests
{
    public static class PointerMagicExtensions
    {
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

        public static void MaskFastestB(string mobile)
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

                    var chars = MemoryMarshal.AsMemory(mobile.AsMemory()).Span;

                    const int halfVisibleChars = VISIBLE_CHARS >> 1;
                    int hideCharsEndIndex = mobile.Length - halfVisibleChars;
                    for (int i = halfVisibleChars; i < hideCharsEndIndex; i++)
                    {
                        chars[i] = ASTERISK;
                    }
                }
            }
        }

        public static unsafe void MaskFastestPtrCopy(string mobile)
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


public static unsafe void MaskFastestPtr(string mobile)
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

        public static unsafe void MaskFastestPtrSJHS_ROS(ReadOnlySpan<char> mobile)
        {
            const int MIN_MOBILE_NUMBER_LENGTH = 7;
            const int VISIBLE_CHARS = 6;
            const char ASTERISK = '*';

            if (mobile.IsWhiteSpace())
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

        public static unsafe void MaskFastestPtrSJHSAlt(string mobile)
        {
            const int MIN_MOBILE_NUMBER_LENGTH = 7;
            const int VISIBLE_CHARS = 6;
            const char ASTERISK = '*';

            if (string.IsNullOrWhiteSpace(mobile) || mobile.Length < MIN_MOBILE_NUMBER_LENGTH)
            {
                return;
            }

            const int halfVisibleChars = VISIBLE_CHARS >> 1;
            int hideCharsEndIndex = mobile.Length - halfVisibleChars;
            fixed (char* mobilePtr = mobile)
            {
                for (int i = halfVisibleChars; i < hideCharsEndIndex; i++)
                {
                    mobilePtr[i] = ASTERISK;
                }
            }
        }

        public static unsafe void MaskFastestPtrConstOpt(ReadOnlySpan<char> mobile)
        {
            const int MIN_MOBILE_NUMBER_LENGTH = 7;
            const int HALF_VISIBLE_CHARS = 3;
            const char ASTERISK = '*';

            //Spans arent null, just empty
            if (MIN_MOBILE_NUMBER_LENGTH > mobile.Length || mobile.IsWhiteSpace())
            {
                return;
            }

            fixed (char* mobilePtr = mobile)
            {
                //For a span this is no longer a callvirt
                int hideCharsEndIndex = mobile.Length - HALF_VISIBLE_CHARS;

                char* writeablePtr = mobilePtr + HALF_VISIBLE_CHARS;
                for (int i = 0; i < hideCharsEndIndex; i++, writeablePtr++)
                {
                    *writeablePtr = ASTERISK;
                }
            }
        }

        public static void MaskFastestEmptyBodyIf(string mobile)
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

        public static void MaskFastestStoreLength(string mobile)
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

    }

    [BenchmarkDotNet.Attributes.MemoryDiagnoser]
    //[BenchmarkDotNet.Attributes.RPlotExporter]
    public class PointerMagic
    {
        [BenchmarkDotNet.Attributes.Params("07111222333", "+447111222333", "239874982379847923749823798472398749823798472398749873897492837")]
        public string Mobile { get; set; }

        [BenchmarkDotNet.Attributes.Benchmark(Baseline = true)]
        public void MaskFastest() => PointerMagicExtensions.MaskFastest(Mobile);

        [BenchmarkDotNet.Attributes.Benchmark()]
        public void MaskFastestPtr() => PointerMagicExtensions.MaskFastestPtr(Mobile);

        [BenchmarkDotNet.Attributes.Benchmark()]
        public void MaskFastestB() => PointerMagicExtensions.MaskFastestB(Mobile);

#if NET48
        [BenchmarkDotNet.Attributes.Benchmark()]
        public void MaskFastestPtrSJHS_ROS() => PointerMagicExtensions.MaskFastestPtrSJHS_ROS(Mobile.AsSpan());
#else
        [BenchmarkDotNet.Attributes.Benchmark()]
        public void MaskFastestPtrSJHS_ROS() => PointerMagicExtensions.MaskFastestPtrSJHS_ROS(Mobile);
#endif


    }

#if PTR
    public class Program
    {
        static void Main(string[] args)
        {
            var config = DefaultConfig.Instance
            .AddJob(Job
                .MediumRun.WithPowerPlan(PowerPlan.Balanced)
                .WithOutlierMode(Perfolizer.Mathematics.OutlierDetection.OutlierMode.DontRemove)
                .WithWarmupCount(5)
                .WithIterationCount(5)
                .WithLaunchCount(1)
                .WithRuntime(BenchmarkDotNet.Environments.CoreRuntime.Core70));
                //.WithRuntime(BenchmarkDotNet.Environments.ClrRuntime.Net48)); ;

            var summary = BenchmarkDotNet.Running.BenchmarkRunner.Run<PointerMagic>(config);
        }
    }
#endif
}
