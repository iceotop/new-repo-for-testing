using api.Data;
using api.Models;
using api.Services;
using api.ViewModels;
using api.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace api.Controllers;
[ApiController]
[Route("api/v1/account")]
public class AccountController : ControllerBase
{
    private readonly UserManager<UserModel> _userManager;
    private readonly SignInManager<UserModel> _signInManager;
    private readonly TokenService _tokenService;
    private readonly BookCircleContext _context;

    
    public AccountController(UserManager<UserModel> userManager, SignInManager<UserModel> signInManager, BookCircleContext context, TokenService tokenService)
    {
        _tokenService = tokenService;
        _signInManager = signInManager;
        _userManager = userManager;
        _context = context;

    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var user = await _userManager.FindByNameAsync(model.UserName); //what name exactly?
        if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
        {
            return Unauthorized();
        }

        // Generate Token
        var token = await _tokenService.CreateToken(user);

        return Ok(new UserViewModel
        {
            Email = user.Email,
            Token = token  // Include the token in the response
        });
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        var user = new UserModel
        {
            UserName = model.UserName,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }

            return ValidationProblem();
        }

        await _userManager.AddToRoleAsync(user, "Admin");

        return StatusCode(201);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var result = await _context.Users
            .Where(u => u.Id == id)
            .Include(u => u.Books)
            .Include (u => u.Events)
            .SingleOrDefaultAsync();

        var user = new ProfileViewModel
        {
            FirstName = result.FirstName,
            LastName = result.LastName,
            Books = result.Books!.Select(
                b => new BookBaseViewModel
                {
                    Title = b.Title,
                    Author = b.Author,
                    PublicationYear = b.PublicationYear,
                    Review = b.Review,
                    IsRead = b.IsRead
                }
            ).ToList(),
            Events = result.Events!.Select(
                e => new EventBaseViewModel
                {
                    Title = e.Title,
                    Description = e.Description,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate
                }).ToList()
        };
        return Ok(user);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            return NotFound($"Vi kan inte hitta en användare med id{id}");
        }

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            return NoContent();
        }
        return StatusCode(500, "Internal Server Error");
    }
}