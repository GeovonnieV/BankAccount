using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BankAccount.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId {get; set;}

        // Amount required to make a transaction
        [Required]
        public decimal Amount {get; set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;

        public int UserId {get;set;}

        // Navigation property for related User object
        // [NotMapped]
        public User AccountUser {get;set;}


    }

}