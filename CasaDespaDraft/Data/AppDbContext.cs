using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CasaDespaDraft.Models;
using CasaDespaDraft.Controllers;

namespace CasaDespaDraft.Data;

public class AppDbContext : IdentityDbContext<User>
{
    public DbSet<FAQs> FAQs { get; set; }
    public DbSet<Gallery> Gallery { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Homepage> infos { get; set; }
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

        modelBuilder.Entity<Homepage>().HasData(
            new Homepage()
            {
                Id = 1,
                wd22hrs = "₱ 12,500",
                wdMornhrs = "₱ 6,500",
                wdNythrs = "₱ 8,500",
                we22hrs = "₱ 15,000",
                weMornhrs = "₱ 7,000",
                weNythrs = "₱ 9,000",
                af1 = "₱ 300",
                af2 = "₱ 300",
                af3 = "₱ 400",
                af4 = "₱ 100",
                af5 = "₱ 400",
                sd1 = "₱ 1,000",
                c1 = "₱ 1,000",
                c2 = "₱ 1,500",
                oc1 = "₱ 500",
                cIN = "Standard check-in time for a 22-hr stay is at 2 pm\r\n",
                cOUT = "You may check-out anytime before 12 noon. but if you wish to extend your stay, kindly notify us 24 hrs prior.\r\n",
                payment = "30% downpayment is required to secure your booking. DOWNPAYMENT IS STRICTLY Non-Refundable. The Remaining Balance Shall be paid on-site or theough our payment channels.\r\n",
                mc1 = "Rates are good for 15 persons. but we can accommodate a maximum of 20 persons for an overnight/22-hour stay.\r\n**Additional of 300/head in Excess of 15 pax.",
                mc2 = "ADDITIONAL CHARGE OF P 3,000\r\nMaximum of 50 pax DURING THE EVENT ONLY.\r\nParty Hours: 2pm - 10 pm\r\nSTRICTLY AFTER 10 PM, A Maximum of 20 pax can stay in the villa.\r\nAll Gates shall be closed by 10 pm for Security Purposes.",
                clean = "Please Treat our house with the same respect as you would your own home. We’ve put a lot of effort in making this into a lovely space to be enjoyed by our guests. DAMAGED/MISSING ITEMS & STAINED MATTRESSES SHALL BE CHARGED TO GUESTS.\r\n",
                sc1 = "We have Five (5) available beds; Each bed can accommodate about 2-3 persons for a total of 15 persons. We also have Extra Mattresses for Rent\r\n",
                noise = "We hope you have lots of laughter and fun times during your Stay! However, we’d appreciate it if you’re mindful and keep the noise to a minimum during night time.\r\n",
                penalty = "₱ 3,000",
                warning = "If any of our rules and regulations are violated, we have the right to charge extra fees or terminate your stay in our resort.\r\n"

            }

            );;

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