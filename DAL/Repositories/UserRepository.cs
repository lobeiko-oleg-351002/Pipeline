﻿using DAL.Entities;
using DAL.Mapping;
using DAL.Repositories.Interface;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Repositories
{
    public class UserRepository : Repository<DalUser, User, UserMapper>, IUserRepository
    {
        private readonly ServiceDB context;
        public UserRepository(ServiceDB context) : base(context)
        {
            this.context = context;
        }

        public DalUser Authorize(string login, string password)
        {
            var ormEntity = context.Users.FirstOrDefault(entity => entity.login == login);
            if (ormEntity != null)
            {
                if (ormEntity.password.Equals(password))
                {
                    return mapper.MapToDal(ormEntity);
                }
            }
            return null;

        }

        public DalUser GetUserByLogin(string login)
        {
            var ormEntity = context.Users.FirstOrDefault(entity => entity.login == login);
            if (ormEntity == null) return null;
            return mapper.MapToDal(ormEntity);
        }

        public List<DalUser> GetUsersByGroup(int group_id)
        {
            List<DalUser> users = new List<DalUser>();
            foreach (var item in context.Users.Where(entity => entity.group_id == group_id))
            {
                users.Add(mapper.MapToDal(item));
            }
            return users;
        }

        public List<DalUser> GetUsersByGroupAndSignInDateRange(int group_id, int permissibleRangeInDays)
        {
            List<DalUser> users = new List<DalUser>();
           
            foreach (var item in context.Users.Where(entity => entity.group_id == group_id))
            {
                if (item.signInDate != null)
                {
                    if (IsDateInPermissibleRange(item.signInDate.Value, permissibleRangeInDays))
                    {
                        users.Add(mapper.MapToDal(item));
                    }
                }
            }
            return users;
        }

        private bool IsDateInPermissibleRange(DateTime date, int rangeInDays)
        {
            //date + range > now
            return DateTime.Now.CompareTo(date.AddDays(rangeInDays)) < 0;
        }
    }
}
