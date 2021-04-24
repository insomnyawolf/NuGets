namespace AzureSamlSSO
{
    public class AzureAuthConfig
    {
        // Nombre de la aplicación en AZURE
        public string Issuer { get; set; }

        // APP Identifier
        public string AppId { get; set; }

        // URL de nuestra aplicación que interpretará la respuesta
        public string Endpoint { get; set; }

        // CertificatePath
        public string CertificatePath { get; set; }
    }
}
