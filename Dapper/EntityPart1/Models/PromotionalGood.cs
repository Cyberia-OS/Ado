using System;

namespace MailingListApp.Models
{
    public class PromotionalGood
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public DateTime EndDate { get; set; }
    }
}
