namespace BITool.Models.SendMail
{
    public class SendMailDto
    {
        public string FromAddress { get; set; }
        public string ToAddresses { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        public string Subject { get; set; }
        public string TextContent { get; set; }
        public string HtmlContent { get; set; }
        public bool IsSendInSeparateEmail { get; set; }
        public bool IsPersonalizedContent { get; set; }
        //public MailBodyParamsDto BodyData { get; set; }
        public bool RequireLayout { get; set; } = true;
        //public AttachmentFileDto AttachmentFile { get; set; }
        public string TemplateName { get; set; }
        public object BodyData { get; set; }
    }
}
