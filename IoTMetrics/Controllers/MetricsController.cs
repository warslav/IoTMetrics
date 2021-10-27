using IoTMetrics.Core.Interfaces;
using IoTMetrics.Models.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IoTMetrics.Controllers
{
    public class MetricsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public MetricsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: MetricsController/Create
        public ActionResult NewMetric(int deviceId)
        {
            Metric metric = new Metric
            {
                DeviceId = deviceId
            };

            return View(metric);
        }

        // POST: MetricsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewMetric([FromBody] Metric metric)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.MetricRepository.AddAsync(metric);
                await _unitOfWork.SaveAsync();
                return RedirectToAction("Details", "Device", new { id = metric.DeviceId });
            }
            return View(metric);
        }

        // GET: MetricsController/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = await _unitOfWork.MetricRepository.GetByIdAsync(id);
            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }

        // POST: MetricsController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var metric = await _unitOfWork.MetricRepository.GetByIdAsync(id);
            _unitOfWork.MetricRepository.Delete(id);
            await _unitOfWork.SaveAsync();
            return RedirectToAction("Details", "Device", new { id = metric.DeviceId });
        }
    }
}
