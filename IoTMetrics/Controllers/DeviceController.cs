using IoTMetrics.Core.Interfaces;
using IoTMetrics.Models.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IoTMetrics.Controllers
{
    public class DeviceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeviceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        // GET: DeviceController
        public async Task<IActionResult> Index()
        {
            return View(await _unitOfWork.DeviceRepository.GetAllAsync());
        }

        // GET: DeviceController/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["DeviceId"] = id;
            ViewData["DeviceName"] = _unitOfWork.DeviceRepository.GetById(id ?? 0).Name ;
            var devices = await _unitOfWork.MetricRepository.Include(id);
            if (devices == null)
            {
                return NotFound();
            }
            return View(devices);
        }

        // GET: DeviceController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DeviceController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Device device)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.DeviceRepository.AddAsync(device);
                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(device);
        }

        // GET: DeviceController/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = await _unitOfWork.DeviceRepository.GetByIdAsync(id);
            if (device == null)
            {
                return NotFound();
            }
            return View(device);
        }

        // POST: DeviceController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Device device)
        {
            if (id != device.Id || !DeviceExists(device.Id))
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.DeviceRepository.Update(device);
                    
                    await _unitOfWork.SaveAsync();
                }
                
                catch (Exception)
                {
                    return View(device);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(device);
        }
        private bool DeviceExists(int id)
        {
            return _unitOfWork.DeviceRepository.GetAllEntities().Any(e => e.Id == id);
        }

        // GET: DeviceController/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = await _unitOfWork.DeviceRepository.GetByIdAsync(id);
            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }

        // POST: DeviceController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //.OnDelete(DeleteBehavior.Cascade) don`t work with SQLite
            List<Metric> devices = await _unitOfWork.MetricRepository.Include(id) as List<Metric>;
            for (int i = 0; i < devices.Count; i++)
            {
                _unitOfWork.MetricRepository.Delete(devices[i].Id);
            }

            _unitOfWork.DeviceRepository.Delete(id);
            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Search(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["DeviceId"] = id;
            ViewData["DeviceName"] = _unitOfWork.DeviceRepository.GetById(id ?? 0).Name;
            var devices = await _unitOfWork.MetricRepository.Include(id);
            if (devices == null)
            {
                return NotFound();
            }
            return View(devices);
        }

        public async Task<JsonResult> GraphicData(int deviceId, DateTime startDay, DateTime endDay, string name)
        {
            try
            {
                List<Metric> devices = await _unitOfWork.MetricRepository.GetMetricsBetweenDates(deviceId, startDay, endDay, name) as List<Metric>;
                var list = devices.Select(c => new {
                    Time = c.Time,
                    Value = c.Value
                });
                return new JsonResult(list);

            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
            
        }
    }
}
