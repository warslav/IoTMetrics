using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IoTMetrics.Database;
using IoTMetrics.Models.Models;
using IoTMetrics.Core.Interfaces;

namespace IoTMetrics.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailNotification _emailNotification;

        public NotificationsController(IUnitOfWork unitOfWork, IEmailNotification emailNotification)
        {
            _unitOfWork = unitOfWork;
            _emailNotification = emailNotification;
        }

        // GET: Notifications
        public async Task<IActionResult> Index()
        {
            return View(await _unitOfWork.NotificationRepository.GetAllAsync());
        }

        // GET: Notifications/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var notification = await _context.Notifications
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (notification == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(notification);
        //}

        // GET: Notifications/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Notifications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,MinValue,MaxValue,Condition,Email")] Notification notification)
        {
            var checkCorrectCondition = await _emailNotification.CheckCorrectConditionAsunc(notification.Condition);
            if (ModelState.IsValid && checkCorrectCondition.IsSucces)
            {
                notification.Condition = checkCorrectCondition.improvedСondition;
                await _unitOfWork.NotificationRepository.AddAsync(notification);
                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(notification);
        }

        // GET: Notifications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _unitOfWork.NotificationRepository.GetByIdAsync(id);
            if (notification == null)
            {
                return NotFound();
            }
            return View(notification);
        }

        // POST: Notifications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,MinValue,MaxValue,Condition,Email")] Notification notification)
        {
            if (id != notification.Id || !NotificationExists(notification.Id))
            {
                return NotFound();
            }
            var checkCorrectCondition = await _emailNotification.CheckCorrectConditionAsunc(notification.Condition);
            if (ModelState.IsValid && checkCorrectCondition.IsSucces)
            {
                try
                {
                    notification.Condition = checkCorrectCondition.improvedСondition;
                    _unitOfWork.NotificationRepository.Update(notification);
                    await _unitOfWork.SaveAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotificationExists(notification.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(notification);
        }

        // GET: Notifications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _unitOfWork.NotificationRepository.GetByIdAsync(id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var notification = await _unitOfWork.NotificationRepository.GetByIdAsync(id);
            _unitOfWork.NotificationRepository.Delete(id);
            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotificationExists(int id)
        {
            return _unitOfWork.NotificationRepository.GetAllEntities().Any(e => e.Id == id);
        }
    }
}
