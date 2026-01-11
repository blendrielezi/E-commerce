// Controllers/ProfileController.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjektSezon2.Models;

namespace ProjektSezon2.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ProfileController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: /Profile/Edit
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var model = new EditProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                City = user.City,
                BirthDate = user.BirthDate
            };
            return View(model);
        }

        // POST: /Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // Update fields
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.UserName = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.City = model.City;
            user.BirthDate = model.BirthDate;

            var emailResult = await _userManager.UpdateAsync(user);
            if (!emailResult.Succeeded)
            {
                foreach (var error in emailResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);
            ViewData["StatusMessage"] = "Your profile has been updated";
            return View(model);
        }

        // GET: /Profile/ChangePassword
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: /Profile/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var changeResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changeResult.Succeeded)
            {
                foreach (var error in changeResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);
            ViewData["StatusMessage"] = "Your password has been changed.";
            return View();
        }
    }

    // ViewModels
    public class EditProfileViewModel
    {
        [Required, Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Required, Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Required, EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        public string? City { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required, DataType(DataType.Password), Display(Name = "Current password")]
        public string OldPassword { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Display(Name = "New password")]
        public string NewPassword { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Display(Name = "Confirm new password"), Compare("NewPassword")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
