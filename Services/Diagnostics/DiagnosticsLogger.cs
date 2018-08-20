﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Text;
using Microsoft.Azure.IoTSolutions.DeviceSimulation.Services.Http;
using Microsoft.Azure.IoTSolutions.DeviceSimulation.Services.Runtime;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.IoTSolutions.DeviceSimulation.Services.Diagnostics
{
    public interface IDiagnosticsLogger
    {
        Task<IHttpResponse> LogDiagnosticsData(string eventType, string message);
    }
    public class DiagnosticsLogger : IDiagnosticsLogger
    {
        private readonly IHttpClient httpClient;
        private readonly IServicesConfig servicesConfig;
              
        public DiagnosticsLogger(ILogger logger, IServicesConfig servicesConfig)
        {
            this.httpClient = new HttpClient(logger);
            this.servicesConfig = servicesConfig;
        }

        public async Task<IHttpResponse> LogDiagnosticsData(string eventType, string message = "")
        {
            dynamic jobj = new JObject();
            jobj.EventId = Guid.NewGuid().ToString();
            jobj.EventType = eventType;
            jobj.DeploymentId = this.servicesConfig.DeploymentId;
            jobj.SolutionType = this.servicesConfig.SolutionType;
            if (!string.IsNullOrEmpty(message))
            {
                jobj.EventProperties = new JObject(
                    new JProperty("ErrorMessage", message));
            }
            return await httpClient.PostAsync(this.PrepareRequest(this.servicesConfig.DiagnosticsEndpointUrl, jobj));
        }

        private HttpRequest PrepareRequest(string path, object obj=null)
        {
            var request = new HttpRequest();
            request.AddHeader(HttpRequestHeader.Accept.ToString(), "application/json");
            request.SetUriFromString(path);
            request.SetContent(obj);
            return request;
        }

    }
}