namespace NOP
{
    public class Error
    {
        public readonly CodeElement Element;
        public readonly string Message;

        public Error(CodeElement element, string message)
        {
            Element = element;
            Message = message;
        }
    }
}
