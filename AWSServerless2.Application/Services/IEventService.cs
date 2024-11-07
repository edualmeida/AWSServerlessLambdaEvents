using AWSServerless2.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSServerless2.Application.Services
{
    public interface IEventService
    {
        Task<IList<Event>> GetAllAsync();
        Task<IList<Event>> GetByDateRangeAsync(DateTime start, DateTime end);
        Task CreateAsync(Event request);
        Task UpdateAsync(string id, Event request);
        Task DeleteAsync(string id);
        Task<EventList> GetListByDateRangeAsync(DateTime start, DateTime end);
    }
}
