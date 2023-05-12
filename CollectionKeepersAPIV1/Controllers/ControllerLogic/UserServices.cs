using CollectionKeepersAPIV1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace CollectionKeepersAPIV1.Controllers.ControllerLogic
{
    public class UserServices
    {
        public CollectionsDbContext ctx;

        public UserServices(CollectionsDbContext ctx)
        {
            this.ctx = ctx; 
        }

        public  List<TblUser> GetAllUsers()
        {
            List<TblUser> users = ctx.TblUsers.ToList();
            return users;
        }

        public List<TblUser> GetUsersOnEmail(string Email) 
        {
            List<TblUser> QueriedUsers = ctx.TblUsers.Where(User => User.FldEmail == Email).ToList();
            return QueriedUsers;
        }

        public List<TblUser> GetUsersOnID(int ID)
        {
            List<TblUser> QueriedUsers = ctx.TblUsers.Where(User => User.FldUserId == ID).ToList();
            return QueriedUsers;
        }

        public void AddUserToDB(TblUser NewUser) 
        {
            ctx.TblUsers.Add(NewUser);
            ctx.SaveChanges();
        }

        public void UpdateUserDetails(TblUser FoundUser, TblUser NewUserDetails)
        {
            //Update all details for this account
            FoundUser.FldUsername = NewUserDetails.FldUsername;
            FoundUser.FldPassword = NewUserDetails.FldPassword;
            FoundUser.FldEmail = NewUserDetails.FldEmail;

            //Save the changes
            ctx.SaveChanges();
        }
      
    }
}
