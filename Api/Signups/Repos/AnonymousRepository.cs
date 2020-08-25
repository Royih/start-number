
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using SendGrid;
using SendGrid.Helpers.Mail;
using Signup.API.Common;
using Signup.API.Dtos;
using Signup.API.Models;
using Signup.API.Signups;
using Signup.API.Signups.Dtos;

namespace Signup.API.Users.Repos
{
    public class AnonymousRepository : IAnonymousRepository
    {

        private readonly IDb _db;
        private readonly IHttpContextAccessor _ctx;
        private readonly IConfiguration _configuration;
        private readonly IConverter _pdfGenerator;




        public AnonymousRepository(IDb db, IHttpContextAccessor ctx, IConfiguration configuration, IConverter pdfGenerator)
        {
            _db = db;
            _ctx = ctx;
            _configuration = configuration;
            _pdfGenerator = pdfGenerator;
        }

        public async Task<EventDataDto> GetEventData(string key)
        {
            var tenant = await (await _db.Tenants.FindAsync(x => x.Key == key)).SingleAsync();
            var currentEvent = await (await _db.Events.FindAsync(x => x.Id == tenant.CurrentlyActiveEventId)).SingleAsync();
            return new EventDataDto { TenantName = tenant.Name, TenantLogo = tenant.Base64EncodedLogo };
        }

        public async Task<IEnumerable<ActiveEventDto>> ListActiveEvents()
        {
            var activeTenants = (await _db.Tenants.FindAsync(x => !string.IsNullOrEmpty(x.CurrentlyActiveEventId))).ToEnumerable().ToList();
            var ids = activeTenants.Select(x => x.CurrentlyActiveEventId);
            var currentEvents = (await _db.Events.FindAsync(x => ids.Contains(x.Id))).ToEnumerable();
            return activeTenants.Select(x => new ActiveEventDto { TenantKey = x.Key, Name = currentEvents.Single(y => y.Id == x.CurrentlyActiveEventId).Name, EventId = x.CurrentlyActiveEventId, Logo = x.Base64EncodedLogo });
        }

