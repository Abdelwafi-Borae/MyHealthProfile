using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyHealthProfile.pagination;
using Tawtheiq.Application.Cores.General.Dtos;

namespace MyHealthProfile.pagination
{
    public class UserFilterDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        // public Sort? Sort { get; set; }
    }
}


//public async Task<IActionResult> GetAllUser([FromQuery] UserFilterDto filter)
//{
//    var x = (await _service.GetAllUser(filter)).ToSuccessResult();
//    // Response.AddPaginationHeader();
//    Response.AddPaginationHeader(x.Data.MetaData);
//    return Ok(x);
//}
//return Ok(x);
//}

//public async Task<PagedList<GetAllUsersDto>> GetAllUser(UserFilterDto UserPram)
//{
//    //var users =  await _dbcontext.Roles.Where(c => c.Name == "SystemSuperAdmin").FirstOrDefaultAsync();
//    //string roleid = users.Id;
//    //List<string>userid = await _dbcontext.ApplicationUserRoles.Where(c => c.ApplicationRoleId == roleid).Select(c => c.UserId).ToListAsync();
//    IQueryable<GetAllUsersDto> query = _userManager.Users.Select(c => new GetAllUsersDto
//    {
//        FirstName = c.FirstName,
//        LastName = c.LastName,
//        UserId = c.Id,
//        Email = c.Email,
//        IsRegiteredCompleted = c.IsRegisterCompleted,
//        UserState = c.Status,
//    });
//    query = query.OrderBy(x => x.FirstName);
//    var users = await PagedList<GetAllUsersDto>.ToPagedList(query, UserPram.PageNumber, UserPram.PageSize);

//    return users;

//}