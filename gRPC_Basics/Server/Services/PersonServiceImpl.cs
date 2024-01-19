using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Extensions.Msal;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PersonService;

namespace Server.Services
{
    public class PersonServiceImpl : PersonServiceBase
    {
        public override async Task<Person> CreatePerson(CreatePersonRequest request, ServerCallContext context)
        {
            using var db = new ServerDbContext();
            var dbPerson = new Data.Model.DbPerson { Fname = request.FName, Lname = request.LName };
            db.People.Add(dbPerson);
            await db.SaveChangesAsync();
            return new Person { FName = request.FName, LName = request.LName, Id = dbPerson.Id };
        }
        public override async Task<GetAllResponse> GetAll(GetAllRequest request, ServerCallContext context)
        {
            using var db = new ServerDbContext();
            var persons = await db.People.ToListAsync();
            var response = new GetAllResponse();
            response.People.AddRange(persons.ConvertAll(p => new Person
            {
                Id = p.Id,
                FName = p.Fname,
                LName = p.Lname
            }));
            return response;
        }
        public override async Task GetByPage(IAsyncStreamReader<GetByPageRequest> requestStream, IServerStreamWriter<GetByPageResponse> responseStream, ServerCallContext context)
        {
            using var db = new ServerDbContext();
            while (await requestStream.MoveNext())
            {
                int page = Math.Max(requestStream.Current.Page, 1);
                var persons = await db.People
                    .OrderBy(p => p.Id)
                    .Skip((page - 1) * 15)
                    .Take(15)
                    .ToListAsync();
                var result = new GetByPageResponse { Page = page };
                result.People.AddRange(persons.Select(p => new Person { Id = p.Id, FName = p.Fname, LName = p.Lname }));
                await responseStream.WriteAsync(result);
            }
        }
    }
}
