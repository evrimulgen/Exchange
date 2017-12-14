namespace Exchange.Cryptopia.APIResults
{
    public abstract class CryptopiaResponse
    {
        public bool Success { get; set; }
        public object Message { get; set; }
        public object Error { get; set; }
    }
}