using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CasaDespaDraft.Models;

namespace CasaDespaDraft.Data;

public class AppDbContext : IdentityDbContext<User>
{
    public DbSet<FAQs> FAQs { get; set; }
    public DbSet<Gallery> Gallery { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    
    //Data Seeding
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // do not remove this!

        modelBuilder.Entity<FAQs>().HasData(
            new FAQs()
            {
                faqsId = 1,
                FAQuestion = "What is the Capacity of the Resort for REGULAR ACCOMODATIONS?",
                FAQAnswer = "Rates are good for 15 persons. But can accomodate a maximum of 20 Persions for an overnight / 22-hr stay*ADDITIONAL of ₱300/head in excess of 15pax.*",
            },

            new FAQs()
            {
            faqsId = 2,
                FAQuestion = "What is the Capacity of the Resort for ACCOMODATIONS with PARTY/EVENTS?",
                FAQAnswer = "ADDITIONAL CHARGE OF ₱3,000, Maximum of 50 pax DURING THE EVENT ONLY., Party Hours: 2pm - 10pm, STRICTLY AFTER 10PM, A MAXIMUM OF 20PAX CAN STAY IN THE VILLA, ALL GATES SHALL BE CLOSED BY 10PM for security purposes",
            },

            new FAQs()
            {
                faqsId = 3,
                FAQuestion = "What are the Sleeping Capacity?",
                FAQAnswer = "We have five(5) available bed; Each bed can accomodate about 2-3 person for a total of 15 persons. We also have ENTRA MATTRESSES FOR RENT",
            },

            new FAQs()
            {
                faqsId = 4,
                FAQuestion = "When can we Check-in?",
                FAQAnswer = "Standard check-in time for a 22-hr stay is at 2pm",
            },

            new FAQs()
            {
                faqsId = 5,
                FAQuestion = "Until when can we Check-out?",
                FAQAnswer = "You may check-out anytime before 12 noon. But, if you wish to extend your stay, kindly notify us 24-hrs prior.",
            },

            new FAQs()
            {
                faqsId = 6,
                FAQuestion = "What is the purpose of the Security Deposit?",
                FAQAnswer = "₱1,000 - A security deposit is required upon arrival for incidentals. This desposit  is fully refundable upon check-out and subbject to a damage inspection of the resort.\r\n",
            }

            );

/*        modelBuilder.Entity<Gallery>().HasData(
            new Gallery()
            {
                imageId = 1,
                image = "~/img/portfolio/Entertainment-1.jpg",
                filter = Filter.Pool
            }

            );*/

    }
}