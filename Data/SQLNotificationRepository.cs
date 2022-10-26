using CS3750_PlanetExpressLMS.Models;
using System.Collections.Generic;
using System.Linq;

namespace CS3750_PlanetExpressLMS.Data
{
    public class SQLNotificationRepository : INotificationRepository
    {
        public readonly CS3750_PlanetExpressLMSContext context;
        public SQLNotificationRepository(CS3750_PlanetExpressLMSContext context)
        {
            this.context = context;
        }
        public Notification Add(Notification newNotification)
        {
            context.Notification.Add(newNotification);
            context.SaveChanges();
            return newNotification;
        }
        public IEnumerable<Notification> GetAllNotifications()
        {
            return context.Notification;
        }
        public List<Notification> GetNotifications(int id)
        {
            var userNotification = GetAllNotifications();
            userNotification = userNotification.Where(c => c.ID == id);
            return userNotification.ToList<Notification>();
        }
        public Notification Delete(int id)
        {
            Notification no = context.Notification.Find(id);
            if (no != null)
            {
                context.Notification.Remove(no);
                context.SaveChanges();
            }
            return no;
        }
    }
}
