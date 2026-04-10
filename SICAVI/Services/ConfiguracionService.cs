using SICAVI.WinUI.Models;

public class ConfiguracionService
{
    public Configuracion ConfiguracionActual { get; private set; }
    public ConfiguracionSMTP SMTPActual { get; private set; }

    public void Cargar()
    {
        ConfiguracionActual = new Configuracion
        {
            NombreTienda = "Mai Hardware Store",
            iva = 0.19m
        };

        SMTPActual = new ConfiguracionSMTP
        {
            Servidor = "smtp.gmail.com",
            Puerto = 587,
            UsarSSL = true
        };
    }

    public void Guardar()
    {
        // Guardar en archivo o base de datos
    }
}