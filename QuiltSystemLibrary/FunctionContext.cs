//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace RichTodd.QuiltSystem
{
    public class FunctionContext : IFunctionContext
    {
        private const int MAX_LENGTH = 1000;
        private static JsonSerializerSettings JsonSerializerSettings { get; }

        private ILogger Logger { get; }
        private IDisposable LoggerScope { get; }
        private DateTime StartDateTime { get; }

        private bool m_resultLogged;

        private bool m_disposed;

        static FunctionContext()
        {
            JsonSerializerSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            };
        }

        public FunctionContext(ILogger logger, IDisposable loggerScope)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            LoggerScope = loggerScope ?? throw new ArgumentNullException(nameof(loggerScope));
            StartDateTime = DateTime.Now;
        }

        public void Exception(Exception ex)
        {
            Logger.LogError(ex, "Exception occurred.");
        }

        public void Message(string message)
        {
            Logger.LogInformation(message);
        }

        public void Result(object result)
        {
            var endDateTime = DateTime.Now;
            var elapsedTime = endDateTime - StartDateTime;

            var jsonResult = JsonSerialize(result);
            if (jsonResult.Length > MAX_LENGTH)
            {
                Logger.LogInformation($"Result = char[{jsonResult.Length}] ({elapsedTime.TotalMilliseconds} ms)");
            }
            else
            {
                Logger.LogInformation($"Result = {jsonResult} ({elapsedTime.TotalMilliseconds} ms)");
            }

            m_resultLogged = true;
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~FunctionContext()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {
                    if (!m_resultLogged)
                    {
                        var endDateTime = DateTime.Now;
                        var elapsedTime = endDateTime - StartDateTime;
                        Logger.LogInformation($"End function ({elapsedTime.TotalMilliseconds} ms)");
                    }

                    LoggerScope.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                m_disposed = true;
            }
        }

        private string JsonSerialize(object value)
        {
            return JsonConvert.SerializeObject(value, JsonSerializerSettings);
        }
    }
}
