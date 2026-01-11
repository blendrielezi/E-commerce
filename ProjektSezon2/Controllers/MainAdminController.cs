// Controllers/MainAdminController.cs
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ProjektSezon2.Models;
using ProjektSezon2.Filters;
using System.Collections.Generic;

namespace ProjektSezon2.Controllers
{
    [ServiceFilter(typeof(AdminFilter))]
    
    [Authorize(Roles = "Admin")]
    public class MainAdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public MainAdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: /MainAdmin//lista e perdoruese me email,username e role
        
        public IActionResult Index()
        {
            var users = _userManager.Users
                .Select(u => new UserRoleViewModel
                {
                    Id = u.Id,
                    Email = u.Email,
                    UserName = u.UserName
                })
                .ToList();

            foreach (var vm in users)
            {
                var roles = _userManager.GetRolesAsync(_userManager.FindByIdAsync(vm.Id).Result).Result;
                vm.Roles = string.Join(", ", roles);
            }

            return View(users);
        }

        // GET: /MainAdmin/Edit/{id}zgjedj edhe role
        
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var vm = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                SelectedRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault()
            };
            vm.AvailableRoles = _roleManager.Roles.Select(r => r.Name).ToList();
            return View(vm);
        }

        // POST: /MainAdmin/Edit
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = await _userManager.FindByIdAsync(vm.Id);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!string.IsNullOrEmpty(vm.SelectedRole))
            {
                await _userManager.AddToRoleAsync(user, vm.SelectedRole);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /MainAdmin/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction(nameof(Index));
        }
    }

    // Përfaqson informacionin baz te perdoruesit bashk me rolet e tij si string tbashkuar.
    public class UserRoleViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Roles { get; set; } = string.Empty;
    }
   // Përfaqson modelin per redaktimin e perdoruesit, perfshir rolin e zgjedhur dhe listen e roleve disponueshme.
    public class EditUserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? SelectedRole { get; set; }
        public List<string> AvailableRoles { get; set; } = new List<string>();
    }
}