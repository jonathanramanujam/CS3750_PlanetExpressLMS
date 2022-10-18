using CS3750_PlanetExpressLMS.Models;
using Microsoft.AspNetCore.Http;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;

namespace CS3750_PlanetExpressLMS.Data
{
    public class PlanetExpressSession
    {
        private HttpContext httpContext;

        public PlanetExpressSession(HttpContext httpContext)
        {
            this.httpContext = httpContext;
        }

        public User GetUser()
        {
            if (httpContext.Session.GetString("user") == null)
            {
                return null;
            }
            else
            {
                return JsonSerializer.Deserialize<User>(httpContext.Session.GetString("user"));
            }
        }

        public IEnumerable<Course> GetCourses()
        {
            return JsonSerializer.Deserialize<IEnumerable<Course>>(httpContext.Session.GetString("courses"));
        }

        public IEnumerable<Assignment> GetAssignments()
        {
            return JsonSerializer.Deserialize<IEnumerable<Assignment>>(httpContext.Session.GetString("assignments"));
        }

        public void SetValue<T>(string key, T value)
        {
            httpContext.Session.SetString(key, JsonSerializer.Serialize(value));
        }

        public void SetUser(User user)
        {
            SetValue<User>("user", user);
        }

        public void SetCourses(IEnumerable<Course> courses)
        {
            SetValue<IEnumerable<Course>>("courses", courses);
        }

        public void SetAssignments(IEnumerable<Assignment> assignments)
        {
            SetValue<IEnumerable<Assignment>>("assignments", assignments);
        }

        public void Update(HttpContext httpContext)
        {

        }
    }
}
