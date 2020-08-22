
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using Signup.API.Common;
using Signup.API.Dtos;
using Signup.API.Models;

namespace Signup.API.Users.Repos
{
    public class AnonymousRepository : IAnonymousRepository
    {

        private readonly IDb _db;
        private readonly IHttpContextAccessor _ctx;


        public AnonymousRepository(IDb db, IHttpContextAccessor ctx)
        {
            _db = db;
            _ctx = ctx;
        }

        public async Task<IEnumerable<ActiveEventDto>> ListActiveEvents()
        {
            var activeTenants = (await _db.Tenants.FindAsync(x => !string.IsNullOrEmpty(x.CurrentlyActiveEventId))).ToEnumerable().ToList();
            var ids = activeTenants.Select(x => x.CurrentlyActiveEventId);
            return (await _db.Events.FindAsync(x => ids.Contains(x.Id))).ToEnumerable().Select(x => new ActiveEventDto { TenantKey = activeTenants.Single(y => y.Id == x.TenantId).Key, Name = x.Name });
        }

        public async Task<CommandResultDto> SignUp(SignUpDto signUpData)
        {
            try
            {
                var tenant = await (await _db.Tenants.FindAsync(x => x.Key == signUpData.TenantKey)).SingleAsync();
                var currentEvent = await (await _db.Events.FindAsync(x => x.Id == tenant.CurrentlyActiveEventId)).SingleAsync();
                var compareEmail = signUpData.Email.ToLower().Trim();
                var compareFirstName = signUpData.FirstName.ToLower().Trim();
                var compareSurName = signUpData.SurName.ToLower().Trim();
                var existingPerson = await (await _db.Persons.FindAsync(x => x.TenantId == tenant.Id &&
                                                                                x.EMail == compareEmail &&
                                                                                x.FirstName.ToLower() == compareFirstName &&
                                                                                x.SurName.ToLower() == compareSurName)).SingleOrDefaultAsync();
                if (existingPerson == null)
                {
                    await _db.Persons.InsertOneAsync(new Person
                    {
                        TenantId = tenant.Id,
                        FirstName = signUpData.FirstName.Trim(),
                        SurName = signUpData.SurName.Trim(),
                        EMail = compareEmail,
                        AllowUsToContactPersonByEmail = signUpData.AllowUsToContactPersonByEmail
                    });
                    existingPerson = _db.Persons.Find(x => x.TenantId == tenant.Id && x.FirstName == signUpData.FirstName.Trim() && x.SurName == signUpData.SurName.Trim()).Single();
                }
                else
                {
                    var listOfUpdates = new List<UpdateDefinition<Person>>();
                    listOfUpdates.Add(Builders<Person>.Update.Set(x => x.AllowUsToContactPersonByEmail, signUpData.AllowUsToContactPersonByEmail));
                    var finalUpd = Builders<Person>.Update.Combine(listOfUpdates);
                    await _db.Persons.UpdateOneAsync(x => x.Id == existingPerson.Id, finalUpd, new UpdateOptions { IsUpsert = true });
                }

                if (!currentEvent.Signups.Any(x => x.PersonId == existingPerson.Id))
                {
                    var newSignupsArray = currentEvent.Signups.Concat(new[] { new EventSignup { PersonId = existingPerson.Id, PersonName = existingPerson.FirstName + " " + existingPerson.SurName } });
                    var listOfUpdates = new List<UpdateDefinition<Event>>();
                    listOfUpdates.Add(Builders<Event>.Update.Set(x => x.Signups, newSignupsArray));
                    var finalUpd = Builders<Event>.Update.Combine(listOfUpdates);
                    await _db.Events.UpdateOneAsync(x => x.Id == currentEvent.Id, finalUpd, new UpdateOptions { IsUpsert = true });
                }

                await _db.SignupRecords.InsertOneAsync(new SignupRecord
                {
                    FirstName = signUpData.FirstName.Trim(),
                    SurName = signUpData.SurName.Trim(),
                    Email = signUpData.Email.Trim(),
                    AllowUsToContactPersonByEmail = signUpData.AllowUsToContactPersonByEmail,
                    PreviouslyParticipated = signUpData.PreviouslyParticipated,
                    IPAddress = _ctx.HttpContext.Connection.RemoteIpAddress.ToString(),
                    SignupUTC = DateTime.UtcNow,
                    PersonId = existingPerson.Id
                });

                return new CommandResultDto { Success = true };
            }
            catch (Exception ex)
            {
                return new CommandResultDto { Success = false, ErrorMessages = new[] { ex.Message } };
            }

        }
    }
}