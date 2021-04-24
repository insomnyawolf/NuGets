using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace AzureSamlSSO
{
    public enum AuthRequestFormat
    {
        Base64 = 1
    }

    public static class AzureLoginHelper
    {
        #region Mensajes

        public const string CertificateSignatureNotValid = "Certificate signature is not valid";
        public const string LoginNotValid = "Login request is not valid -> Current time: {0} Expected MinTime: {1} Expected MaxTime: {2}";

        #endregion Mensajes

        // There are no other formats supported (yet)
        private const AuthRequestFormat Format = AuthRequestFormat.Base64;

        private static string GetRequestSAML(AzureAuthConfig config)
        {
            string ID = "_" + Guid.NewGuid().ToString();
            string Issue_instant = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");

            using var sw = new StringWriter();

            var xws = new XmlWriterSettings()
            {
                OmitXmlDeclaration = true
            };

            using var xw = XmlWriter.Create(sw, xws);
            xw.WriteStartElement("samlp", "AuthnRequest", "urn:oasis:names:tc:SAML:2.0:protocol");
            // Identificador unico de la request
            xw.WriteAttributeString("ID", ID);
            xw.WriteAttributeString("Version", "2.0");
            // Timestamp del momento en el que se realiza la request
            xw.WriteAttributeString("IssueInstant", Issue_instant);
            xw.WriteAttributeString("IsPassive", "false");

            if (!string.IsNullOrEmpty(config.Endpoint))
            {
                // URL de nuestra aplicación que interpretará la respuesta
                xw.WriteAttributeString("AssertionConsumerServiceURL", config.Endpoint);
            }

            // Nombre de la aplicación en AZURE
            xw.WriteStartElement("Issuer", "urn:oasis:names:tc:SAML:2.0:assertion");
            xw.WriteString(config.Issuer);
            xw.WriteEndElement();

            xw.WriteStartElement("samlp", "NameIDPolicy", null);
            xw.WriteAttributeString("Format", "urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified");
            xw.WriteEndElement();

            // RequestedAuthnContext
            xw.WriteEndElement();

            byte[] bytes = Encoding.UTF8.GetBytes(sw.ToString());

            using var output = new MemoryStream();
            using var zip = new DeflateStream(output, CompressionMode.Compress);

            zip.Write(bytes, 0, bytes.Length);

            string data = Format switch
            {
                AuthRequestFormat.Base64 => Convert.ToBase64String(output.ToArray()),
                _ => throw new NotImplementedException(),
            };

            return HttpUtility.UrlEncode(data);
        }

        public static string GetRequest(AzureAuthConfig config)
        {
            // config.AppId => ID Aplicación a la que se hace la request en azure
            return string.Format($"https://login.microsoftonline.com/{config.AppId}/saml2?SAMLRequest={GetRequestSAML(config)}");
        }

        // To-Do => Optimize Cert Checking
        public static AzureResponse DecodeResponse(string EncodedSAML, AzureAuthConfig config)
        {
            var result = new AzureResponse();

            byte[] SAMLbytes;
            try
            {
                SAMLbytes = Format switch
                {
                    AuthRequestFormat.Base64 => Convert.FromBase64String(EncodedSAML),
                    _ => throw new NotImplementedException(),
                };
            }
            catch (FormatException deserializeException)
            {
                result.IsValid = false;
                result.Error = $"Could not deserialize {nameof(EncodedSAML)}\n{deserializeException.Message}";
                return result;
            }

            string SAML = Encoding.UTF8.GetString(SAMLbytes);

            var serializer = new XmlSerializer(typeof(AzureResponse));

            using var reader = XmlReader.Create(new MemoryStream(SAMLbytes));

            result = (serializer.Deserialize(reader) as AzureResponse);

            // Result is not valid till all validations are completed sucessfully
            result.IsValid = false;

            // Check if it detects unvalid certificate
            X509Certificate2 cert;
            if (!string.IsNullOrEmpty(config.CertificatePath) && (cert = LoadCertificate(config.CertificatePath)) != null)
            {
                var xmlDoc = new XmlDocument()
                {
                    PreserveWhitespace = true,
                    XmlResolver = null
                };
                xmlDoc.LoadXml(SAML);

                XmlNamespaceManager manager = new XmlNamespaceManager(xmlDoc.NameTable);
                manager.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl);
                manager.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");
                manager.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");

                XmlNodeList nodeList = xmlDoc.SelectNodes("//ds:Signature", manager);
                SignedXml signedXml = new SignedXml(xmlDoc);

                signedXml.LoadXml((XmlElement)nodeList[0]);

                try
                {
                    if (!signedXml.CheckSignature(cert, true))
                    {
                        result.IsValid = false;
                        result.Error = CertificateSignatureNotValid;
                        return result;
                    }
                }
                catch (CryptographicException e)
                {
                    result.Error = e.Message;
                    return result;
                }
                catch (ArgumentNullException e)
                {
                    result.Error = e.Message;
                    return result;
                }
            }

            if (NotBefore(result.Assertion.Conditions.NotBefore) && NotOnOrAfter(result.Assertion.Conditions.NotOnOrAfter))
            {
                result.Error = string.Format(
                    LoginNotValid,
                    DateTime.Now, result.Assertion.Conditions.NotBefore, result.Assertion.Conditions.NotOnOrAfter
                    );
                return result;
            }

            result.IsValid = true;
            return result;
        }

        private static bool NotBefore(string timeString)
        {
            if (timeString == null)
                return false;

            DateTime time = DateTime.Parse(timeString);

            return time > DateTime.Now;
        }

        private static bool NotOnOrAfter(string timeString)
        {
            if (timeString == null)
                return false;

            DateTime time = DateTime.Parse(timeString);

            return time <= DateTime.Now;
        }

        private static X509Certificate2 LoadCertificate(string path)
        {
            if (path != null && File.Exists(path))
            {
                File.ReadAllBytes(path);
                return new X509Certificate2(path);
            }
            return null;
        }
    }
}
