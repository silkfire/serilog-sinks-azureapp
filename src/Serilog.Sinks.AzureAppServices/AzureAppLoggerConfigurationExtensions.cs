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



namespace Serilog
{
    using Sinks.AzureAppServices;

    using Configuration;
    using Core;
    using Events;
    using Formatting.Display;
    using Formatting;
    using Formatting.Json;

    using System;

    /// <summary>
    /// Adds the <c>Serilog.WriteTo.AzureAppServices()</c> extension method to <see cref="LoggerConfiguration"/>.
    /// </summary>
    public static class TraceLoggerConfigurationExtensions
    {
        private const string DefaultOutputTemplate = "{Message}{NewLine}{Exception}";

        /// <summary>
        /// Write log events to the Azure Web Apps diagnostics log.
        /// </summary>
        /// <param name="sinkConfiguration">Logger sink configuration.</param>
        /// <param name="restrictedToMinimumLevel">The minimum level for
        /// events passed through the sink. Ignored when <paramref name="levelSwitch"/> is specified.</param>
        /// <param name="levelSwitch">A switch allowing the pass-through minimum level
        /// to be changed at runtime.</param>
        /// <param name="outputTemplate">A message template describing the format used to write to the sink.
        /// the default is "{Message}{NewLine}{Exception}".</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or <see langword="null"/>.</param>
        /// <returns>Configuration object allowing method chaining.</returns>
        public static LoggerConfiguration AzureAppServices(
            this LoggerSinkConfiguration sinkConfiguration,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            string outputTemplate = DefaultOutputTemplate,
            IFormatProvider formatProvider = null,
            LoggingLevelSwitch levelSwitch = null)
        {
            if (sinkConfiguration == null) throw new ArgumentNullException(nameof(sinkConfiguration));
            if (outputTemplate == null) throw new ArgumentNullException(nameof(outputTemplate));

            var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);

            return AzureAppServices(sinkConfiguration, formatter, restrictedToMinimumLevel, levelSwitch);
        }

        /// <summary>
        /// Write log events to the Azure Web Apps diagnostics log.
        /// </summary>
        /// <param name="sinkConfiguration">Logger sink configuration.</param>
        /// <param name="restrictedToMinimumLevel">The minimum level for
        /// events passed through the sink. Ignored when <paramref name="levelSwitch"/> is specified.</param>
        /// <param name="levelSwitch">A switch allowing the pass-through minimum level
        /// to be changed at runtime.</param>
        /// <param name="formatter">A custom formatter to apply to the output events. This can be used with
        /// e.g. <see cref="JsonFormatter"/> to produce JSON output. To customize the text layout only, use the
        /// overload that accepts an output template instead.</param>
        /// <returns>Configuration object allowing method chaining.</returns>
        public static LoggerConfiguration AzureAppServices(
            this LoggerSinkConfiguration sinkConfiguration,
            ITextFormatter formatter,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            LoggingLevelSwitch levelSwitch = null)
        {
            if (sinkConfiguration == null) throw new ArgumentNullException(nameof(sinkConfiguration));
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));

            return sinkConfiguration.Sink(new AzureAppServicesSink(formatter), restrictedToMinimumLevel, levelSwitch);
        }
    }
}
