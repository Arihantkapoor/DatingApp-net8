using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using API.Helpers;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;

namespace API.Data;

public class UserRepository(DataContext context, IMapper mapper) : IUserRepository
{
    public async Task<MemberDto?> GetMemberAsync(string username)
    {
        return await context.Users
        .Where(x=> x.UserName==username)
        .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
        .SingleOrDefaultAsync();
    }

    public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
    {
        var query =  context.Users.AsQueryable();

        query = query.Where(x=>x.UserName!=userParams.CurrentUserName);

        if(userParams.Gender!=null){
            query = query.Where(x=>x.Gender==userParams.Gender);
        }

        var mindob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
        var maxdob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

        query = query.Where(x=>x.DateOfBirth >=mindob && x.DateOfBirth<=maxdob);

        //new way of writing switch statement in c# 8.0
        query = userParams.OrderBy switch{
            "created" => query.OrderByDescending(x=>x.Created),
            _ => query.OrderByDescending(x=>x.LastActive)
        };

        
        return await PagedList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(mapper.ConfigurationProvider),userParams.PageNumber,userParams.PageSize);

    }

    public async Task<AppUser?> GetUserByIdAsync(int id)
    {
        return await context.Users.FindAsync(id);
    }

    public async Task<AppUser?> GetUserByUsernameAsync(string username)
    {
        return await context.Users.Include(x=>x.Photos)
        .SingleOrDefaultAsync(x=>x.UserName==username);
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        return await context.Users.Include(x=>x.Photos)
        .ToListAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync()>0;
    }

    public void Update(AppUser user)
    {
        context.Entry(user).State = EntityState.Modified;
    }
}
