
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
        private readonly IConverter _pdfGenerator;


        public SignupRepository(IDb db, IConverter pdfGenerator)
        {
            _db = db;
            _pdfGenerator = pdfGenerator;
        }


        public async Task<byte[]> GetStartNumberPdf(string eventId, string personId)
        {
            var ev = await (await _db.Events.FindAsync(x => x.Id == eventId)).SingleAsync();
            var person = await (await _db.Persons.FindAsync(x => x.Id == personId)).SingleAsync();
            var idArr = ev.Signups.Select(x => x.PersonId).ToArray();
            var startNumber = Array.IndexOf(idArr, person.Id) + 1;
            var year = DateTime.Now.Year;

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                },
                Objects = {
                    new ObjectSettings() {
                        HtmlContent = $@"<table border=0 style='width: 100%; font-family: Helvetica'>
                                            <tr>
                                                <td colspan='3' style='text-align:center;'>
                                                    <img alt='Embedded Image' style='height: 300px' src='{Constants.DefaultBase64EncodedLogo}'>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style='width:10%'>&nbsp;</td>
                                                <td style='font-size: 450px; font-weight: bolder; text-align:center; font-family: Arial, Helvetica, sans-serif'>
                                                    {("00"+startNumber).PadRight(3)}
                                                </td>
                                                <td style='width:20%; font-size: 150px; line-height: 90%; text-align: center; font-weight: bold; border: 10px solid #000; padding: 15px'>{string.Join("<BR />",year.ToString().ToCharArray())}</td>
                                            </tr>
                                            <tr>
                                                <td colspan='3'>
                                                    <div style='margin-top: 25px; border: 1px solid #bbb; padding: 10px; min-height: 100px; background-color: #eee; font-size: 50px;'>
                                                        Sponsors...
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

        public async Task<IEnumerable<SignUpsForEventDto>> ListSignups(string eventId)
        {
            var ev = await (await _db.Events.FindAsync(x => x.Id == eventId)).SingleAsync();
            var idArr = ev.Signups.Select(x => x.PersonId).ToArray();
            var persons = (await _db.Persons.FindAsync(x => idArr.Contains(x.Id))).ToEnumerable();
            return persons.Select(x => new SignUpsForEventDto { PersonId = x.Id, FirstName = x.FirstName, SurName = x.SurName, Email = x.Email, AllowUsToContactPersonByEmail = x.AllowUsToContactPersonByEmail, StartNumber = Array.IndexOf(idArr, x.Id) + 1 });
        }


    }
}