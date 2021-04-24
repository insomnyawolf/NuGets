//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;

//namespace AzureSamlSSO
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AzureLoginController : ControllerBase
//    {
//        private static readonly AzureSamlConfig azureAuthConfig = Startup.Configuration.GetSection(nameof(AzureSamlConfig)).Get<AzureSamlConfig>();

//        public AzureLoginController() { }

//        [HttpGet]
//        [AllowAnonymous]
//        public JsonResult Login()
//        {
//            return new JsonResult(AzureLoginHelper.GetRequest(azureAuthConfig));
//        }

//        [HttpPost]
//        public ActionResult Login([FromForm] string SAMLResponse)
//        {

//            if (SAMLResponse is null)
//            {
//                // Si la peticion no tiene datos no se puede autenticar a nungun usuario
//                return ParseError(HttpStatusCode.BadRequest);
//            }

//            AzureResponse decodedResponse = AzureLoginHelper.DecodeResponse(SAMLResponse, azureAuthConfig);

//            if (decodedResponse is null)
//            {
//                // Si la respuesta decodificada es null, ocurrió un error durante la deserialización
//                return ParseError();
//            }

//            if (!decodedResponse.IsValid)
//            {
//                // Si la respuesta esta marcada como no valida es por que el certificado no es valido
//                // o por que la petición expiró
//                return ParseError(decodedResponse.Error, HttpStatusCode.Conflict);
//            }

//            var attributtes = decodedResponse?.Assertion?.AttributeStatement?.Attribute;

//            if (attributtes is null)
//            {
//                // Si la peticion no tiene atributos, no se puede autenticar al usuario
//                return ParseError(HttpStatusCode.BadRequest);
//            }

//            // Aquí se llega si la peticion es satisfactoria
//            var user = new AzureUser(attributtes);

//            // Poner aquí lógica de sesiones de la aplicacion
//            // Si fuera necesario usar la informacion de los grupos e puede comparar el id con el nombre en el siguiente enlace
//            // https://portal.azure.com/#blade/Microsoft_AAD_IAM/GroupsManagementMenuBlade/AllGroups
//        }
//    }

//    public class AzureUser
//    {
//        public string DisplayName { get; set; }
//        public string EmailAddress { get; set; }
//        public string Company { get; set; }
//        public List<string> Grupos { get; set; }
//        public string Username { get; set; }
//        public string EmployeeId { get; set; }
//        public string TennantId { get; set; }
//        public AzureUser(List<AzureSamlSSO.Attribute> attributtes)
//        {
//            DisplayName = attributtes.SingleOrDefault(x => x.Name.EndsWith("/displayname"))?.AttributeValue[0];
//            EmailAddress = attributtes.SingleOrDefault(x => x.Name.EndsWith("/emailaddress"))?.AttributeValue[0];
//            Company = attributtes.SingleOrDefault(x => x.Name.EndsWith("/company"))?.AttributeValue[0];
//            Grupos = attributtes.SingleOrDefault(x => x.Name.EndsWith("/grupos"))?.AttributeValue;
//            Username = attributtes.SingleOrDefault(x => x.Name.EndsWith("/accountname"))?.AttributeValue[0] ?? EmailAddress?.Split('@')[0];
//            EmployeeId = attributtes.SingleOrDefault(x => x.Name.EndsWith("/employeeId"))?.AttributeValue[0];
//            TennantId = attributtes.SingleOrDefault(x => x.Name.EndsWith("/tenantid"))?.AttributeValue[0];
//        }
//    }
//}
