 using System;
 using System.Collections.Concurrent;
 using System.Runtime.CompilerServices;
 using System.Threading;
 using VerifyTests;

 class CounterContext
 {
     static AsyncLocal<CounterContext?> local = new();

     ConcurrentDictionary<Guid, int> guidCache = new();
     int currentGuid;

     public int NextGuid(Guid input)
     {
         return guidCache.GetOrAdd(input, _ => Interlocked.Increment(ref currentGuid));
     }

     ConcurrentDictionary<DateTimeOffset, int> dateTimeOffsetCache = new();
     int currentDateTimeOffset;

     public int NextDateTimeOffset(DateTimeOffset input)
     {
         return dateTimeOffsetCache.GetOrAdd(input, _ => Interlocked.Increment(ref currentDateTimeOffset));
     }

     ConcurrentDictionary<DateTime, int> dateTimeCache = new();
     int currentDateTime;

     public int NextDateTime(DateTime input)
     {
         return dateTimeCache.GetOrAdd(input, _ => Interlocked.Increment(ref currentDateTime));
     }

     [ModuleInitializer]
     public static void Init()
     {
         InnerVerifier.AddTestCallback(Start, Stop);
     }

     public static CounterContext Current
     {
         get
         {
             var context = local.Value;
             if (context == null)
             {
                 throw new("No current context");
             }

             return context;
         }
     }

     static void Start()
     {
         local.Value = new();
     }

     static void Stop()
     {
         local.Value = null;
     }
 }