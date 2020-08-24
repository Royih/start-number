
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using MongoDB.Driver;
using Signup.API.Common;
using Signup.API.Signups.Dtos;

namespace Signup.API.Users.Repos
{
    public class SignupRepository : ISignupRepository
    {

        private readonly IDb _db;
       

        public SignupRepository(IDb db)
        {
            _db = db;
        }


        public async Task<IEnumerable<SignUpsForEventDto>> ListSignups(string eventId)
        {
            var ev = await (await _db.Events.FindAsync(x => x.Id == eventId)).SingleAsync();
            var idArr = ev.Signups.Select(x => x.PersonId).ToArray();
            var persons = (await _db.Persons.FindAsync(x => idArr.Contains(x.Id))).ToEnumerable();
            return persons.Select(x => new SignUpsForEventDto { PersonId = x.Id, FirstName = x.FirstName, SurName = x.SurName, Email = x.Email, AllowUsToContactPersonByEmail = x.AllowUsToContactPersonByEmail, StartNumber = Array.IndexOf(idArr, x.Id) + 1 });
        }


    }
}