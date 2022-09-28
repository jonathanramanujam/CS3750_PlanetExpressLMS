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
            context.User.Add(newUser);
            context.SaveChanges();
            return newUser;
        }

        public User Delete(int id)
        {
            User user = context.User.Find(id);
            if(user != null)
            {
                context.User.Remove(user);
                context.SaveChanges();
            }
            return user;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return context.User;
        }

        public User GetUser(int id)
        {
            IEnumerable<User> users = context.User;
            
            return context.User.Find(id);
        }

        public User Update(User updatedUser)
        {
            context.User.Attach(updatedUser);
            //Decide which fields to update
            context.Entry(updatedUser).Property("FirstName").IsModified = true;
            context.Entry(updatedUser).Property("LastName").IsModified = true;
            if (updatedUser.Image != null)
            {
                context.Entry(updatedUser).Property("Image").IsModified = true;
            }
            context.Entry(updatedUser).Property("Bio").IsModified = true;
            context.SaveChanges();
            //'Reset' DbContext so we don't get updatedUser returned, which has missing information
            context.Entry(updatedUser).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            var user = context.User.Find(updatedUser.ID);
            return user;
        }
    }
}
