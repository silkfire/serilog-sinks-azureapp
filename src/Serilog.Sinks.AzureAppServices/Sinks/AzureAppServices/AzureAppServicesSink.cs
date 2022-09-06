// Copyright 2013-2022 Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Serilog.Sinks.AzureAppServices
{
    using Core;
    using Events;
    using Formatting;

    using Microsoft.Extensions.Logging;

    using System;
    using System.Collections.Concurrent;
    using System.IO;

    internal class AzureAppServicesSink : ILogEventSink
    {
        /// <summary>
        /// Our very own <see cref="LoggerFactory"/>, this is where we'll send Serilog events so that Azure can pick up the logs.
        /// We expect that Serilog has replaced this in the app's services.
        /// </summary>
        private static ILoggerFactory CoreLoggerFactory { get; } = LoggerFactory.Create(builder => builder.AddAzureWebAppDiagnostics());

        /// <summary>
        /// The <see cref="LoggerFactory"/> implementation of <see cref="LoggerFactory.CreateLogger"/> uses <c>lock(_sync)</c> before looking in its dictionary.
        /// We'll use our own <see cref="ConcurrentDictionary{TKey,TValue}"/> for performance, since we lookup the category on every log write.
        /// </summary>
        private readonly ConcurrentDictionary<string, ILogger> _loggerCategories = new ConcurrentDictionary<string, ILogger>();

        private readonly ITextFormatter _textFormatter;

        public AzureAppServicesSink(ITextFormatter textFormatter)
        {
            _textFormatter = textFormatter ?? throw new ArgumentNullException(nameof(textFormatter));
        }

        public void Emit(LogEvent logEvent)
        {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
           
            var sr = new StringWriter();
            _textFormatter.Format(logEvent, sr);
            var text = sr.ToString().Trim();

            var category = logEvent.Properties.TryGetValue("SourceContext", out var value) ? value.ToString() : "";
            var logger = _loggerCategories.GetOrAdd(category, s => CoreLoggerFactory.CreateLogger(s));

            switch (logEvent.Level)
            {
                case LogEventLevel.Fatal:
                    logger.LogCritical(text);
                    break;
                case LogEventLevel.Error:
                    logger.LogError(text);
                    break;
                case LogEventLevel.Warning:
                    logger.LogWarning(text);
                    break;
                case LogEventLevel.Information:
                    logger.LogInformation(text);
                    break;
                case LogEventLevel.Debug:
                    logger.LogDebug(text);
                    break;
                case LogEventLevel.Verbose:
                    logger.LogTrace(text);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
