using DirectCareConnect.Common.Models.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DirectCareConnect.Common.Impl.Communication.DDezModels
{
    public partial class ProviderConfiguration
    {
        [JsonProperty("er")]
        public Er Er { get; set; }

        [JsonProperty("providerId")]
        public long ProviderId { get; set; }

        [JsonProperty("clients")]
        public List<Client> Clients { get; set; }

        [JsonProperty("clientAlerts")]
        public List<ClientAlert> ClientAlerts { get; set; }

        [JsonProperty("credentials")]
        public List<Credential> Credentials { get; set; }

        [JsonProperty("atcScoring")]
        public List<Scoring> AtcScoring { get; set; }

        [JsonProperty("hahScoring")]
        public List<Scoring> HahScoring { get; set; }

        [JsonProperty("messagingContacts")]
        public List<MessagingContact> MessagingContacts { get; set; }

        [JsonProperty("pendingDocumentation")]
        public List<PendingDocumentation> PendingDocumentation { get; set; }

        [JsonProperty("companies")]
        public List<Company> Companies { get; set; }

        public InitialData ToInitialData()
        {
            InitialData data = GetInitialDataModel();
            
            var company = this.Companies.FirstOrDefault();
            data.Company = new Models.Db.Company
            {
                CompanyId = int.Parse(company.CompanyId),
                Name = company.Name,
                ProviderId = int.Parse(company.ProviderId)
            };
            foreach (var client in company.Clients)
            {
                data.Clients.Add(new Models.Db.Clients.Client
                {
                    ClientId = client.ClientId,
                    FirstName = client.ClientFirstName,
                    LastName = client.ClientLastName
                });

                if (client.Locations != null)
                {

                    foreach (var location in client.Locations)
                    {
                        data.Locations.Add(new Models.Db.Clients.Location
                        {
                            ClientId = client.ClientId,
                            Latitude = location.Lat,
                            Longitude = location.Lon,
                            ClientLocationId = location.ClientLocationId,
                            LocationTypeId = location.LocationTypeId,
                            RadiusInFeet = location.Radius,
                            Address=location.Address,
                            City=location.City,
                            State=location.State,
                            Zip=location.Zip
                        });
                    }
                }

                foreach (var service in client.Services)
                {
                    data.ClientServices.Add(new Models.Db.Clients.ClientService
                    {
                        ClientId = client.ClientId,

                        // xxxxxx switched  old was ClientServiceId = service.ServiceId,
                        ClientServiceId = service.ClientServiceId,
                        ServiceId = service.ServiceId,
                        ServiceName = service.Name.ToString(),
                        ServiceShortName = service.ShortName.ToString(),
                        NoteType = service.NoteType.ToString(),
                        IsHCBS = service.IsHCBS,
                        IsTherapy = service.IsTherapy,
                        IsEvaluation = service.IsEvaluation,
                        CleanNote = service.ClientNote,
                        WeeklyHours = GetWeeklyHours(service)
                    }) ;
                }

                if (client.Designees != null)
                {
                    foreach(var designee in client.Designees)
                    {
                        data.Designees.Add(new Models.Db.Clients.Designee
                        { 
                            DCCDesigneeId=(int)designee.DesigneeId,
                            DCCGuardianId = (int)designee.GuardianId,
                            Email =designee.Email,
                            FirstName=designee.FirstName,
                            IsPrimaryGuardian=designee.IsPrimaryGuardian,
                            LastName=designee.LastName,
                            Pin=designee.Pin,
                            ClientId=client.ClientId,
                            UniqueDesigneeId= (int)designee.DesigneeId + "-" + (int)designee.GuardianId
                        });
                    }
                }

            }
            foreach (var clientAlert in company.ClientAlerts)
            {

                data.ClientAlerts.Add(new Models.Db.Clients.ClientAlert
                {
                    Alert = clientAlert.Alert,
                    ClientId = clientAlert.ClientId,
                    Priority = clientAlert.Priority
                });
            }

            foreach (var credential in company.Credentials)
            {

                data.Credentials.Add(new Models.Db.Clients.Credential
                {
                    CoId = credential.CoId,
                    CredId = credential.CredId,
                    CredName = credential.CredName,
                    CredTypeId = credential.CredTypeId,
                    DocId = credential.DocId,
                    ProviderId = credential.ProviderId,
                    Status = credential.Status,
                    ValidFrom = credential.ValidFrom,
                    ValidTo = credential.ValidTo
                });
            }

            foreach (var pendingDocumentation in company.PendingDocumentation)
            {

                data.PendingDocumentation.Add(new Models.Db.Clients.PendingDocumentation
                {
                     ClientId=pendingDocumentation.ClientId,
                     ClientServiceId=pendingDocumentation.ClientServiceId,
                     DocumentId=int.Parse(pendingDocumentation.DocId),
                     Alert=pendingDocumentation.ClientName + " - " + pendingDocumentation.Svc,
                     Svc=pendingDocumentation.Svc.ToString()

                });
            }
            return data;
        }

        private double GetWeeklyHours(Service service)
        {
            if (service.Authorizations == null || service.Authorizations.Count == 0)
                return 0;

            return service.Authorizations.Sum(p => p.WeeklyHours);
        }

        private InitialData GetInitialDataModel()
        {
            InitialData data = new InitialData();
            data.Clients = new List<Models.Db.Clients.Client>();
            data.Locations = new List<Models.Db.Clients.Location>();
            data.ClientServices = new List<Models.Db.Clients.ClientService>();
            data.ClientAlerts = new List<Models.Db.Clients.ClientAlert>();
            data.Credentials = new List<Models.Db.Clients.Credential>();
            data.PendingDocumentation = new List<Models.Db.Clients.PendingDocumentation>();
            data.Designees = new List<Models.Db.Clients.Designee>();
            return data;
        }
    }

    public partial class Scoring
    {
        [JsonProperty("scoreValue")]
        public string ScoreValue { get; set; }

        [JsonProperty("scoreName")]
        public string ScoreName { get; set; }
    }

    public partial class ClientAlert
    {
        [JsonProperty("priority")]
        public long Priority { get; set; }

        [JsonProperty("alert")]
        public string Alert { get; set; }

        [JsonProperty("clientId")]
        public long ClientId { get; set; }
    }



    public partial class Client
    {
        [JsonProperty("clientId")]
        public int ClientId { get; set; }

        [JsonProperty("clientFirstName")]
        public string ClientFirstName { get; set; }

        [JsonProperty("clientLastName")]
        public string ClientLastName { get; set; }

        [JsonProperty("services")]
        public List<Service> Services { get; set; }

        [JsonProperty("locations")]
        public List<Location> Locations { get; set; }

        [JsonProperty("designees")]
        public List<Designee> Designees { get; set; }
    }

    public partial class Location
    {
        [JsonProperty("clientLocationId")]
        public int ClientLocationId { get; set; }

        [JsonProperty("locationTypeId")]
        public int LocationTypeId { get; set; }
        
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("zip")]
        public string Zip { get; set; }

        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lon")]
        public double Lon { get; set; }

        [JsonProperty("radius")]
        public long Radius { get; set; }
    }

    public partial class Service
    {
        [JsonProperty("clientServiceId")]
        public int ClientServiceId { get; set; }

        [JsonProperty("serviceId")]
        public int ServiceId { get; set; }

        [JsonProperty("name")]
        public Name Name { get; set; }

        [JsonProperty("shortName")]
        public ShortName ShortName { get; set; }

        [JsonProperty("noteType")]
        public NoteType NoteType { get; set; }

        [JsonProperty("isHCBS")]
        public bool IsHCBS { get; set; }

        [JsonProperty("isTherapy")]
        public bool IsTherapy { get; set; }

        [JsonProperty("isEvaluation")]
        public bool IsEvaluation { get; set; }

        [JsonProperty("authorizations")]
        public List<Authorization> Authorizations { get; set; }

        [JsonProperty("clientNote")]
        [JsonConverter(typeof(ParseAsStringConverter))]
        public string ClientNote { get; set; }

    }

    public partial class Designee
    {
        [JsonProperty("designeeId")]
        public long DesigneeId { get; set; }

        [JsonProperty("guardianId")]
        public long GuardianId { get; set; }

        [JsonProperty("isPrimaryGuardian")]
        public bool IsPrimaryGuardian { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("pin")]
        public string Pin { get; set; }
    }

    public partial class Note
    {
        [JsonProperty("providerId")]
        public long ProviderId { get; set; }

        [JsonProperty("clientId")]
        public long ClientId { get; set; }

        [JsonProperty("clientServiceId")]
        public long ClientServiceId { get; set; }

        [JsonProperty("shortName")]
        public ShortName ShortName { get; set; }

        [JsonProperty("date")]
        public Date Date { get; set; }

        [JsonProperty("clATCNoteId", NullValueHandling = NullValueHandling.Ignore)]
        public long? ClAtcNoteId { get; set; }

        [JsonProperty("note")]
        public string NoteNote { get; set; }

        [JsonProperty("atcCareAreas", NullValueHandling = NullValueHandling.Ignore)]
        public List<AtcCareArea> AtcCareAreas { get; set; }

        [JsonProperty("clRSPNoteId", NullValueHandling = NullValueHandling.Ignore)]
        public long? ClRspNoteId { get; set; }
    }

    public partial class AtcCareArea
    {
        [JsonProperty("careId")]
        public long CareId { get; set; }

        [JsonProperty("careArea")]
        public string CareArea { get; set; }

        [JsonProperty("score")]
        public string Score { get; set; }
    }

    public partial class Authorization
    {
        [JsonProperty("startDt")]
        public string StartDt { get; set; }

        [JsonProperty("endDt")]
        public string EndDt { get; set; }

        [JsonProperty("remainingHours")]
        public double RemainingHours { get; set; }

        [JsonProperty("weeklyHours")]
        public double WeeklyHours { get; set; }

        [JsonProperty("remainingDailyHours")]
        public double RemainingDailyHours { get; set; }
    }

    public partial class HahNote
    {
        [JsonProperty("providerId")]
        public long ProviderId { get; set; }

        [JsonProperty("clientId")]
        public long ClientId { get; set; }

        [JsonProperty("clientServiceId")]
        public long ClientServiceId { get; set; }

        [JsonProperty("shortName")]
        public ShortName ShortName { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("clHAHNoteId")]
        public long ClHahNoteId { get; set; }

        [JsonProperty("hahObjectives")]
        public List<HahObjective> HahObjectives { get; set; }
    }

    public partial class HahObjective
    {
        [JsonProperty("objectiveId")]
        public long ObjectiveId { get; set; }

        [JsonProperty("objective")]
        public string Objective { get; set; }

        [JsonProperty("strategy")]
        public string Strategy { get; set; }

        [JsonProperty("lastDate")]
        public string LastDate { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }

        [JsonProperty("score")]
        public string Score { get; set; }
    }

    public partial class Credential
    {
        [JsonProperty("coId")]
        public long CoId { get; set; }

        [JsonProperty("providerId")]
        public long ProviderId { get; set; }

        [JsonProperty("credId")]
        public long CredId { get; set; }

        [JsonProperty("credTypeId")]
        public long CredTypeId { get; set; }

        [JsonProperty("credName")]
        public string CredName { get; set; }

        [JsonProperty("docId")]
        public string DocId { get; set; }

        [JsonProperty("validFrom")]
        public string ValidFrom { get; set; }

        [JsonProperty("validTo")]
        public string ValidTo { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public partial class Er
    {
        [JsonProperty("code")]
        public long Code { get; set; }

        [JsonProperty("msg")]
        public object Msg { get; set; }
    }

    public partial class MessagingContact
    {
        [JsonProperty("contactId")]
        public long ContactId { get; set; }

        [JsonProperty("contactName")]
        public string ContactName { get; set; }

        [JsonProperty("contactRole")]
        public string ContactRole { get; set; }
    }

    public partial class PendingDocumentation
    {
        [JsonProperty("docId")]
        public string DocId { get; set; }

        [JsonProperty("docType")]
        public string DocType { get; set; }

        [JsonProperty("clientId")]
        public long ClientId { get; set; }

        [JsonProperty("serviceId")]
        public long ServiceId { get; set; }

        [JsonProperty("clientServiceId")]
        public long ClientServiceId { get; set; }

        [JsonProperty("completed")]
        public bool Completed { get; set; }

        [JsonProperty("approved")]
        public bool Approved { get; set; }

        [JsonProperty("lostSession")]
        public bool LostSession { get; set; }

        [JsonProperty("clientName")]
        public string ClientName { get; set; }

        [JsonProperty("dueDt")]
        public string DueDt { get; set; }

        [JsonProperty("svc")]
        [JsonConverter(typeof(NameConverter))]
        public Name Svc { get; set; }

        [JsonProperty("noteType")]
        public string NoteType { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }


    public enum Date { Empty, The992019 };

    public enum ShortName { Atc, Hah, Rsp, Pta, Ota, Sta, Pea, Oea, Sea, Ham, Hsk, Hai, Unknown };

    

    public enum Name { AttendantCare, Habilitation, Respite, PhysicalTherapy, OccupationalTherapy, SpeechTherapy, PhysicalTherapyEval, OccupationalTherapyEval, SpeechTherapyEval, HabilitationWithMusic, HouseKeeping, HabilitaionIndependantLiving, Unknown };

    public enum NoteType { AtcNote, HahNote, RspNote, TherapyNote, TherapyEvalNote };



    public partial class ProviderConfiguration
    {
        public static ProviderConfiguration FromJson(string json) => JsonConvert.DeserializeObject<ProviderConfiguration>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this ProviderConfiguration self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                DateConverter.Singleton,
                ShortNameConverter.Singleton,
                NameConverter.Singleton,
                NoteTypeConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseAsStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var str = JObject.Load(reader).ToString(); 
            return str;
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            writer.WriteRawValue(untypedValue.ToString());
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }

            return 0;
            //can this be logged??
            //throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }

    internal class DateConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Date) || t == typeof(Date?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "":
                    return Date.Empty;
                case "9/9/2019":
                    return Date.The992019;
            }
            throw new Exception("Cannot unmarshal type Date");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Date)untypedValue;
            switch (value)
            {
                case Date.Empty:
                    serializer.Serialize(writer, "");
                    return;
                case Date.The992019:
                    serializer.Serialize(writer, "9/9/2019");
                    return;
            }
            throw new Exception("Cannot marshal type Date");
        }

        public static readonly DateConverter Singleton = new DateConverter();
    }

    internal class ShortNameConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ShortName) || t == typeof(ShortName?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "ATC":
                    return ShortName.Atc;
                case "HAH":
                    return ShortName.Hah;
                case "RSP":
                    return ShortName.Rsp;
                case "HAM":
                    return ShortName.Ham;
                case "HAI":
                    return ShortName.Hai;
                case "HSK":
                    return ShortName.Hsk;
                case "PTA":
                    return ShortName.Pta;
                case "OTA":
                    return ShortName.Ota;
                case "STA":
                    return ShortName.Sta;
                case "PEA":
                    return ShortName.Pea;
                case "OEA":
                    return ShortName.Oea;
                case "SEA":
                    return ShortName.Sea;

            }
            return ShortName.Unknown;
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (ShortName)untypedValue;
            switch (value)
            {
                case ShortName.Atc:
                    serializer.Serialize(writer, "ATC");
                    return;
                case ShortName.Hah:
                    serializer.Serialize(writer, "HAH");
                    return;
                case ShortName.Rsp:
                    serializer.Serialize(writer, "RSP");
                    return;
                case ShortName.Ham:
                    serializer.Serialize(writer, "HAM");
                    return;
                case ShortName.Hai:
                    serializer.Serialize(writer, "HAI");
                    return;
                case ShortName.Hsk:
                    serializer.Serialize(writer, "HSK");
                    return;
                case ShortName.Pta:
                    serializer.Serialize(writer, "PTA");
                    return;
                case ShortName.Ota:
                    serializer.Serialize(writer, "OTA");
                    return;
                case ShortName.Sta:
                    serializer.Serialize(writer, "STA");
                    return;
                case ShortName.Pea:
                    serializer.Serialize(writer, "PEA");
                    return;
                case ShortName.Oea:
                    serializer.Serialize(writer, "OEA");
                    return;
                case ShortName.Sea:
                    serializer.Serialize(writer, "SEA");
                    return;






            }
            throw new Exception("Cannot marshal type ShortName");
        }

        public static readonly ShortNameConverter Singleton = new ShortNameConverter();
    }

    

    internal class NameConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Name) || t == typeof(Name?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "Attendant Care":
                    return Name.AttendantCare;
                case "Habilitation":
                    return Name.Habilitation;
                case "Respite":
                    return Name.Respite;
                case "Habilitation/Music":
                    return Name.HabilitationWithMusic;
                case "Habilitation/Ind. Living":
                    return Name.HabilitaionIndependantLiving;
                case "Homemaker":
                    return Name.HouseKeeping;
                case "Phys. Therapy":
                    return Name.PhysicalTherapy;
                case "Occ. Therapy":
                    return Name.OccupationalTherapy;
                case "Spch. Therapy":
                    return Name.SpeechTherapy;

                case "Phys. Therapy Eval":
                    return Name.PhysicalTherapyEval;
                case "Occ. Therapy Eval":
                    return Name.OccupationalTherapyEval;
                case "Spch. Thearpy Eval":
                    return Name.SpeechTherapyEval;


            }
            return Name.Unknown;
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Name)untypedValue;
            switch (value)
            {
                case Name.AttendantCare:
                    serializer.Serialize(writer, "Attendant Care");
                    return;
                case Name.Habilitation:
                    serializer.Serialize(writer, "Habilitation");
                    return;
                case Name.Respite:
                    serializer.Serialize(writer, "Respite");
                    return;
                case Name.HabilitationWithMusic:
                    serializer.Serialize(writer, "Habilitation/Music");
                    return;
                case Name.HabilitaionIndependantLiving:
                    serializer.Serialize(writer, "Habilitation/Ind. Living");
                    return;
                case Name.HouseKeeping:
                    serializer.Serialize(writer, "Homemaker");
                    return;
                case Name.PhysicalTherapy:
                    serializer.Serialize(writer, "Phys. Therapy");
                    return;
                case Name.OccupationalTherapy:
                    serializer.Serialize(writer, "Occ. Therapy");
                    return;
                case Name.SpeechTherapy:
                    serializer.Serialize(writer, "Spch. Therapy");
                    return;
                case Name.PhysicalTherapyEval:
                    serializer.Serialize(writer, "Phys. Therapy Eval");
                    return;
                case Name.OccupationalTherapyEval:
                    serializer.Serialize(writer, "Occ. Therapy Eval");
                    return;
                case Name.SpeechTherapyEval:
                    serializer.Serialize(writer, "Spch. Thearpy Eval");
                    return;




            }
            serializer.Serialize(writer, "Unknown");
        }

        public static readonly NameConverter Singleton = new NameConverter();
    }

    internal class NoteTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(NoteType) || t == typeof(NoteType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "ATCNote":
                    return NoteType.AtcNote;
                case "HAHNote":
                    return NoteType.HahNote;
                case "RSPNote":
                    return NoteType.RspNote;
                case "TherapyNote":
                    return NoteType.TherapyNote;
                case "TherapyEvalNote":
                    return NoteType.TherapyNote;
            }
            throw new Exception("Cannot unmarshal type NoteType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (NoteType)untypedValue;
            switch (value)
            {
                case NoteType.AtcNote:
                    serializer.Serialize(writer, "ATCNote");
                    return;
                case NoteType.HahNote:
                    serializer.Serialize(writer, "HAHNote");
                    return;
                case NoteType.RspNote:
                    serializer.Serialize(writer, "RSPNote");
                    return;
            }
            throw new Exception("Cannot marshal type NoteType");
        }

        public static readonly NoteTypeConverter Singleton = new NoteTypeConverter();
    }
}
