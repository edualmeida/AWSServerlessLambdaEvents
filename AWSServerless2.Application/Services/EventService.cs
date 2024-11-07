using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using AWSServerless2.Domain.Models;
using Microsoft.Extensions.Logging;

namespace AWSServerless2.Application.Services
{
    public class EventService : IEventService
    {
        private readonly IAmazonDynamoDB _amazonDynamoDB;
        private readonly DynamoDBContext _context;
        private readonly string _tableName = "Event";
        private readonly ILogger<EventService> _logger;

        public EventService(IAmazonDynamoDB amazonDynamoDB, ILogger<EventService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _amazonDynamoDB = amazonDynamoDB ?? throw new ArgumentNullException(nameof(amazonDynamoDB));

            try
            {
                _context = new DynamoDBContext(_amazonDynamoDB);
            }
            catch (Exception ex)
            {
                //LambdaLogger.Log("ENVIRONMENT VARIABLES: " + JsonConvert.SerializeObject(System.Environment.GetEnvironmentVariables()));
                //LambdaLogger.Log("CONTEXT: " + JsonConvert.SerializeObject(_context));
                //LambdaLogger.Log("EVENT: " + JsonConvert.SerializeObject(invocationEvent));
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        public async Task CreateAsync(Event request)
        {
            Table eventsTable = Table.LoadTable(_amazonDynamoDB, _tableName);
            var document = new Document();

            document["Id"] = request.Id;
            document["Title"] = request.Title;
            document["Description"] = request.Description;
            document["Start"] = request.Start;
            document["End"] = request.End;
            document["AllDay"] = request.AllDay;

            await eventsTable.PutItemAsync(document);
        }

        public async Task DeleteAsync(string id)
        {
            await _context.DeleteAsync<Event>(id);
        }

        public async Task<IList<Event>> GetAllAsync()
        {
            var table = _context.GetTargetTable<Event>();
            var scanOps = new ScanOperationConfig();

            var results = table.Scan(scanOps);
            List<Document> data = await results.GetNextSetAsync();

            return _context.FromDocuments<Event>(data)?.OrderBy(x=>x.Start).ToList() ?? new List<Event>();
        }

        public async Task<IList<Event>> GetByDateRangeAsync(DateTime start, DateTime end)
        {
            var scanConditions = new List<ScanCondition>();
            scanConditions.Add(new ScanCondition("Start", ScanOperator.GreaterThanOrEqual, start));
            scanConditions.Add(new ScanCondition("End", ScanOperator.LessThanOrEqual, end));

            var queryResult = await _context.ScanAsync<Event>(scanConditions,
                    new DynamoDBOperationConfig() { }).GetRemainingAsync();

            return queryResult;
        }

        public async Task UpdateAsync(string id, Event request)
        {
            var dbRecord = await _context.LoadAsync<Event>(id); //.QueryAsync<Employee>(EmployeeId.ToString()).GetRemainingAsync();

            dbRecord.AllDay = request.AllDay;
            dbRecord.Description = request.Description;
            dbRecord.End = request.End;
            dbRecord.Start = request.Start;
            dbRecord.Title = request.Title;

            await _context.SaveAsync<Event>(dbRecord);
        }

        public async Task<EventList> GetListByDateRangeAsync(DateTime start, DateTime end)
        {
            var scanConditions = new List<ScanCondition>();
            scanConditions.Add(new ScanCondition("Start", ScanOperator.GreaterThanOrEqual, start));
            scanConditions.Add(new ScanCondition("End", ScanOperator.LessThanOrEqual, end));

            var queryResult = await _context.ScanAsync<Event>(scanConditions,
                    new DynamoDBOperationConfig() { }).GetRemainingAsync();

            return new EventList()
            {
                Events = queryResult
            };
        }
    }
}
