using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StationaryHub.Models;
using StationaryHub.ViewModels;

namespace StationaryHub.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = new ApplicationUser { FullName = model.FullName, UserName = model.Email, Email = model.Email, PhoneNumber = model.PhoneNumber };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, Roles.Customer);
            await _signInManager.SignInAsync(user, false);
            return RedirectToAction("Index", "Home");
        }
        foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
        return View(model);
    }

    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid) return View(model);
        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true);
        if (result.Succeeded) return LocalRedirect(returnUrl ?? Url.Action("Index", "Home")!);
        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();
        return View(new ProfileViewModel { Email = user.Email ?? string.Empty, FullName = user.FullName, PhoneNumber = user.PhoneNumber ?? string.Empty });
    }

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ProfileViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();
        user.FullName = model.FullName;
        user.PhoneNumber = model.PhoneNumber;
        await _userManager.UpdateAsync(user);
        TempData["Success"] = "Profile updated.";
        return RedirectToAction(nameof(Profile));
    }

    [Authorize]
    public IActionResult ChangePassword() => View(new ChangePasswordViewModel());

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();
        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (result.Succeeded)
        {
            await _signInManager.RefreshSignInAsync(user);
            TempData["Success"] = "Password changed.";
            return RedirectToAction(nameof(Profile));
        }
        foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
        return View(model);
    }

    public IActionResult AccessDenied() => View();
}
