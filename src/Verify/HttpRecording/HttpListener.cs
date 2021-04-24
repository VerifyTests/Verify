#if NET5_0
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DiagnosticAdapter;
using VerifyTests;

class HttpListener :
    IObserver<DiagnosticListener>,
    IDisposable
{
    ConcurrentQueue<IDisposable> subscriptions = new();
    AsyncLocal<List<HttpCall>?> local = new();

    public void Start()
    {
        local.Value = new();
    }

    public bool TryFinish(out IEnumerable<HttpCall>? entries)
    {
        entries = local.Value;

        if (entries == null)
        {
            return false;
        }

        local.Value = null;
        return true;
    }

    public IEnumerable<HttpCall> Finish()
    {
        var localValue = local.Value;

        if (localValue == null)
        {
            throw new("HttpRecording.StartRecording must be called prior to FinishRecording.");
        }

        local.Value = null;
        return localValue;
    }

    public void OnNext(DiagnosticListener value)
    {
        if (value.Name != "HttpHandlerDiagnosticListener")
        {
            return;
        }

        subscriptions.Enqueue(value.SubscribeWithAdapter(this, _ => local.Value != null));
    }

    [DiagnosticName("System.Net.Http.HttpRequestOut")]
    public void IsEnabled()
    {
    }

    [DiagnosticName("System.Net.Http.HttpRequestOut.Start")]
    public virtual void OnHttpRequestOutStart(HttpRequestMessage request)
    {
        Debug.WriteLine("d");
    }

    [DiagnosticName("System.Net.Http.HttpRequestOut.Stop")]
    public virtual void OnHttpRequestOutStop(HttpRequestMessage request, HttpResponseMessage response, TaskStatus requestTaskStatus)
    {
        local.Value!.Add(new(request, response, requestTaskStatus));
    }

    void Clear()
    {
        foreach (var subscription in subscriptions)
        {
            subscription.Dispose();
        }
    }

    public void Dispose()
    {
        Clear();
    }

    public void OnCompleted()
    {
        Clear();
    }

    public void OnError(Exception error)
    {
    }
}
#endif