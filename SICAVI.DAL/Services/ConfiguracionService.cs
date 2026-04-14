using SICAVI.DAL.Models;

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
            UsarSSL = true,
            Usuario = "andresfzapatamar@gmail.com",
            Contrasena = "jwsg qifn zkdv avdc\r\n"
        };
    }

    public void Guardar()
    {
        // Guardar en archivo o base de datos
    }
}