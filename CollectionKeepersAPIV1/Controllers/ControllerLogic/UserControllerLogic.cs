using CollectionKeepersAPIV1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace CollectionKeepersAPIV1.Controllers.ControllerLogic
{
    public class UserControllerLogic
    {
        public CollectionsDbContext ctx;

        public UserControllerLogic(CollectionsDbContext ctx)
        {
            this.ctx = ctx; 
        }

        public  List<TblUser> GetAllUsers()
        {
            List<TblUser> users = ctx.TblUsers.ToList();
            return users;
        }

        public string CreateNewUser(TblUser NewUser)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(NewUser.FldEmail, emailPattern))
            {
                return "Invalid email";
            }
            string Email = NewUser.FldEmail;

            //TODO: Check that the email id doesn't already exist in the db before adding a new user with that email
            List<TblUser> QueriedUsers = ctx.TblUsers.Where(User => User.FldEmail == Email).ToList();

            if (QueriedUsers.Count != 0)
            {
                return "Email ID already in DB";
            }

            ctx.TblUsers.Add(NewUser);
            ctx.SaveChanges();
            return "NewUser Posted";
        }

        public object Login(TblUser User)
        {
            string Email = User.FldEmail;
            string Password = User.FldPassword;
            string ErrorMessage = "Invalid credentials entered";

            List<TblUser> QueriedUsers = ctx.TblUsers.Where(User => User.FldEmail == Email).ToList();

            if (QueriedUsers.Count == 0)
            {
                return ErrorMessage;
            }

            TblUser QueriedUser = QueriedUsers.First();

            if (Password == QueriedUser.FldPassword)
            {
                return QueriedUser;
            }
            else
            {
                return ErrorMessage;
            }
        }
    }
}
