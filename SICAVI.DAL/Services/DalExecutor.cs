using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace SICAVI.DAL.Services
{
    public static class DalExecutor
    {
        public static T Execute<T>(Func<T> action, string contexto = "operación")
        {
            try
            {
                return action();
            }
            catch (SqliteException ex)
            {
                throw new DalException(
                    $"Error de base de datos en {contexto} (código {ex.SqliteErrorCode}): {ex.Message}", ex);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new DalException(
                    $"El registro fue modificado por otro proceso en {contexto}.", ex);
            }
            catch (DbUpdateException ex)
            {
                var inner = ex.InnerException?.Message ?? ex.Message;
                throw new DalException(
                    $"Error al guardar cambios en {contexto}: {inner}", ex);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("context"))
            {
                throw new DalException(
                    $"Error de configuración del contexto en {contexto}.", ex);
            }
            catch (Exception ex) when (ex is not DalException)
            {
                throw new DalException(
                    $"Error inesperado en {contexto}: {ex.Message}", ex);
            }
        }

        public static void Execute(Action action, string contexto = "operación")
        {
            Execute<object?>(() => { action(); return null; }, contexto);
        }
    }
}