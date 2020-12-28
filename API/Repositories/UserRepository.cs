using API.Data;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repositories
{
    public class UserRepository : BaseRepository<User>
    {
        public UserRepository(DataContext context) : base(context) 
        {

        }

        public async Task<bool> UserExistAsync(string username)
        {
            return await this.ExistAsync(x => x.UserName == username.ToLower());
        }

        public async Task<User> FindUserAsync(string Username) {
            return await myEntity.SingleOrDefaultAsync(x=>x.UserName == Username.ToLower());
        }
    }
}
