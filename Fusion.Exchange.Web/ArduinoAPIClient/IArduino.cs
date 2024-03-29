﻿// Code generated by Microsoft (R) AutoRest Code Generator 0.9.7.0
// Changes may cause incorrect behavior and will be lost if the code is regenerated.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fusion.Exchange.Web.ArduinoAPI.Models;
using Microsoft.Rest;

namespace Fusion.Exchange.Web.ArduinoAPI
{
    public partial interface IArduino
    {
        /// <param name='cancellationToken'>
        /// Cancellation token.
        /// </param>
        Task<HttpOperationResponse<IList<SalaDeReuniao>>> GetWithOperationResponseAsync(CancellationToken cancellationToken = default(System.Threading.CancellationToken));
        
        /// <param name='idCliente'>
        /// Required.
        /// </param>
        /// <param name='cancellationToken'>
        /// Cancellation token.
        /// </param>
        Task<HttpOperationResponse<IList<SalaDeReuniao>>> GetClienteByIdclienteWithOperationResponseAsync(int idCliente, CancellationToken cancellationToken = default(System.Threading.CancellationToken));
        
        /// <param name='token'>
        /// Required.
        /// </param>
        /// <param name='cancellationToken'>
        /// Cancellation token.
        /// </param>
        Task<HttpOperationResponse<string>> LimparByTokenWithOperationResponseAsync(string token, CancellationToken cancellationToken = default(System.Threading.CancellationToken));
        
        /// <param name='idCliente'>
        /// Required.
        /// </param>
        /// <param name='cancellationToken'>
        /// Cancellation token.
        /// </param>
        Task<HttpOperationResponse<string>> LimparClienteByIdclienteWithOperationResponseAsync(int idCliente, CancellationToken cancellationToken = default(System.Threading.CancellationToken));
        
        /// <param name='smtp'>
        /// Required.
        /// </param>
        /// <param name='haspeople'>
        /// Required.
        /// </param>
        /// <param name='idCliente'>
        /// Optional.
        /// </param>
        /// <param name='cancellationToken'>
        /// Cancellation token.
        /// </param>
        Task<HttpOperationResponse<string>> PostBySmtpAndHaspeopleAndIdclienteWithOperationResponseAsync(string smtp, string haspeople, int? idCliente = null, CancellationToken cancellationToken = default(System.Threading.CancellationToken));
    }
}
