using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using DijitalEvrakTakip.Persistance.Context;
using GenericRepository;

namespace DijitalEvrakTakip.Persistance.Repositories;

public sealed class NotificationRepository
    : Repository<Notification, AppDbContext>,
      INotificationRepository
{
    public NotificationRepository(AppDbContext context)
        : base(context)
    {
    }
}
