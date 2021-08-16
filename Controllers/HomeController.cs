using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using BankAccount.Models;
using Microsoft.AspNetCore.Identity;

namespace BankAccount.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;

        // here we can "inject" our context service into the constructor
        public HomeController(MyContext context)
        {
            _context = context;
        }
        // Home / Register page
        [HttpGet("")]
        public IActionResult Index()
        {

            return View();
        }

        // get for user home page
        [HttpGet("Account")]
        // User user is passed into us at log in its the specific user logged in
        public IActionResult AccountHome(User user)
            {
                ViewBag.UsersFirstName = user.FirstName;
                ViewBag.UsersBalance = user.Balance;
                // get all transactions this user has made
                // Makes a list of transactions
                List<Transaction> UsersTransactions = _context.Transactions
                    .Include(t => t.AccountUser)
                    .Where(u => u.AccountUser.UserId == user.UserId)
                    .ToList();
                ViewBag.UsersTransaction = UsersTransactions;

                return View();
            }
        

        // get the Login page
        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }

        // Register Post
        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            // Check initial ModelState
            if (ModelState.IsValid)
            {
                // If a User exists with provided email
                if (_context.Users.Any(u => u.Email == user.Email))
                {
                    // Manually add a ModelState error to the Email field, with provided
                    // error message
                    ModelState.AddModelError("Email", "Email already in use!");

                    // You may consider returning to the View at this point
                    return View("Index");
                }
                // if everything is okay save the user to the DB
                // Initializing a PasswordHasher object, providing our User class as its type
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                _context.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            // other code
            return View("Index");
        }

        // Login Post 
        [HttpPost("LoginPost")]
        public IActionResult LoginPost(LoginUser userSubmission)
        {
            if (ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided email
                var userInDb = _context.Users.FirstOrDefault(u => u.Email == userSubmission.Email);
                // If no user exists with provided email
                if (userInDb == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("SomeView");
                }

                // Initialize hasher object
                var hasher = new PasswordHasher<LoginUser>();

                // verify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);

                // result can be compared to 0 for failure
                if (result == 0)
                {
                    // handle failure (this should be similar to how "existing email" is handled)
                    ModelState.AddModelError("Password", "Not the right password cops are being called!");
                }

                // If everything is good go to the account home view page 
                // pass in the user we found in the db into it
                return RedirectToAction("AccountHome", userInDb);
            }
            // go back to login if fails
            return View("Login");
        }

        // Add a transaction
        [HttpPost("TransactionAdd")]
        public IActionResult TransactionAdd(Transaction newTransaction)
            {
                // get the ammount the user put in
                var newTransactionAmount = newTransaction.Amount;
                // now get the ammount the user has in his account
                 var userInDb = _context.Users.FirstOrDefault(u => u.UserId == newTransaction.UserId);
                // if the new transaction amount is positive aka a deposit just add it to the balance
                if(newTransactionAmount > 0 )
                    {
                        userInDb.Balance += newTransactionAmount;
                    }
                // if newTransaction amount is less than 0 aka withdraw check if that withdraw is bigger than users balance
                    else if(newTransaction.Amount < 0 )
                        {
                            var withdrawResult = userInDb.Balance + newTransactionAmount;
                            // if the withdraw amount is greater than or == 0 everythings good update the users balance 
                            if(withdrawResult >= 0)
                                {
                                    userInDb.Balance = withdrawResult;
                                    _context.SaveChanges();
                                }

                                else{
                                    ModelState.AddModelError("Amount", "You no have enough funds");
                                    return View("AccountHome");
                                }
                        }

                        _context.Add(newTransaction);
                        _context.SaveChanges();
                        return  View("AccountHome");

            }


    }
}
