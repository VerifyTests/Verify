 namespace VerifyTests;

 public class Counter
 {
     static AsyncLocal<Counter?> local = new();

     ConcurrentDictionary<object, int> idCache = new();
     int currentId;

     public int NextId(object input)
     {
         return idCache.GetOrAdd(input, _ => Interlocked.Increment(ref currentId));
     }

     ConcurrentDictionary<Guid, int> guidCache = new();
     int currentGuid;

     public int Next(Guid input)
     {
         return guidCache.GetOrAdd(input, _ => Interlocked.Increment(ref currentGuid));
     }

     ConcurrentDictionary<DateTimeOffset, int> dateTimeOffsetCache = new();
     int currentDateTimeOffset;

     public int Next(DateTimeOffset input)
     {
         return dateTimeOffsetCache.GetOrAdd(input, _ => Interlocked.Increment(ref currentDateTimeOffset));
     }

     ConcurrentDictionary<DateTime, int> dateTimeCache = new();
     int currentDateTime;

     public int Next(DateTime input)
     {
         return dateTimeCache.GetOrAdd(input, _ => Interlocked.Increment(ref currentDateTime));
     }

     #if NET6_0_OR_GREATER

     ConcurrentDictionary<DateOnly, int> dateCache = new();
     int currentDate;

     public int Next(DateOnly input)
     {
         return dateCache.GetOrAdd(input, _ => Interlocked.Increment(ref currentDate));
     }

     #endif

     public static Counter Current
     {
         get
         {
             var context = local.Value;
             if (context is null)
             {
                 throw new("No current context");
             }

             return context;
         }
     }

     internal static Counter Start()
     {
         var context = new Counter();
         local.Value = context;
         return context;
     }

     internal static void Stop()
     {
         local.Value = null;
     }
 }