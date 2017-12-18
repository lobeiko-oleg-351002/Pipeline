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
                ChangeRights = entity.changeRights,
                Login = entity.login,
                CreateRights = entity.createRights,
                Password = entity.password,
                Fullname = entity.fullName,
                Group_id = entity.group_id
            };
        }

        public User MapToOrm(DalUser entity)
        {
            return new User
            {
                id = entity.Id,
                createRights = entity.CreateRights,
                login = entity.Login,
                changeRights = entity.ChangeRights,
                password = entity.Password,
                fullName = entity.Fullname,
                group_id = entity.Group_id
            };
        }
    }
}