        public async Task<CommandResultDto<string>> SignUp(SignUpDto signUpData)
        {
            try
            {
                var tenant = await (await _db.Tenants.FindAsync(x => x.Key == signUpData.TenantKey)).SingleAsync();
                var currentEvent = await (await _db.Events.FindAsync(x => x.Id == tenant.CurrentlyActiveEventId)).SingleAsync();
                var compareEmail = signUpData.Email.ToLower().Trim();
                var compareFirstName = signUpData.FirstName.ToLower().Trim();
                var compareSurName = signUpData.SurName.ToLower().Trim();
                var existingPerson = await (await _db.Persons.FindAsync(x => x.TenantId == tenant.Id &&
                                                                                x.Email == compareEmail &&
                                                                                x.FirstName.ToLower() == compareFirstName &&
                                                                                x.SurName.ToLower() == compareSurName)).SingleOrDefaultAsync();
                if (existingPerson == null)
                {
                    await _db.Persons.InsertOneAsync(new Person
                    {
                        TenantId = tenant.Id,
                        FirstName = signUpData.FirstName.Trim(),
                        SurName = signUpData.SurName.Trim(),
                        Email = compareEmail,
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
                    await _db.Events.FindOneAndUpdateAsync(x => x.Id == currentEvent.Id && x.Signups.Length == currentEvent.Signups.Length, finalUpd);
                }
                var newlyReservedStartNumber = currentEvent.Signups.Length + 1;

                var signupRecord = new SignupRecord
                {
                    EventId = currentEvent.Id,
                    FirstName = signUpData.FirstName.Trim(),
                    SurName = signUpData.SurName.Trim(),
                    Email = signUpData.Email.Trim(),
                    AllowUsToContactPersonByEmail = signUpData.AllowUsToContactPersonByEmail,
                    PreviouslyParticipated = signUpData.PreviouslyParticipated,
                    IPAddress = _ctx.HttpContext.Connection.RemoteIpAddress.ToString(),
                    SignupUTC = DateTime.UtcNow,
                    PersonId = existingPerson.Id,
                    ActualStartNumber = newlyReservedStartNumber
                };
                await _db.SignupRecords.InsertOneAsync(signupRecord);

                var startNumberPdf = GetStartNumberPdf(newlyReservedStartNumber);
                await SendEmail(currentEvent, signupRecord, startNumberPdf);

                return new CommandResultDto<string> { Success = true, Data = signupRecord.Id };
            }
            catch (Exception ex)
            {
                return new CommandResultDto<string> { Success = false, ErrorMessages = new[] { ex.Message }, Data = null };
            }

        }

        public async Task<byte[]> GetStartNumberPdf(string eventId, string personId)
        {
            var ev = await (await _db.Events.FindAsync(x => x.Id == eventId)).SingleAsync();
            var person = await (await _db.Persons.FindAsync(x => x.Id == personId)).SingleAsync();
            var idArr = ev.Signups.Select(x => x.PersonId).ToArray();
            var startNumber = Array.IndexOf(idArr, person.Id) + 1;
            return GetStartNumberPdf(startNumber);
        }

        public byte[] GetStartNumberPdf(SignupRecord signup)
        {
            return GetStartNumberPdf(signup.ActualStartNumber);
        }

        private byte[] GetStartNumberPdf(int startNumber)
        {

            var year = DateTime.Now.Year;

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    DPI = 380,
                },
                Objects = {
                    new ObjectSettings() {
                        HtmlContent = $@"
                                <table cellspacing='0' cellpadding='0' border=1 style='font-family: Arial, Helvetica, sans-serif; width:100%'>
                                        <tr>
                                            <td style='text-align:center;padding:0;margin:0'>
                                                <img alt='Embedded Image' style='max-width:100%;max-height:100%; margin:0' src='{Constants.DefaultBase64EncodedLogo}'>
                                            </td>
                                        </tr>
                                         <tr>                                                
                                            <td style='font-size: 200px; font-weight: bolder; text-align:center; font-family: Arial, Helvetica, sans-serif; padding:0;margin:0'>
                                                {startNumber.ToString().PadLeft(3, '0')}
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style='text-align:center; padding:0;margin:0'>
                                                <div style='margin-top: 25px; border: 1px solid #bbb;'>
                                                    <img alt='Embedded Image' style='max-width:100%;max-height:100%;' src='{Constants.DefaultBase64EncodedSponsorStripe}'>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>",
                                           WebSettings = { DefaultEncoding  ="utf-8" }
                    }
                }
            };
            return _pdfGenerator.Convert(doc);
        }



        private async Task SendEmail(Event ev, SignupRecord signupRecord, byte[] pdfAsBytes)
        {
            var client = new SendGridClient(_configuration.GetValue<string>(Constants.AppSettingSendGridApiKey));
            var from = new EmailAddress("ingunn.vaer@gmail.com", "Ingunn");
            var subject = "Your start number for " + ev.Name;
            var to = new EmailAddress(signupRecord.Email, signupRecord.FirstName + " " + signupRecord.SurName);
            var plainTextContent = "Your start number for " + ev.Name + " attached.";
            var htmlContent = "Your start number for <strong>" + ev.Name + "</strong> attached. ";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var ms = new MemoryStream(pdfAsBytes);
            await msg.AddAttachmentAsync(signupRecord.GetStartNumberPdfFileName(), contentStream: ms);
            var response = await client.SendEmailAsync(msg);
            var bodyAsString = await response.Body.ReadAsStringAsync();
            var listOfUpdates = new List<UpdateDefinition<SignupRecord>>();
            listOfUpdates.Add(Builders<SignupRecord>.Update.Set(x => x.EmailSendStatusCode, (int)response.StatusCode));
            listOfUpdates.Add(Builders<SignupRecord>.Update.Set(x => x.EmailSendResponseBody, bodyAsString));
            var finalUpd = Builders<SignupRecord>.Update.Combine(listOfUpdates);
            await _db.SignupRecords.FindOneAndUpdateAsync(x => x.Id == signupRecord.Id, finalUpd);
        }

        public async Task<SignupRecord> GetSignup(string signupId)
        {
            return await (await _db.SignupRecords.FindAsync(x => x.Id == signupId)).SingleAsync();
        }


    }
}