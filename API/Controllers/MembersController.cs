using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class MembersController(AppDbContext Context) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AppUser>>> GetMembers()
    {
        var members = await Context.Users.ToListAsync();
        return members;
    }
    [Authorize]
    [HttpGet("{Id}")]
    public async Task<ActionResult<AppUser>> GetMemberById(string Id)
    {
        var member = await Context.Users.FindAsync(Id);
        if (member == null) return NotFound();
        return member;
    }
}
