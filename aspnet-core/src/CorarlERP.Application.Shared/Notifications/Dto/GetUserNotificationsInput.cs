using Abp.Notifications;
using CorarlERP.Dto;

namespace CorarlERP.Notifications.Dto
{
    public class GetUserNotificationsInput : PagedInputDto
    {
        public UserNotificationState? State { get; set; }
    }
}