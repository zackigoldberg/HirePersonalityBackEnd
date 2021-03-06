﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HirePersonality.Database.Contexts;
using HirePersonality.Database.DataContract.Authorization.Interfaces;
using HirePersonality.Database.DataContract.Authorization.RAOs;
using HirePersonality.Database.Entities.People;
using System;
using System.Threading.Tasks;

namespace HirePersonality.Database.Authorization
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IMapper _mapper;
        private readonly SISContext _context;
        private readonly UserManager<UserEntity> _userManager;
        private readonly SignInManager<UserEntity> _signInManager;


        public AuthRepository(IMapper mapper, SISContext context, 
                UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;  
            _context = context;
        }

        public async Task<ReceivedExistingUserRAO> Login(QueryForExistingUserRAO queryRao)
        {
            var user = await _userManager.FindByNameAsync(queryRao.UserName);

            var result = await _signInManager
                .CheckPasswordSignInAsync(user, queryRao.Password, false);

            if (result.Succeeded)
            {
                var appUser = await _userManager.Users
                  .FirstOrDefaultAsync(u => u.NormalizedUserName == queryRao.UserName.ToUpper());

                var admin = IsUserAdmin(appUser);

                var rao =_mapper.Map<ReceivedExistingUserRAO>(appUser);

                rao.Admin = await admin;

                return rao;
            }

            throw new NotImplementedException();
        }

        private async Task<bool> IsUserAdmin(UserEntity user)
        {
            return await _userManager.IsInRoleAsync(user, "Admin");
        }

        public async Task<ReceivedExistingUserRAO> Register(RegisterUserRAO regUserRAO, string password)
        {
            var user = _mapper.Map<UserEntity>(regUserRAO);


            var result = await _userManager.CreateAsync(user, password);

            await _userManager.AddToRoleAsync(user, "user");

            if (result.Succeeded)
            {
                var rao = _mapper.Map<ReceivedExistingUserRAO>(user);
                return rao;
            }
            throw new NotImplementedException("User already exists");
        }

        public async Task<bool> UserExists(string username)
        {
            var query = await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);
            if (query != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<ReceivedExistingUserRAO> GetUserById(int ownerId)
        {
            var query = await _context.UserTableAccess.FirstOrDefaultAsync(u => u.Id == ownerId);

            var user = _mapper.Map<ReceivedExistingUserRAO>(query);

            return user;
        }

        public async Task<bool> AmIAnAdmin(int id)
        {
            var entity = await _context.UserRoles.FirstOrDefaultAsync(e => e.UserId == id);

            var admin = entity.RoleId == 2;

            return admin;
        }
    }
}