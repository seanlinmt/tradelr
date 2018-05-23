namespace OpenSRS
{
    public class MessageBody
    {
        public DataBlock data_block { get; set; }

        public MessageBody()
        {
            data_block = new DataBlock();
        }
    }
}
