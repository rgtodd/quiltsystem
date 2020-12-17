//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using RichTodd.QuiltSystem.Service.Micro.Abstractions;

namespace RichTodd.QuiltSystem.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SquareController : ControllerBase
    {
        private IEventProcessorMicroService EventProcessorMicroService { get; }
        private ISquareMicroService SquareMicroService { get; }

        public SquareController(
            IEventProcessorMicroService eventProcessorMicroService,
            ISquareMicroService squareMicroService)
        {
            EventProcessorMicroService = eventProcessorMicroService ?? throw new ArgumentNullException(nameof(eventProcessorMicroService));
            SquareMicroService = squareMicroService ?? throw new ArgumentNullException(nameof(squareMicroService));
        }

        public static async Task<byte[]> ReadRequestBodyAsync(Stream input)
        {
            using var ms = new MemoryStream();

            await input.CopyToAsync(ms);

            return ms.ToArray();
        }

        [HttpPost]
        public async Task<ActionResult> Post()
        {
            var requestBinary = await ReadRequestBodyAsync(Request.Body);
            var requestAscii = Encoding.ASCII.GetString(requestBinary);
            if (string.IsNullOrEmpty(requestAscii))
            {
                requestAscii = "<EMPTY>";
            }

            try
            {
                var squarePayloadId = await SquareMicroService.CreateWebhookPayloadAsync(requestAscii).ConfigureAwait(false);

                await SquareMicroService.ProcessWebhookPayloadAsync(squarePayloadId);

                _ = await EventProcessorMicroService.ProcessPendingEvents();
            }
            catch (Exception)
            {
                // Load exception.
            }

            return StatusCode(StatusCodes.Status200OK);
        }
    }
}