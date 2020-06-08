 using System;
 using System.Threading;

 class CounterContext
 {
     static AsyncLocal<CounterContext?> local = new AsyncLocal<CounterContext?>();
     GuidCounter GuidCounter = new GuidCounter();
     DateTimeOffsetCounter DateTimeOffsetCounter = new DateTimeOffsetCounter();
     DateTimeCounter DateTimeCounter = new DateTimeCounter();

     public static CounterContext Current
     {
         get
         {
             var context = local.Value;
             if (context == null)
             {
                 throw new Exception("No current context");
             }
             return context;
         }
     }

     public static void Start()
     {
         local.Value = new CounterContext();
     }

     public int IntOrNext<T>(T input)
     {
         if (input is Guid guidInput)
         {
             return GuidCounter.IntOrNext(guidInput);
         }

         if (input is DateTime dateTimeInput)
         {
             return DateTimeCounter.IntOrNext(dateTimeInput);
         }

         if (input is DateTimeOffset dateTimeOffsetInput)
         {
             return DateTimeOffsetCounter.IntOrNext(dateTimeOffsetInput);
         }

         throw new Exception($"Unknown type {typeof(T).FullName}");
     }

     public static void Stop()
     {
         local.Value = null;
     }
 }