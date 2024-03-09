using CasaDespaDraft.Data;
using System.ComponentModel.DataAnnotations.Schema;



public class Notification
{
    public int Id { get; set; }

    [ForeignKey("userId")]
    public User User { get; set; }

    public string Subject { get; set; }
    public string Message { get; set; }
    public DateTime SentDate { get; set; }
    public string RecipientEmail { get; set; }

    public string userId { get; set; }


}