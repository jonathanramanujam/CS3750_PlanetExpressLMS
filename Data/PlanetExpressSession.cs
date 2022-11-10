using CS3750_PlanetExpressLMS.Models;
using Microsoft.AspNetCore.Http;
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

        public List<Course> GetCourses()
        {
            if (httpContext.Session.GetString("courses") == null)
            {
                return null;
            }
            else
            {
                return GetValue<List<Course>>("courses");
            }
        }

        public List<Course> GetAllCourses()
        {
            if (httpContext.Session.GetString("allCourses") == null)
            {
                return null;
            }
            else
            {
                return GetValue<List<Course>>("allCourses");
            }
        }

        public List<Assignment> GetAssignments()
        {
            if (httpContext.Session.GetString("assignments") == null)
            {
                return null;
            }
            else
            {
                return GetValue<List<Assignment>>("assignments");
            }
        }

        public List<Invoice> GetInvoices()
        {
            if (httpContext.Session.GetString("invoices") == null)
            {
                return null;
            }
            else
            {
                return GetValue<List<Invoice>>("invoices");
            }
        }

        public List<Enrollment> GetEnrollments()
        {
            if (httpContext.Session.GetString("enrollments") == null)
            {
                return null;
            }
            else
            {
                return GetValue<List<Enrollment>>("enrollments");
            }
        }

        public List<Submission> GetSubmissions()
        {
            if (httpContext.Session.GetString("submissions") == null)
            {
                return null;
            }
            else
            {
                return GetValue<List<Submission>>("submissions");
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

        public void SetCourses(List<Course> courses)
        {
            SetValue("courses", courses);
        }

        public void SetAllCourses(List<Course> courses)
        {
            SetValue("allCourses", courses);
        }

        public void SetAssignments(List<Assignment> assignments)
        {
            SetValue("assignments", assignments);
        }

        public void SetInvoices(List<Invoice> invoices)
        {
            SetValue("invoices", invoices);
        }

        public void SetEnrollments(List<Enrollment> enrollments)
        {
            SetValue("enrollments", enrollments);
        }

        public void SetSubmissions(List<Submission> submissions)
        {
            SetValue("submissions", submissions);
        }

        #endregion
    }
}
