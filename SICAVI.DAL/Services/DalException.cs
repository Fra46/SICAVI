namespace SICAVI.DAL.Services
{
    public class DalException : Exception
    {
        public DalException(string message, Exception inner)
            : base(message, inner) { }
    }
}