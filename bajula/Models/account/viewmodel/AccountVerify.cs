namespace tradelr.Models.account.viewmodel
{
    /// <summary>
    /// for use when verifying if confirmation code is correct
    /// </summary>
    public class AccountVerify
    {
        public bool isValidCode { get; set; }
        public string domainName { get; set; }
    }
}
