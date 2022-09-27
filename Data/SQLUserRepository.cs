using CS3750_PlanetExpressLMS.Models;
using System.Collections.Generic;

namespace CS3750_PlanetExpressLMS.Data
{
    public class SQLUserRepository : IUserRepository
    {
        public readonly CS3750_PlanetExpressLMSContext context;

        public SQLUserRepository(CS3750_PlanetExpressLMSContext context)
        {
            this.context = context;
        }

        public User Add(User newUser)
        {
            context.Users.Add(newUser);
            context.SaveChanges();
            return newUser;
        }

        public User Delete(int id)
        {
            User user = context.Users.Find(id);
            if(user != null)
            {
                context.Users.Remove(user);
                context.SaveChanges();
            }
            return user;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return context.Users;
        }

        public User GetUser(int id)
        {
            return context.Users.Find(id);
        }

        public User Update(User updatedUser)
        {
            var user = context.Users.Attach(updatedUser);
            user.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return updatedUser;
        }
    }
}
