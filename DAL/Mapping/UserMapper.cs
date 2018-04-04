using DAL.Entities;
using DAL.Mapping.Interface;
using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Mapping
{
    public class UserMapper : IUserMapper
    {
        public DalUser MapToDal(User entity)
        {
            return new DalUser
            {
                Id = entity.id,
                Login = entity.login,
                Password = entity.password,
                Fullname = entity.fullName,
                Group_id = entity.group_id,
                EventTypeLib_id = entity.eventTypeLib_id,
                StatusLib_id = entity.statusLib_id,
                SignInDate = entity.signInDate,
                RightToApprove = entity.rightToApprove
            };
        }

        public User MapToOrm(DalUser entity)
        {
            return new User
            {
                id = entity.Id,
                login = entity.Login,
                password = entity.Password,
                fullName = entity.Fullname,
                group_id = entity.Group_id,
                eventTypeLib_id = entity.EventTypeLib_id,
                statusLib_id = entity.StatusLib_id,
                signInDate = entity.SignInDate,
                rightToApprove = entity.RightToApprove
            };
        }
    }
}
