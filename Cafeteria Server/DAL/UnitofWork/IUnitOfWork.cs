using CafeteriaServer.DAL.Repositories;
using CafeteriaServer.Repositories;
using System;

namespace CafeteriaServer.UnitofWork
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IMenuItemRepository MenuItems { get; }
        IFeedbackRepository Feedbacks { get; }

        void Save();
    }
}
