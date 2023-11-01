using System.Security.Claims;
using api.Data;
using api.Interfaces;
using api.Services;
using api.ViewModels;
using api.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using SQLitePCL;

namespace api.Controllers;
[ApiController]
[Route("api/v1/account")]
public class AccountController : ControllerBase
{
    private readonly UserManager<UserModel> _userManager;
    private readonly TokenService _tokenService;
    private readonly IUserRepository _userRepo;

    public AccountController(UserManager<UserModel> userManager, TokenService tokenService, IUserRepository userRepo)
    {
        _userRepo = userRepo;
        _tokenService = tokenService;
        _userManager = userManager;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var user = await _userManager.FindByNameAsync(model.UserName);
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

        await _userManager.AddToRoleAsync(user, "User");

        return StatusCode(201);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var result = await _userRepo.FindByIdAsync(id);

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
                    ReadStatus = b.ReadStatus
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

    [HttpGet("email")]
    public async Task<IActionResult> GetByEmail()
    {
        // "User" = property som hör till ControllerBase och hanterar claims
        // FindFirst plockar upp det första claimet som uppnår villkoret
        var emailClaim = User.FindFirst(claim => claim.Type == ClaimTypes.Email);
        string email = emailClaim.Value;

        var result = await _userRepo.FindByEmailAsync(email);

        var user = new ProfileViewModel
        {
            UserName = result.UserName,
            FirstName = result.FirstName,
            LastName = result.LastName,
            Books = result.Books!.Select(
                b => new BookBaseViewModel
                {
                    ImageUrl = b.ImageUrl,
                    Title = b.Title,
                    Author = b.Author,
                    PublicationYear = b.PublicationYear,
                    Review = b.Review,
                    ReadStatus = b.ReadStatus
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