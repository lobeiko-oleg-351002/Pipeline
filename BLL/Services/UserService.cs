﻿using BLL.Mapping;
using BLL.Services.Interface;
using BllEntities;
using DAL.Entities;
using DAL.Repositories.Interface;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Services
{
    public class UserService : Service<BllUser, DalUser, User, UserMapper>, IUserService
    {
        // private readonly IUnitOfWork uow;

        public UserService(IUnitOfWork uow) : base(uow, uow.Users)
        {
            //    this.uow = uow;
            //   mapper = new UserMapper(uow);
        }

        protected override void InitMapper()
        {
            mapper = new UserMapper(uow);
        }

        public BllUser Authorize(string login, string password)
        {
            var user = uow.Users.Authorize(login, password);
            if (user != null)
            {
                return mapper.MapToBll(user);
            }
            return null;
        }

        public BllUser GetUserByLogin(string login)
        {
            return mapper.MapToBll(uow.Users.GetUserByLogin(login));
        }

        public new BllUser Create(BllUser entity)
        {
            var testEntity = uow.Users.GetUserByLogin(entity.Login);
            if (testEntity == null)
            {
                uow.Users.Create(mapper.MapToDal(entity));
                uow.Commit();
                return entity;
            }
            return null;
        }

        public IEnumerable<BllUser> GetUsersByGroup(int group_id)
        {
            List<BllUser> users = new List<BllUser>();
            foreach(var item in uow.Users.GetUsersByGroup(group_id))
            {
                users.Add(mapper.MapToBll(item));
            }
            return users;
        }
    }
}