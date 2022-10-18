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

        #region Getters

        public T GetValue<T>(string key)
        {
            return JsonSerializer.Deserialize<T>(httpContext.Session.GetString(key));
        }

        public User GetUser()
        {
            if (httpContext.Session.GetString("user") == null)
            {
                return null;
            }
            else
            {
                return GetValue<User>("user");
            }
        }

        public IEnumerable<Course> GetCourses()
        {
            if (httpContext.Session.GetString("courses") == null)
            {
                return null;
            }
            else
            {
                return GetValue<IEnumerable<Course>>("courses");
            }
        }

        public IEnumerable<Course> GetAllCourses()
        {
            if (httpContext.Session.GetString("allCourses") == null)
            {
                return null;
            }
            else
            {
                return GetValue<IEnumerable<Course>>("allCourses");
            }
        }

        public IEnumerable<Assignment> GetAssignments()
        {
            if (httpContext.Session.GetString("assignments") == null)
            {
                return null;
            }
            else
            {
                return GetValue<IEnumerable<Assignment>>("assignments");
            }
        }

        public IEnumerable<Invoice> GetInvoices()
        {
            if (httpContext.Session.GetString("invoices") == null)
            {
                return null;
            }
            else
            {
                return GetValue<IEnumerable<Invoice>>("invoices");
            }
        }

        public IEnumerable<Enrollment> GetEnrollments()
        {
            if (httpContext.Session.GetString("enrollments") == null)
            {
                return null;
            }
            else
            {
                return GetValue<IEnumerable<Enrollment>>("enrollments");
            }
        }

        #endregion


        #region Setters

        public void SetValue<T>(string key, T value)
        {
            httpContext.Session.SetString(key, JsonSerializer.Serialize(value));
        }        

        public void SetUser(User user)
        {
            SetValue("user", user);
        }

        public void SetCourses(IEnumerable<Course> courses)
        {
            SetValue("courses", courses);
        }

        public void SetAllCourses(IEnumerable<Course> courses)
        {
            SetValue("allCourses", courses);
        }

        public void SetAssignments(IEnumerable<Assignment> assignments)
        {
            SetValue("assignments", assignments);
        }

        public void SetInvoices(IEnumerable<Invoice> invoices)
        {
            SetValue("invoices", invoices);
        }

        public void SetEnrollments(IEnumerable<Enrollment> enrollments)
        {
            SetValue("enrollments", enrollments);
        }

        #endregion
    }
}
