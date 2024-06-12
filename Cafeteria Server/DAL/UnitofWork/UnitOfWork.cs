using CafeteriaServer.Context;
using CafeteriaServer.DAL.Repositories;
using CafeteriaServer.Repositories;
using System;

namespace CafeteriaServer.UnitofWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CafeteriaContext _context;

        public UnitOfWork(CafeteriaContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            MenuItems = new MenuItemRepository(_context);
            Feedbacks = new FeedbackRepository(_context);
        }

        public IUserRepository Users { get; }
        public IMenuItemRepository MenuItems { get; }
        public IFeedbackRepository Feedbacks { get; }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
