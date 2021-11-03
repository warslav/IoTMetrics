using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IoTMetrics.Database;
using IoTMetrics.Models.Models;
using IoTMetrics.Core.Interfaces;

namespace IoTMetrics.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IoTHubController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public IoTHubController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // POST: api/IoTHub
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Metric>> PostMetric([FromBody] Metric metric)
        {
            if (metric.DeviceId == 0 || metric.DeviceId == null || metric.Value == 0 || metric.Name == "")
            {
                return BadRequest();
            }
            await _unitOfWork.MetricRepository.AddAsync(metric);
            await _unitOfWork.SaveAsync();

            return CreatedAtAction("GetMetric", new { id = metric.Id }, metric);
        }

    }
}
