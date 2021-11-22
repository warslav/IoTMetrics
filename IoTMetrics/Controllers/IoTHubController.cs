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
using IoTMetrics.Core.Services;

namespace IoTMetrics.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IoTHubController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailNotification _emailNotification;
        private readonly SenderAzureSB _senderAzureSB;
        private readonly MQTTBroker _mqttBroker;

        public IoTHubController(IUnitOfWork unitOfWork, IEmailNotification emailNotification, SenderAzureSB senderAzureSB, MQTTBroker mqttBroker)
        {
            _unitOfWork = unitOfWork;
            _emailNotification = emailNotification;
            _senderAzureSB = senderAzureSB;
            _mqttBroker = mqttBroker;
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
            await _emailNotification.CheckMetric(metric.Name, metric.Value);
            await _senderAzureSB.SendMessagesAsync(metric.Name, metric.Value, metric.Time, metric.DeviceId??0);
            await _mqttBroker.PublishMetricAsync(metric.Name, metric.Value, metric.Time, metric.DeviceId ?? 0);
            //await _unitOfWork.MetricRepository.AddAsync(metric);
            //await _unitOfWork.SaveAsync();

            return CreatedAtAction("GetMetric", new { id = metric.Id }, metric);
        }

        // GET: api/TestMetrics/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Metric>> GetMetric(int id)
        {
            var metric = await _unitOfWork.MetricRepository.GetByIdAsync(id);

            if (metric == null)
            {
                return NotFound();
            }

            return metric;
        }
    }
}
